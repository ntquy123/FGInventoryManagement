using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace service.Service
{
    public partial class FGInventoryService
    {
        public async Task<(IReadOnlyList<FgReceiptResultRow> rows, string rtnCode, string rtnMsg)> ScanQrtoSubWHReceipt(ParamScanQR param)
        {
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.whCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.subwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, param.locCode, ParameterDirection.Input);
            var pRefer = new OracleParameter("P_REFER_INFO", OracleDbType.Varchar2, param.referInfo, ParameterDirection.Input);
            var pCarton = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, param.cartonId, ParameterDirection.Input);
            var pUser = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, param.userId, ParameterDirection.Input);

            var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.InputOutput, Value = "C" };
            var pRtnMsg = new OracleParameter("P_RTN_MSG", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

            var plsql = "BEGIN PKAMT.MT_FG_PKG.M_SUB_RECEIPT(:P_WH_CODE, :P_SUBWH_CODE, :P_LOC_CODE, :P_REFER_INFO, :P_CARTON_ID, :P_USER_ID, :P_RTN_CODE, :P_RTN_MSG); END;";

            using var tx = await _amtContext.Database.BeginTransactionAsync();

            await _amtContext.Database.ExecuteSqlRawAsync(
                plsql,
                pWhCode, pSubwh, pLoc, pRefer, pCarton, pUser, pRtnCode, pRtnMsg);

            var rtnCode = (pRtnCode.Value ?? "").ToString();
            var rtnMsg = (pRtnMsg.Value ?? "").ToString();

            var sqlResult = $@"
SELECT
  REQ.WH_CODE AS WhCode,
  REQ.REQ_NO AS ReqNo,
  REQ.LINE_NO AS LineNo,
  (REQ.REQUEST_QTY - NVL(REQ.CANCEL_QTY,0) - NVL(REQ.RECEIPT_QTY,0)) AS RemainQty,
  REQ.RECEIPT_QTY AS ReceiptQty,
  REQ.STATUS AS Status,
  (SELECT C_NAME
     FROM ST_TYPECODE_TBL
    WHERE C_TYPE='FG_REQUEST_STATUS'
      AND C_ID = REQ.STATUS) AS StatusNm,
  REQ.CARTON_ID AS CartonId
FROM MT_FG_REQUEST_UCC REQ
INNER JOIN AO_STLMST_TBL STL
        ON REQ.STLCD = STL.STLCD
       AND REQ.STLSIZ = STL.STLSIZ
       AND REQ.STLCOSN = STL.STLCOSN
       AND REQ.STLREVN = STL.STLREVN
LEFT JOIN MT_FG_STOCK STK
       ON REQ.WH_CODE = STK.WH_CODE
      AND REQ.SUBWH_CODE = STK.SUBWH_CODE
      AND REQ.AONO = STK.AONO
      AND REQ.STLCD = STK.STLCD
      AND REQ.STLSIZ = STK.STLSIZ
      AND REQ.STLCOSN = STK.STLCOSN
      AND REQ.STLREVN = STK.STLREVN
      AND NVL(REQ.LOC_CODE,'N') = NVL(STK.LOC_CODE,'N')
LEFT JOIN MT_FG_REQUEST MST
       ON MST.WH_CODE = REQ.WH_CODE
      AND MST.SUBWH_CODE = REQ.SUBWH_CODE
      AND MST.REQ_NO = REQ.REQ_NO
WHERE MST.WH_CODE       = '{param.whCode}'
  AND MST.TO_SUBWH_CODE = '{param.subwhCode}'
  AND MST.TR_TYPE       = 2
  AND REQ.STATUS IN (2,3)
  AND REQ.REQ_NO       = '{param.referInfo}'
  AND REQ.CARTON_ID    = '{param.cartonId}'
ORDER BY REQ.LINE_NO";

            var rows = await _amtContext.FgReceiptResultRow
                .FromSqlRaw(sqlResult)
                .ToListAsync();

            await tx.CommitAsync();
            return (rows, rtnCode, rtnMsg);
        }

        public async Task<List<FgRequestRow>> GetHeaderSubWhReceipt(string whCode, string toSubwh)
        {
            var result = await _amtContext.FgRequestRow.FromSqlInterpolated($@"
SELECT *
  FROM (
        SELECT DISTINCT MST.REQ_NO AS ReqNo
             , CASE WHEN SST.JOB_CONTROL = 'N'
                    THEN '@'
                    ELSE JOB.JOB_NO
               END AS JobNo
             , REQ_DATE AS ReqDate
             , MST.SUBWH_CODE AS FrSubwh
             , (SELECT SUBWH_NAME
                 FROM PKAMT.ST_SUBWH_TBL
                WHERE SUBWH_CODE = MST.SUBWH_CODE) AS FrSubwhName
             , SST.JOB_CONTROL AS JobControl
             , (SELECT C_CODE
                  FROM ST_TYPECODE_TBL STCT
                 WHERE STCT.C_TYPE='FG_REQUEST_STATUS'
                   AND STCT.C_ID = MST.STATUS
               ) AS Status
             , MST.CRTID AS CrtId
             , MST.REMARK AS Remark
          FROM MT_FG_REQUEST MST
             , ST_SUBWH_TBL SST
             , (
                SELECT JOBL.*
                  FROM MT_FG_MV_ORDER JOBH
                     , MT_FG_MV_ORDER_DTL JOBL
                 WHERE JOBH.WH_CODE = JOBL.WH_CODE
                   AND JOBH.SUBWH_CODE = JOBL.SUBWH_CODE
                   AND JOBH.JOB_NO = JOBL.JOB_NO
                   AND JOBH.STATUS IN (2,3,5)
               ) JOB
         WHERE MST.TO_SUBWH_CODE = SST.SUBWH_CODE(+)
           AND MST.WH_CODE = JOB.WH_CODE(+)
           AND MST.TO_SUBWH_CODE = JOB.SUBWH_CODE(+)
           AND MST.REQ_NO = JOB.REQ_NO(+)
           AND MST.TR_TYPE=2
           AND MST.STATUS IN (2,3)
           AND MST.WH_CODE= {whCode}
           AND MST.TO_SUBWH_CODE   = {toSubwh}
       )
 WHERE JobNo IS NOT NULL
")
                .ToListAsync();

            return result;
        }

        public async Task<List<FgRequestDetailRow>> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh)
        {
            int trType = 2;
            int status1 = 2;
            int status2 = 3;

            var result = await _amtContext.FgRequestDetailRow
                .FromSqlInterpolated($@"
SELECT
    REQ.WH_CODE AS WhCode,
    (SELECT /*+ FIRST_ROWS(1) */ DISTINCT WMS_WHNAME
       FROM ST_FACTORY_TBL
      WHERE CORPORATIONCD_FORMAL = REQ.WH_CODE) AS WarehouseNm,
    REQ.SUBWH_CODE AS SubwhCode,
    (SELECT SUBWH_NAME FROM PKAMT.ST_SUBWH_TBL WHERE SUBWH_CODE = REQ.SUBWH_CODE) AS SubwhName,
    REQ.REQ_NO AS ReqNo,
    REQ.LINE_NO AS LineNo,
    REQ.AONO AS Aono,
    REQ.STLCD AS Stlcd,
    REQ.STLSIZ AS Stlsiz,
    REQ.STLCOSN AS Stlcosn,
    REQ.STLREVN AS Stlrevn,
    STL.STLNM AS Stlnm,
    STL.STLCLRWAY AS Stlclrway,
    REQ.REQUEST_QTY AS RequestQty,
    REQ.RECEIPT_QTY AS ReceiptQty,
    REQ.CANCEL_QTY AS CancelQty,
    (REQ.REQUEST_QTY - NVL(REQ.CANCEL_QTY,0) - NVL(REQ.RECEIPT_QTY,0)) AS RemainQty,
    REQ.STATUS AS Status,
    (SELECT C_NAME FROM ST_TYPECODE_TBL WHERE C_TYPE='FG_REQUEST_STATUS' AND C_ID = REQ.STATUS) AS StatusNm,
    (SELECT LOC_CONTROL FROM PKAMT.ST_SUBWH_TBL WHERE SUBWH_CODE = MST.TO_SUBWH_CODE) AS LocControl
FROM MT_FG_REQUEST_DTL REQ
INNER JOIN AO_STLMST_TBL STL
        ON REQ.STLCD = STL.STLCD
       AND REQ.STLSIZ = STL.STLSIZ
       AND REQ.STLCOSN = STL.STLCOSN
       AND REQ.STLREVN = STL.STLREVN
LEFT JOIN MT_FG_STOCK STK
       ON REQ.WH_CODE = STK.WH_CODE
      AND REQ.SUBWH_CODE = STK.SUBWH_CODE
      AND REQ.AONO = STK.AONO
      AND REQ.STLCD = STK.STLCD
      AND REQ.STLSIZ = STK.STLSIZ
      AND REQ.STLCOSN = STK.STLCOSN
      AND REQ.STLREVN = STK.STLREVN
      AND NVL(REQ.LOC_CODE,'N') = NVL(STK.LOC_CODE,'N')
LEFT JOIN MT_FG_REQUEST MST
       ON MST.WH_CODE = REQ.WH_CODE
      AND MST.SUBWH_CODE = REQ.SUBWH_CODE
      AND MST.REQ_NO = REQ.REQ_NO
WHERE MST.WH_CODE       = {whCode}
  AND MST.TO_SUBWH_CODE = {toSubwh}
  AND MST.TR_TYPE       = {trType}
  AND REQ.STATUS IN ({status1}, {status2})
  AND REQ.REQ_NO       = {reqNo}
ORDER BY REQ.LINE_NO")
                .ToListAsync();

            return result;
        }
    }
}
