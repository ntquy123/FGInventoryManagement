using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace erpsolution.service.FGInventoryMobile
{
    public partial class FGInventoryService
    {
        public async Task<List<UccStockRow>> GetUccByCartonAsync(string cartonId)
        {
            var sql = @"
                SELECT
                  MFSD.WH_CODE     AS FrWhCode,
                  MFSD.SUBWH_CODE  AS FrSubwhCode,
                  (SELECT B.SUBWH_NAME
                     FROM ST_SUBWH_TBL B
                    WHERE B.SUBWH_CODE = MFSD.SUBWH_CODE
                      AND ROWNUM = 1)  AS SubwhName,
                  MFSD.LOC_CODE    AS LocCode,
                  SUBSTR(MULL.AONO, 4, 3) AS ByrCd,
                  MULL.AONO        AS Aono,
                  MULL.STLCD       AS Stlcd,
                  ASMT.STLNM       AS Stlnm,
                  MULL.STLSIZ      AS Stlsiz,
                  MULL.STLCOSN     AS Stlcosn,
                  ASMT.STLCLRWAY   AS Stlclrway,
                  MULL.STLREVN     AS Stlrevn,
                  MULL.TOTAL_QTY   AS TrQty,
                  MULL.CARTON_ID   AS CartonId
                FROM MT_UCC_LIST MULL
                LEFT JOIN (
                    SELECT MFSU.*
                    FROM c MFSM, MT_FG_STOCK_UCC MFSU
                    WHERE MFSM.WH_CODE     = MFSU.WH_CODE
                      AND MFSM.SUBWH_CODE  = MFSU.SUBWH_CODE
                      AND MFSM.LOC_CODE    = MFSU.LOC_CODE
                      AND MFSM.AONO        = MFSU.AONO
                      AND MFSM.STLCD       = MFSU.STLCD
                      AND MFSM.STLSIZ      = MFSU.STLSIZ
                      AND MFSM.STLCOSN     = MFSU.STLCOSN
                      AND MFSM.STLREVN     = MFSU.STLREVN
                      AND MFSU.STOCK_QTY  <> 0
                ) MFSD
                  ON MFSD.AONO      = MULL.AONO
                 AND MFSD.STLCD     = MULL.STLCD
                 AND MFSD.STLSIZ    = MULL.STLSIZ
                 AND MFSD.STLCOSN   = MULL.STLCOSN
                 AND MFSD.STLREVN   = MULL.STLREVN
                 AND MFSD.CARTON_ID = MULL.CARTON_ID
                INNER JOIN AO_STLMST_TBL ASMTMT_UCC_LIST

                        ON MULL.STLCD   = ASMT.STLCD
                       AND MULL.STLSIZ  = ASMT.STLSIZ
                       AND MULL.STLCOSN = ASMT.STLCOSN
                       AND MULL.STLREVN = ASMT.STLREVN
                WHERE MULL.USED_FLAG = 'Y'
                  AND MULL.CARTON_ID = :pCartonId";

            var pCartonId = new OracleParameter("pCartonId", OracleDbType.Varchar2, cartonId, ParameterDirection.Input);

            var rows = await _amtContext.UccStockRow
                .FromSqlRaw(sql, pCartonId)
                .ToListAsync();

            return rows;
        }

        public async Task<(string rtnCode, string rtnMsg)> ScanQRtoTransferLocation(LocTransferRequest param)
        {
            CancellationToken ct = default;

            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.SubwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, param.LocCode, ParameterDirection.Input);
            var pCarton = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, param.CartonId, ParameterDirection.Input);
            var pUser = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, (object?)param.UserId ?? DBNull.Value, ParameterDirection.Input);

            var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10)
            {
                Direction = ParameterDirection.InputOutput,
                Value = "C"
            };
            var pRtnMsg = new OracleParameter("P_RTN_MSG", OracleDbType.Varchar2, 4000)
            {
                Direction = ParameterDirection.Output
            };

            var plsql = "BEGIN PKAMT.MT_FG_PKG.M_LOC_TRANS(:P_WH_CODE, :P_SUBWH_CODE, :P_LOC_CODE, :P_CARTON_ID, :P_USER_ID, :P_RTN_CODE, :P_RTN_MSG); END;";

            using var tx = await _amtContext.Database.BeginTransactionAsync(ct);

            await _amtContext.Database.ExecuteSqlRawAsync(plsql,
                parameters: new object[] { pWhCode, pSubwh, pLoc, pCarton, pUser, pRtnCode, pRtnMsg },
                cancellationToken: ct);

            var rtnCode = (pRtnCode.Value ?? "").ToString();
            var rtnMsg = (pRtnMsg.Value ?? "").ToString();

            await tx.CommitAsync(ct);
            return (rtnCode, rtnMsg);
        }
    }
}
