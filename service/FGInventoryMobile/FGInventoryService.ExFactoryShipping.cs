using erpsolution.dal.EF;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace erpsolution.service.FGInventoryMobile
{
    public partial class FGInventoryService
    {
        public async Task<List<ExFactoryShippingHeaderRow>> GetExFactoryShippingHeadersAsync(string whCode)
        {
            var sql = @"
SELECT MFSM.SHPPKG AS Shppkg
     , INV.INVOICE_NO AS InvoiceNo
     , MFSM.DEST AS Dest
     , MFSM.SCHEDULE_DATE AS ScheduleDate
     , (SELECT C_CODE
          FROM ST_TYPECODE_TBL STCT
         WHERE STCT.C_TYPE='FG_SHIP_STATUS'
           AND STCT.C_ID = MFSM.STATUS
         ) AS Status
     , JOB.JOB_NO AS JobNo
     , MFSM.REMARK AS Remark
  FROM MT_FG_SHIP MFSM
     , MT_FG_MV_ORDER_DTL JOB
     , (SELECT AIMT.INVNO
             , AIMT.USRINVNO AS INVOICE_NO
             , AIDT.SHPPKG
          FROM AO_INVMST_TBL AIMT
             , AO_INVDTL_TBL AIDT
 WHERE AIMT.INVNO = AIDT.INVNO ) INV
 WHERE 1=1          
   AND MFSM.WH_CODE = JOB.WH_CODE(+)
   AND MFSM.SHPPKG = JOB.REQ_NO(+)
   AND MFSM.SHPPKG = INV.SHPPKG
   AND MFSM.STATUS IN (6,8)
   AND MFSM.WH_CODE = :pWhCode";

            var pWhCode = new OracleParameter("pWhCode", OracleDbType.Varchar2, whCode, ParameterDirection.Input);

            var rows = await _amtContext.ExFactoryShippingHeaderRow
                .FromSqlRaw(sql, pWhCode)
                .ToListAsync();

            return rows;
        }

        public async Task<List<ExFactoryShippingLineRow>> GetExFactoryShippingLinesAsync(string shpPkg)
        {
            var sql = @"
SELECT MFSD.SHPPKG AS Shppkg
     , MFSD.LINE_NO AS LineNo
     , MFSD.AONO AS Aono
     , MFSD.STLCD AS Stlcd
     , MFSD.STLSIZ AS Stlsiz
     , MFSD.STLCOSN AS Stlcosn
     , MFSD.STLREVN AS Stlrevn
     , MFSD.RELEASE_QTY AS ReleaseQty
     , MFIM.PICK_QTY AS PickQty
     , MFIM.SHIP_QTY AS ShipQty
     , (SELECT C_CODE
          FROM ST_TYPECODE_TBL STCT
         WHERE STCT.C_TYPE='FG_SHIP_STATUS'
           AND STCT.C_ID = MFSD.STATUS
       ) AS Status
  FROM MT_FG_SHIP_DTL MFSD
     , (SELECT WH_CODE
             , REFER_INFO
             , ORI_AONO AS AONO
             , STLCD
             , STLSIZ
             , STLCOSN
             , ORI_STLREVN AS STLREVN
             , LINE_NO
             , SUM(INPUT_PICK_QTY) AS PICK_QTY
             , SUM(INPUT_SHIP_QTY) AS SHIP_QTY
          FROM MT_FG_INPUT
         WHERE REFER_INFO= :pShpPkg
         GROUP BY
               WH_CODE
             , REFER_INFO
             , ORI_AONO
             , STLCD
             , STLSIZ
             , STLCOSN
             ,  ORI_STLREVN
             , LINE_NO                     
       )MFIM
 WHERE 1=1
   AND MFSD.WH_CODE = MFIM.WH_CODE(+)
   AND MFSD.SHPPKG = MFIM.REFER_INFO(+)
   AND MFSD.LINE_NO= MFIM.LINE_NO(+)      
   AND MFSD.SHPPKG = :pShpPkg";

            var pShpPkg = new OracleParameter("pShpPkg", OracleDbType.Varchar2, shpPkg, ParameterDirection.Input);

            var rows = await _amtContext.ExFactoryShippingLineRow
                .FromSqlRaw(sql, pShpPkg)
                .ToListAsync();

            return rows;
        }

        public async Task<(IReadOnlyList<ExFactoryShippingLineRow> rows, string rtnCode, string rtnMsg)> ScanExFactoryShippingAsync(ExFactoryShippingScanRequest request)
        {
            bool isprocess = false;
            if (_ApiExcLockService.IsRequestScanQRPending(request.CartonId))
            {
                isprocess = true;
                throw new Exception("A request is being saved. Please wait until the current process completes.");
            }
            _ApiExcLockService.MarkRequestScanQRAsPending(request.CartonId);
            CancellationToken ct = default;
            int TrType = 6;  // SHIPPING EX_FACTORY
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, request.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, request.SubwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, request.LocCode, ParameterDirection.Input);
            var pTrType = new OracleParameter("P_TR_TYPE", OracleDbType.Int32, TrType, ParameterDirection.Input);
            var pTrAction = new OracleParameter("P_TR_ACTION", OracleDbType.Int32, request.TrAction, ParameterDirection.Input);
            var pTrInfo = new OracleParameter("P_TR_INFO", OracleDbType.Varchar2, request.TrInfo, ParameterDirection.Input);
            var pCartonId = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, request.CartonId, ParameterDirection.Input);
            var pContainerNo = new OracleParameter("P_CONTAINER_NO", OracleDbType.Varchar2, (object?)request.ContainerNo ?? DBNull.Value, ParameterDirection.Input);
            var pUserId = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, (object?)request.UserId ?? DBNull.Value, ParameterDirection.Input);

            var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10)
            {
                Direction = ParameterDirection.InputOutput,
                Value = "C"
            };
            var pRtnMsg = new OracleParameter("P_RTN_MSG", OracleDbType.Varchar2, 4000)
            {
                Direction = ParameterDirection.Output
            };

            const string plsql = "BEGIN PKAMT.MT_FG_PKG.M_SCAN_INPUT(:P_WH_CODE, :P_SUBWH_CODE, :P_LOC_CODE, :P_TR_TYPE, :P_TR_ACTION, :P_TR_INFO, :P_CARTON_ID, :P_CONTAINER_NO, :P_USER_ID, :P_RTN_CODE, :P_RTN_MSG); END;";

            await using var tx = await _amtContext.Database.BeginTransactionAsync(ct);
            try
            {
                await _amtContext.Database.ExecuteSqlRawAsync(
                    plsql,
                    new object[]
                    {
                        pWhCode,
                        pSubwh,
                        pLoc,
                        pTrType,
                        pTrAction,
                        pTrInfo,
                        pCartonId,
                        pContainerNo,
                        pUserId,
                        pRtnCode,
                        pRtnMsg
                    },
                    ct);

                var rtnCode = (pRtnCode.Value ?? string.Empty).ToString();
                var rtnMsg = (pRtnMsg.Value ?? string.Empty).ToString();

                await tx.CommitAsync(ct);

                var rows = await GetExFactoryShippingLinesAsync(request.TrInfo);

                return (rows, rtnCode, rtnMsg);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
            finally
            {
                if (!isprocess)
                    _ApiExcLockService.ClearPendingScanQRRequest(request.CartonId);
            }
        }
    }
}
