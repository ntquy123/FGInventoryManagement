
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using entities.Common;
using entities.Setting;
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities.Common;
using FGInventoryManagement.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;
using service.Common.Base.Interface;
using service.Interface;
namespace service.Service
{
    public partial class FGInventoryService : ServiceBase<FgRequestRow>
    {
        public async Task<List<UccListDetailDto>> ScanQRtoChangeLabelForCarton(string cartonId)
        {
            try
            {
                var result = await _amtContext.UccListDetailDto.FromSqlInterpolated($@"
            SELECT
                 MULL.BYRCD AS ByrCd
               , MULL.AONO AS Aono
               , MULL.STLCD AS Stlcd
               , ASMT.STLNM AS Stlnm
               , MULL.STLSIZ AS Stlsiz
               , MULL.STLCOSN AS Stlcosn
               , ASMT.STLCLRWAY AS Stlclrway
               , MULL.STLREVN AS Stlrevn
               , MULL.TOTAL_QTY AS Qty
               , MULL.CARTON_ID AS CartonId
               , MULL.MIXED_FLAG AS MixedFlag
               , CASE
                    WHEN MULL.QTY_PER_CTN - MULL.TOTAL_QTY > 0
                    THEN 'Y'
                    ELSE 'N'
                 END AS PatialBox
            FROM MT_UCC_LIST MULL
            INNER JOIN AO_STLMST_TBL ASMT
                ON MULL.STLCD = ASMT.STLCD
               AND MULL.STLSIZ = ASMT.STLSIZ
               AND MULL.STLCOSN = ASMT.STLCOSN
               AND MULL.STLREVN = ASMT.STLREVN
            WHERE MULL.USED_FLAG = 'Y'
              AND MULL.LABEL_TYPE = 'P'
              AND MULL.CARTON_ID = {cartonId}
        ").ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching UccListDetail", ex);
            }
        }

        public async Task<List<MtUccList>> ScanQRtoChangeLabelForBuyer(string cartonId)
        {
            try
            {
                var result = await _amtContext.MtUccList
                    .Where(m => m.UsedFlag == "Y"
                                && m.LabelType == "B"
                                && m.CartonId == cartonId)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching UccListBoxDetail", ex);
            }
        }

        public async Task<(string rtnCode, string rtnMsg)> ScanQRtoLabelChange(LabelChangeRequest param)
        {
            CancellationToken ct = default;

            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.SubwhCode, ParameterDirection.Input);
            var pFrCarton = new OracleParameter("P_FR_CARTON_ID", OracleDbType.Varchar2, param.FromCartonId, ParameterDirection.Input);
            var pToCarton = new OracleParameter("P_TO_CARTON_ID", OracleDbType.Varchar2, param.ToCartonId, ParameterDirection.Input);
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

            const string plsql = @"
        BEGIN
            PKAMT.MT_FG_PKG.LABEL_CHANGE(
                :P_WH_CODE,
                :P_SUBWH_CODE,
                :P_FR_CARTON_ID,
                :P_TO_CARTON_ID,
                :P_USER_ID,
                :P_RTN_CODE,
                :P_RTN_MSG
            );
        END;";

            await using var tx = await _amtContext.Database.BeginTransactionAsync(ct);
            try
            {
                await _amtContext.Database.ExecuteSqlRawAsync(
                    plsql,
                    new object[] { pWhCode, pSubwh, pFrCarton, pToCarton, pUser, pRtnCode, pRtnMsg },
                    ct
                );

                var rtnCode = (pRtnCode.Value ?? "").ToString();
                var rtnMsg = (pRtnMsg.Value ?? "").ToString();

                await tx.CommitAsync(ct);
                return (rtnCode, rtnMsg);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
