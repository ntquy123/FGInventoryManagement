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
        public async Task<List<WHReceiptHeaderRow>> GetReceiptHeadersAsync(string whCode, string subwhCode)
        {
            var sql = @"
SELECT
    AMMT.INVNO      AS Invno,
    AMMT.USRINVNO   AS InvoiceNo,
    AMMT.CSTSHTNO   AS Cstshtno,
    AMMT.TO_WHCODE  AS Whcode,
    AMMT.WHCODE     AS FromWhcode,
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
            MAX(REQ_NO) AS REQ_NO,
            MAX(STATUS) AS STATUS,
            MIN(JOB_NO) AS JOB_NO
          FROM (
               SELECT
                    MFI.ATTRIBUTE2 AS INVNO,
                    MFI.WH_CODE,
                    MFRM.TO_WH_CODE,
                    NVL(JOB.JOB_NO, NULL) AS JOB_NO,
                    MFI.REFER_INFO AS REQ_NO,
                    MFI.STATUS     AS STATUS
                  FROM MT_FG_REQUEST MFRM,
                       MT_FG_REQUEST_DTL MFRD,
                       MT_FG_INPUT MFI,
                       MT_FG_MV_ORDER_DTL JOB
                 WHERE     MFRM.WH_CODE = MFRD.WH_CODE
                       AND MFRM.SUBWH_CODE = MFRD.SUBWH_CODE
                       AND MFRM.REQ_NO = MFRD.REQ_NO
                       AND MFI.WH_CODE = MFRD.WH_CODE
                       AND MFI.SUBWH_CODE = MFRD.SUBWH_CODE
                       AND MFI.LOC_CODE = MFRD.LOC_CODE
                       AND MFI.REFER_INFO = MFRD.REQ_NO
                       AND MFI.LINE_NO = MFRD.LINE_NO
                       AND MFRM.TO_WH_CODE = JOB.WH_CODE(+)
                       AND MFRM.REQ_NO = JOB.REQ_NO(+)
                       AND MFI.STATUS IN (3, 7)
                       AND NVL(MFI.SHIP_QTY, 0) - NVL(MFI.RECEIPT_QTY, 0) > 0
               )
         GROUP BY INVNO, WH_CODE
     ) MFIP
WHERE AMMT.INVNO = MFIP.INVNO
  AND AMMT.WHCODE = MFIP.WH_CODE
  AND AMMT.TO_WHCODE = :pWhCode";

            var pWhCode = new OracleParameter("pWhCode", OracleDbType.Varchar2, whCode, ParameterDirection.Input);
            var rows = await _amtContext.WHReceiptHeaderRow
                .FromSqlRaw(sql, pWhCode)
                .ToListAsync();

            return rows;
        }

        public async Task<List<WHReceiptLineRow>> GetReceiptLinesAsync(string invoiceNo)
        {
            var sql = @"
SELECT
    MFI.REFER_INFO       AS ReqNo,
    MFRD.LINE_NO         AS LineNo,
    MFRD.AONO            AS Aono,
    MFRD.STLCD           AS Stlcd,
    MFRD.STLSIZ          AS Stlsiz,
    MFRD.STLCOSN         AS Stlcosn,
    MFRD.STLREVN         AS Stlrevn,
    MFRD.SHIP_QTY        AS RequestQty,
    NVL(MFRD.RECEIPT_QTY, 0) AS ReceiptQty,
    MFRD.STATUS          AS Status,
    (SELECT C_NAME
       FROM ST_TYPECODE_TBL
      WHERE C_TYPE = 'FG_REQUEST_STATUS'
        AND C_ID = MFRD.STATUS) AS StatusNm
FROM MT_FG_INPUT MFI,
     MT_FG_REQUEST_DTL MFRD,
     ST_SUBWH_TBL SST
WHERE     MFI.WH_CODE = SST.WH_CODE
      AND MFI.SUBWH_CODE = SST.SUBWH_CODE
      AND MFI.WH_CODE = MFRD.WH_CODE
      AND MFI.SUBWH_CODE = MFRD.SUBWH_CODE
      AND MFI.LOC_CODE = MFRD.LOC_CODE
      AND MFI.REFER_INFO = MFRD.REQ_NO
      AND MFI.LINE_NO = MFRD.LINE_NO
      AND MFI.STATUS IN (3, 7)
      AND MFI.ATTRIBUTE2 = :pInvoiceNo";

            var pInvoiceNo = new OracleParameter("pInvoiceNo", OracleDbType.Varchar2, invoiceNo, ParameterDirection.Input);

            var rows = await _amtContext.WHReceiptLineRow
                .FromSqlRaw(sql, pInvoiceNo)
                .ToListAsync();

            return rows;
        }

        public async Task<(IReadOnlyList<WHReceiptLineRow> rows, string rtnCode, string rtnMsg)> ScanReceiptAsync(WHReceiptScanRequest request)
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
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, request.LocCode, ParameterDirection.Input);
            var pInvNo = new OracleParameter("P_INVNO", OracleDbType.Varchar2, request.InvNo, ParameterDirection.Input);
            var pCartonId = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, request.CartonId, ParameterDirection.Input);
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

            const string plsql = "BEGIN PKAMT.MT_FG_PKG. M_WH_RECEIPT(:P_WH_CODE,:P_SUBWH_CODE,:P_LOC_CODE,:P_INVNO,:P_CARTON_ID,:P_USER_ID,:P_RTN_CODE,:P_RTN_MSG); END;";

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
                        pInvNo,
                        pCartonId,
                        pUserId,
                        pRtnCode,
                        pRtnMsg
                    },
                    ct);

                var rtnCode = (pRtnCode.Value ?? string.Empty).ToString();
                var rtnMsg = (pRtnMsg.Value ?? string.Empty).ToString();

                await tx.CommitAsync(ct);

                var rows = await GetReceiptLinesAsync(request.InvNo);

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
