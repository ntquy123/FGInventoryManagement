using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace service.Service
{
    public class FGInventoryService :   IFGInventoryService
    {
        public FGInventoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override string PrimaryKey => throw new NotImplementedException();
        #region [========================Sub-WH Receipt===========================================]
        public async Task<(IReadOnlyList<FgReceiptResultRow> rows, string rtnCode, string rtnMsg)> ScanQrtoSubWHReceipt(ParamScanQR param)
        {
            // 1) Gọi procedure
            // Giả định: P_RTN_CODE là IN OUT, P_RTN_MSG là OUT (nếu khác, chỉnh Direction tương ứng)
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.whCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.subwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, param.locCode, ParameterDirection.Input);
            var pRefer = new OracleParameter("P_REFER_INFO", OracleDbType.Varchar2, param.referInfo, ParameterDirection.Input);
            var pCarton = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, param.cartonId, ParameterDirection.Input);
            var pUser = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, param.userId, ParameterDirection.Input);

            var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.InputOutput, Value = "C" };
            var pRtnMsg = new OracleParameter("P_RTN_MSG", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

            // Nếu package đòi thêm P_LINE_NO, P_QTY như comment thì thêm tương tự:
            // var pLineNo = new OracleParameter("P_LINE_NO", OracleDbType.Int32) { Direction = ParameterDirection.InputOutput, Value = DBNull.Value };
            // var pQty    = new OracleParameter("P_QTY",    OracleDbType.Decimal) { Direction = ParameterDirection.InputOutput, Value = DBNull.Value };

            var plsql = "BEGIN PKAMT.MT_FG_PKG.M_SUB_RECEIPT(:P_WH_CODE, :P_SUBWH_CODE, :P_LOC_CODE, :P_REFER_INFO, :P_CARTON_ID, :P_USER_ID, :P_RTN_CODE, :P_RTN_MSG); END;";

            // Có thể dùng transaction (khuyến nghị)
            using var tx = await _amtContext.Database.BeginTransactionAsync();

            await _amtContext.Database.ExecuteSqlRawAsync(
                plsql,
                pWhCode, pSubwh, pLoc, pRefer, pCarton, pUser, pRtnCode, pRtnMsg
            // , pLineNo, pQty
            );

            var rtnCode = (pRtnCode.Value ?? "").ToString();
            var rtnMsg = (pRtnMsg.Value ?? "").ToString();

            // Nếu proc báo lỗi (tuỳ convention), bạn có thể kiểm tra và throw:
            // ví dụ coi như 'S' = Success, 'E' = Error
            // if (!string.Equals(rtnCode, "S", StringComparison.OrdinalIgnoreCase))
            // {
            //     await tx.RollbackAsync();
            //     return (Array.Empty<FgReceiptResultRow>(), rtnCode, rtnMsg);
            // }

            // 2) RESULT QTY SQL (param hoá)
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
            try
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

                //var result = await _amtContext.UserMenuRoleView.FromSqlRaw(sqlQuery).AsNoTracking().ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FgRequestDetailRow>> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh)
        {
            try
            {
                int trType = 2;
                int status1 = 2;
                int status2 = 3;
                //string reqNo = "I-GA2-250730-0003";

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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region [======================= Location Transfer =======================]
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
                    FROM MT_FG_STOCK MFSM, MT_FG_STOCK_UCC MFSU
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
                INNER JOIN AO_STLMST_TBL ASMT
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
            // Tham số vào/ra
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.SubwhCode, ParameterDirection.Input);
            var pLoc = new OracleParameter("P_LOC_CODE", OracleDbType.Varchar2, param.LocCode, ParameterDirection.Input);
            var pCarton = new OracleParameter("P_CARTON_ID", OracleDbType.Varchar2, param.CartonId, ParameterDirection.Input);
            var pUser = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, (object?)param.UserId ?? DBNull.Value, ParameterDirection.Input);

            // IN OUT / OUT
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
        #endregion

        #region [======================= Change to label =======================]


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
        public async Task<List<UccListBoxDto>> ScanQRtoChangeLabelForBuyer(string cartonId)
        {
            try
            {
                var result = await _amtContext.UccListBoxDto.FromSqlInterpolated($@"
            SELECT 
                 MULL.BYRCD       AS ByrCd
               , MULL.BYR_PONO    AS ByrPono
               , MULL.STLCD       AS Stlcd
               , MULL.BYR_STLNAME AS ByrStlname
               , MULL.BYR_STLCLR  AS ByrStlclr
               , MULL.BYR_STLCLRWAY AS ByrStlclrway
               , MULL.TOTAL_QTY   AS Qty
               , MULL.CARTON_ID   AS CartonId
               , MULL.MIXED_FLAG  AS MixedFlag
               , CASE 
                    WHEN MULL.QTY_PER_CTN - MULL.TOTAL_QTY > 0 
                    THEN 'Y' 
                    ELSE 'N' 
                 END AS PatialBox
            FROM MT_UCC_LIST MULL
            WHERE MULL.USED_FLAG = 'Y'
              AND MULL.LABEL_TYPE = 'B'
              AND MULL.CARTON_ID = {cartonId}
        ").ToListAsync();

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

            // IN params
            var pWhCode = new OracleParameter("P_WH_CODE", OracleDbType.Varchar2, param.WhCode, ParameterDirection.Input);
            var pSubwh = new OracleParameter("P_SUBWH_CODE", OracleDbType.Varchar2, param.SubwhCode, ParameterDirection.Input);
            var pFrCarton = new OracleParameter("P_FR_CARTON_ID", OracleDbType.Varchar2, param.FromCartonId, ParameterDirection.Input);
            var pToCarton = new OracleParameter("P_TO_CARTON_ID", OracleDbType.Varchar2, param.ToCartonId, ParameterDirection.Input);
            var pUser = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, (object?)param.UserId ?? DBNull.Value, ParameterDirection.Input);

            // IN OUT / OUT
            var pRtnCode = new OracleParameter("P_RTN_CODE", OracleDbType.Varchar2, 10)
            {
                Direction = ParameterDirection.InputOutput,
                Value = "C"    // default như PL/SQL mẫu
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
        #endregion

        #region [========================= BUyer labelupload ================]
        public async Task<List<StByrmstTbl>> GetComBoBoxForBuyer()
        {
            try
            {
                var result = await _amtContext.StByrmstTbl.FromSqlInterpolated($@"SELECT BYRCD , BYRNM FROM ST_BYRMST_TBL WHERE USEYN='Y'")
                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<MtUccListUpload>> SaveBuyerLabelUpload(DataSaveLableUpload Data)
        {
            try
            {
                string scanId = await GetScanIdAsync(isExcel: false);
                var nowDate = DateTime.Now;
                int seq = 0;
                var dataSave = Data.lstCartonId.Select(x => new MtUccListUpload
                {
                    XlsId = scanId,
                    XlsSq = seq++,
                    WhCode = Data.WhCode,
                    CartonId = x,
                    ByrCd = Data.BuyerCd,
                    LabelType = "B",
                    Status = "N",
                    CrtDat = nowDate,
                    UptDat = nowDate,
                    CrtId = Data.UserId,
                    UptId = Data.UserId,

                }).ToList();
                await _amtContext.MtUccListUpload.AddRangeAsync(dataSave);
                await _amtContext.SaveChangesAsync();
                return dataSave;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> GetScanIdAsync(
       bool isExcel,
       DateTime? date = null,
       string module = "UCC_UPLOAD",
       string separator = "@",
       int width = 6)
        {
            // Chuyển type char
            string typeChar = isExcel ? "E" : "S";

            // Format ngày YYYYMMDD
            DateTime useDate = date ?? DateTime.UtcNow;
            string dateStr = useDate.ToString("yyyyMMdd");

            // SQL: gọi function trả về 1 giá trị
            const string sql = "SELECT PK_NUMBERING_RULES_FC(:P_MODULE, :P_SEP, :P_WIDTH, :P_TYPE, :P_DATE) FROM DUAL";

            // Lấy DbConnection từ DbContext
            var connection = _amtContext.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                // Tạo tham số theo DbProvider (provider-agnostic)
                var pModule = command.CreateParameter();
                pModule.ParameterName = "P_MODULE";
                pModule.DbType = DbType.String;
                pModule.Value = module;
                command.Parameters.Add(pModule);

                var pSep = command.CreateParameter();
                pSep.ParameterName = "P_SEP";
                pSep.DbType = DbType.String;
                pSep.Value = separator;
                command.Parameters.Add(pSep);

                var pWidth = command.CreateParameter();
                pWidth.ParameterName = "P_WIDTH";
                // NUMBER -> dùng Int32 hoặc Decimal tuỳ provider; dùng Int32 vì width là số nguyên
                pWidth.DbType = DbType.Int32;
                pWidth.Value = width;
                command.Parameters.Add(pWidth);

                var pType = command.CreateParameter();
                pType.ParameterName = "P_TYPE";
                pType.DbType = DbType.String;
                pType.Value = typeChar;
                command.Parameters.Add(pType);

                var pDate = command.CreateParameter();
                pDate.ParameterName = "P_DATE";
                pDate.DbType = DbType.String;
                pDate.Value = dateStr;
                command.Parameters.Add(pDate);

                // Thực thi và lấy kết quả
                var result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToString(result);
            }
            catch
            {
                // Tùy bạn: log lỗi ở đây trước khi rethrow hoặc trả null
                // ví dụ: _logger.LogError(ex, "Failed to get scan id");
                throw;
            }
            finally
            {
                // Đóng connection ở đây nếu bạn muốn (DbContext có thể quản lý connection)
                // Tuy nhiên tránh đóng connection nếu bạn dùng connection pooling quản lý bởi DbContext
                if (connection.State == ConnectionState.Open)
                {
                    // Nếu bạn không muốn đóng (DbContext tiếp tục dùng), bỏ dòng dưới
                    await connection.CloseAsync();
                }
            }
        }
        #endregion

        #region [========================= Physical Counting ================]
        public async Task<List<Pccount>> GetPcCountAsync(string whCode, string subwhCode)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
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
                // 1) Gọi proc
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

                // 2) Query data sau proc (lọc theo CARTON_ID vừa truyền vào)
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
   AND MFSU.CARTON_ID   = {req.CartonId}
").ToListAsync();

                // 3) Nhúng status của proc vào từng item
                var statusText = rtnCode switch
                {
                    "C" => "Complete",
                    "E" => "Error",
                    _ => "Unknown"
                };

                if (data.Count == 0)
                {
                    // Nếu không có dòng nào sau proc, vẫn trả về 1 item “placeholder” để UI hiển thị được status
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

        #endregion
    }
}
