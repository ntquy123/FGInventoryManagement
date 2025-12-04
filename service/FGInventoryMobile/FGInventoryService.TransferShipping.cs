using erpsolution.dal.EF;
using erpsolution.service.Interface.SystemMaster;
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
        public async Task<List<TransferShippingHeaderRow>> GetTransferShippingHeadersAsync(string whCode, string subwhCode)
        {
            var sql = @"
SELECT
    AMMT.INVNO      AS Invno,
    AMMT.USRINVNO   AS InvoiceNo,
    AMMT.CSTSHTNO   AS Cstshtno,
    AMMT.TO_WHCODE  AS ToWhcode,
    (SELECT C_CODE
       FROM ST_TYPECODE_TBL STCT
      WHERE STCT.C_TYPE = 'FG_REQUEST_STATUS'
        AND STCT.C_ID   = MFIP.STATUS) AS Status,
    MFIP.JOB_NO     AS JobNo
FROM AO_MOVMST_TBL AMMT,
     (
        SELECT
            INVNO,
            WH_CODE,
            MAX(REQ_NO)  AS REQ_NO,
            MAX(STATUS)  AS STATUS,
            MIN(JOB_NO)  AS JOB_NO
          FROM (
               SELECT
                    MFI.ATTRIBUTE2 AS INVNO,
                    MFI.WH_CODE,
                    MFI.SUBWH_CODE,
                    CASE
                        WHEN SST.JOB_CONTROL = 'N' THEN '@'
                        ELSE NVL(JOB.JOB_NO, NULL)
                    END AS JOB_NO,
                    MFI.REFER_INFO AS REQ_NO,
                    MFI.STATUS     AS STATUS
                  FROM MT_FG_INPUT MFI,
                       MT_FG_REQUEST_DTL MFRD,
                       ST_SUBWH_TBL SST,
                       MT_FG_MV_ORDER_DTL JOB
                 WHERE     MFI.WH_CODE = SST.WH_CODE
                       AND MFI.SUBWH_CODE = SST.SUBWH_CODE
                       AND MFI.WH_CODE = MFRD.WH_CODE
                       AND MFI.SUBWH_CODE = MFRD.SUBWH_CODE
                       AND MFI.LOC_CODE = MFRD.LOC_CODE
                       AND MFI.REFER_INFO = MFRD.REQ_NO
                       AND MFI.LINE_NO = MFRD.LINE_NO
                       AND MFI.WH_CODE = JOB.WH_CODE(+)
                       AND MFI.SUBWH_CODE = JOB.SUBWH_CODE(+)
                       AND MFI.REFER_INFO = JOB.REQ_NO(+)
                       AND MFI.STATUS IN (6, 8)
                       AND SST.PICK_RULE = 'M'
                       AND NVL(MFI.INPUT_PICK_QTY, 0) > 0
                UNION ALL
                SELECT
                    MFI.ATTRIBUTE2 AS INVNO,
                    MFI.WH_CODE,
                    MFI.SUBWH_CODE,
                    CASE
                        WHEN SST.JOB_CONTROL = 'N' THEN '@'
                        ELSE NVL(JOB.JOB_NO, NULL)
                    END AS JOB_NO,
                    MFI.REFER_INFO AS REQ_NO,
                    MFI.STATUS     AS STATUS
                  FROM MT_FG_INPUT MFI,
                       MT_FG_REQUEST_DTL MFRD,
                       ST_SUBWH_TBL SST,
                       MT_FG_MV_ORDER_DTL JOB
                 WHERE     MFI.WH_CODE = SST.WH_CODE
                       AND MFI.SUBWH_CODE = SST.SUBWH_CODE
                       AND MFI.WH_CODE = MFRD.WH_CODE
                       AND MFI.SUBWH_CODE = MFRD.SUBWH_CODE
                       AND MFI.LOC_CODE = MFRD.LOC_CODE
                       AND MFI.REFER_INFO = MFRD.REQ_NO
                       AND MFI.LINE_NO = MFRD.LINE_NO
                       AND MFI.WH_CODE = JOB.WH_CODE(+)
                       AND MFI.SUBWH_CODE = JOB.SUBWH_CODE(+)
                       AND MFI.REFER_INFO = JOB.REQ_NO(+)
                       AND MFI.STATUS IN (6, 8)
                       AND SST.PICK_RULE = 'A'
               )
         GROUP BY INVNO, WH_CODE
        ) MFIP
WHERE AMMT.INVNO = MFIP.INVNO
  AND AMMT.WHCODE = MFIP.WH_CODE
  AND MFIP.JOB_NO IS NOT NULL
  AND AMMT.WHCODE = :pWhCode";

            var pWhCode = new OracleParameter("pWhCode", OracleDbType.Varchar2, whCode, ParameterDirection.Input);

            var rows = await _amtContext.TransferShippingHeaderRow
                .FromSqlRaw(sql, pWhCode)
                .ToListAsync();

            return rows;
        }

        public async Task<List<TransferShippingLineRow>> GetTransferShippingLinesAsync(string Shppkg)
        {
            var sql = @"
SELECT
    MFSD.SHPPKG AS Shppkg,
    MFSD.LINE_NO AS LineNo,
    MFSD.AONO AS Aono,
    MFSD.STLCD AS Stlcd,
    MFSD.STLSIZ AS Stlsiz,
    MFSD.STLCOSN AS Stlcosn,
    MFSD.STLREVN AS Stlrevn,
    MFSD.RELEASE_QTY AS ReleaseQty,
    MFIM.PICK_QTY AS PickQty,
    MFIM.SHIP_QTY AS ShipQty,
    (SELECT C_CODE
       FROM ST_TYPECODE_TBL STCT
      WHERE STCT.C_TYPE='FG_SHIP_STATUS'
        AND STCT.C_ID = MFSD.STATUS
    ) AS Status
FROM MT_FG_SHIP_DTL MFSD,
     (SELECT WH_CODE
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
         WHERE REFER_INFO= :pShppkg
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
   AND MFSD.SHPPKG = :pShppkg";

            var pShppkg = new OracleParameter("pShppkg", OracleDbType.Varchar2, Shppkg, ParameterDirection.Input);

            var rows = await _amtContext.TransferShippingLineRow
                .FromSqlRaw(sql, pShppkg)
                .ToListAsync();

            return rows;
        }

        public async Task<(IReadOnlyList<TransferShippingLineRow> rows, string rtnCode, string rtnMsg)> ScanTransferShippingAsync(TransferShippingScanRequest request)
        {
            bool isprocess = false;
            if (_ApiExcLockService.IsRequestScanQRPending(request.CartonId))
            {
                isprocess = true;
                throw new Exception("A request is being saved. Please wait until the current process completes.");
            }
            _ApiExcLockService.MarkRequestScanQRAsPending(request.CartonId);
            CancellationToken ct = default;
            int TrType = 4;
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, request.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, request.SubwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, (object?)request.LocCode ?? DBNull.Value, ParameterDirection.Input);
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

                //string rtnCode = Convert.ToString(pRtnCode.Value) ?? "";
                //string rtnMsg = Convert.ToString(pRtnMsg.Value) ?? "";

                await tx.CommitAsync(ct);

                var rows = await GetTransferShippingLinesAsync(request.TrInfo);

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
