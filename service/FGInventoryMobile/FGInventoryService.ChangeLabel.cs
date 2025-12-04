
using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;
using System.Data;
namespace erpsolution.service.FGInventoryMobile
{
    public partial class FGInventoryService : ServiceBase<FgRequestRow>
    {
        public async Task<List<UccListDetailDto>> ScanQRtoChangeLabelForCarton(string cartonId)
        {
            try
            {
                var result = await (from mull in _amtContext.MtUccList
                                    join asmt in _amtContext.AoStlmstTbl on new
                                    {
                                        mull.Stlcd,
                                        mull.Stlsiz,
                                        mull.Stlcosn,
                                        mull.Stlrevn
                                    }
                                    equals new
                                    {
                                        asmt.Stlcd,
                                        asmt.Stlsiz,
                                        asmt.Stlcosn,
                                        asmt.Stlrevn
                                    }
                                    where mull.UsedFlag == "Y"
                                          && mull.LabelType == "P"
                                          && mull.CartonId == cartonId
                                    select new UccListDetailDto
                                    {
                                        ByrCd = mull.Byrcd,
                                        Aono = mull.Aono,
                                        Stlcd = mull.Stlcd,
                                        Stlnm = asmt.Stlnm,
                                        Stlsiz = mull.Stlsiz,
                                        Stlcosn = mull.Stlcosn,
                                        Stlclrway = asmt.Stlclrway,
                                        Stlrevn = mull.Stlrevn,
                                        Qty = mull.TotalQty,
                                        CartonId = mull.CartonId,
                                        MixedFlag = mull.MixedFlag,
                                        PatialBox = ((mull.QtyPerCtn ?? 0) - (mull.TotalQty ?? 0)) > 0 ? "Y" : "N"
                                    }).ToListAsync();

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
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string rtnCode, string rtnMsg)> ScanQRtoLabelChange(LabelChangeRequest param)
        {
            bool isprocess = false;
            if (_ApiExcLockService.IsRequestScanQRPending(param.FromCartonId))
            {
                isprocess = true;
                throw new Exception("A request is being saved. Please wait until the current process completes.");
            }
            _ApiExcLockService.MarkRequestScanQRAsPending(param.FromCartonId);
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
            finally
            {
                if (!isprocess)
                    _ApiExcLockService.ClearPendingScanQRRequest(param.FromCartonId);
            }
        }
    }
}
