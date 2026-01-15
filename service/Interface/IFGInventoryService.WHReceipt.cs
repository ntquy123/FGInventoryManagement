using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<WHReceiptHeaderRow>> GetReceiptHeadersAsync(string whCode, string subwhCode);
        Task<List<WHReceiptLineRow>> GetReceiptLinesAsync(string invoiceNo);
        Task<(IReadOnlyList<WHReceiptLineRow> rows, string rtnCode, string rtnMsg)> ScanReceiptAsync(WHReceiptScanRequest request);
    }
}
