using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace service.Service
{
    public partial class FGInventoryService
    {
        public async Task<List<Pccount>> GetPcCountAsync(string whCode, string subwhCode)
        {
            var result = await _amtContext.Set<Pccount>()
                .FromSqlInterpolated($@"
                SELECT  PC_NAME,
                        CRTDAT,
                        FR_LOC,
                        TO_LOC,
                        (SELECT C_CODE
                           FROM ST_TYPECODE_TBL
                          WHERE C_TYPE = 'FG_PC_STATUS'
                            AND C_ID   = MT_FG_PCOUNT.STATUS) AS STATUS
                  FROM MT_FG_PCOUNT
                 WHERE STATUS      = 1
                   AND WH_CODE     = {whCode}
                   AND SUBWH_CODE  = {subwhCode}")
                .ToListAsync();

            return result;
        }

        public async Task<List<Pcinput>> ExecutePcInputAndQueryAsync(Pcinputrequest req)
        {
            string? rtnCode;
            string? rtnMsg;

            await using var conn = (OracleConnection)_amtContext.Database.GetDbConnection();
            var needClose = false;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
                needClose = true;
            }

            try
            {
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.BindByName = true;
                    cmd.CommandText = "PKAMT.MT_FG_PKG.M_PC_INPUT";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_WH_CODE", OracleDbType.Varchar2, req.WhCode, ParameterDirection.Input);
                    cmd.Parameters.Add("P_SUBWH_CODE", OracleDbType.Varchar2, req.SubwhCode, ParameterDirection.Input);
                    cmd.Parameters.Add("P_PC_NAME", OracleDbType.Varchar2, req.PcName, ParameterDirection.Input);
                    cmd.Parameters.Add("P_LOC_CODE", OracleDbType.Varchar2, req.LocCode, ParameterDirection.Input);
                    cmd.Parameters.Add("P_TR_ACTION", OracleDbType.Int32, req.TrAction, ParameterDirection.Input);
                    cmd.Parameters.Add("P_CARTON_ID", OracleDbType.Varchar2, req.CartonId, ParameterDirection.Input);
                    cmd.Parameters.Add("P_USER_ID", OracleDbType.Varchar2, req.UserId, ParameterDirection.Input);

                    var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10, null, ParameterDirection.InputOutput)
                    { Value = DBNull.Value };
                    cmd.Parameters.Add(pRtnCode);

                    var pRtnMsg = new OracleParameter("P_RTN_MSG", OracleDbType.Varchar2, 4000, null, ParameterDirection.InputOutput)
                    { Value = DBNull.Value };
                    cmd.Parameters.Add(pRtnMsg);

                    await cmd.ExecuteNonQueryAsync();

                    rtnCode = pRtnCode.Value?.ToString();
                    rtnMsg = pRtnMsg.Value?.ToString();
                }

                var data = await _amtContext.Set<Pcinput>().FromSqlInterpolated($@"
SELECT MFSU.WH_CODE AS WH_CODE
     , MFSU.SUBWH_CODE FR_SUBWH_CODE
     , (SELECT B.SUBWH_NAME
          FROM ST_SUBWH_TBL B
         WHERE B.SUBWH_CODE = MFSU.SUBWH_CODE
           AND ROWNUM=1) AS SUBWH_NAME
     , MFSU.LOC_CODE AS LOC_CODE
     , MFSU.BYRCD
     , MFSU.AONO
     , MFSU.STLCD
     , ASMT.STLNM
     , MFSU.STLSIZ
     , MFSU.STLCOSN
     , ASMT.STLCLRWAY
     , MFSU.STLREVN
     , MFSU.STOCK_QTY AS QTY
     , (SELECT SUM(TOTAL_QTY)
          FROM MT_UCC_LIST MULL
         WHERE MULL.CARTON_ID = MFSU.CARTON_ID) AS TOTAL_QTY
     , MFSU.CARTON_ID
  FROM MT_FG_STOCK MFSM
     , MT_FG_STOCK_UCC MFSU
     , AO_STLMST_TBL ASMT
 WHERE MFSM.WH_CODE     = MFSU.WH_CODE
   AND MFSM.SUBWH_CODE  = MFSU.SUBWH_CODE
   AND MFSM.LOC_CODE    = MFSU.LOC_CODE
   AND MFSM.AONO        = MFSU.AONO
   AND MFSM.STLCD       = MFSU.STLCD
   AND MFSM.STLSIZ      = MFSU.STLSIZ
   AND MFSM.STLCOSN     = MFSU.STLCOSN
   AND MFSM.STLREVN     = MFSU.STLREVN
   AND MFSU.STOCK_QTY  <> 0
   AND MFSU.STLCD       = ASMT.STLCD
   AND MFSU.STLSIZ      = ASMT.STLSIZ
   AND MFSU.STLCOSN     = ASMT.STLCOSN
   AND MFSU.STLREVN     = ASMT.STLREVN
   AND MFSU.CARTON_ID   = {req.CartonId}")
                    .ToListAsync();

                var statusText = rtnCode switch
                {
                    "C" => "Complete",
                    "E" => "Error",
                    _ => "Unknown"
                };

                if (data.Count == 0)
                {
                    data.Add(new Pcinput
                    {
                        Carton_id = req.CartonId,
                        Statuscode = rtnCode,
                        Status = statusText,
                        Errormsg = rtnMsg
                    });
                }
                else
                {
                    foreach (var row in data)
                    {
                        row.Statuscode = rtnCode;
                        row.Status = statusText;
                        row.Errormsg = rtnMsg;
                    }
                }

                return data;
            }
            finally
            {
                if (needClose) await conn.CloseAsync();
            }
        }
    }
}
