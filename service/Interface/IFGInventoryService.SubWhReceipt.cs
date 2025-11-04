using System.Collections.Generic;
using System.Threading.Tasks;

namespace service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<(IReadOnlyList<FgReceiptResultRow> rows, string rtnCode, string rtnMsg)> ScanQrtoSubWHReceipt(ParamScanQR param);
        Task<List<FgRequestRow>> GetHeaderSubWhReceipt(string whCode, string toSubwh);
        Task<List<FgRequestDetailRow>> GetDetailSubWhReceipt(string reqNo, string whCode, string toSubwh);
    }
}
