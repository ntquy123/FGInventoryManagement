using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<TransferShippingHeaderRow>> GetTransferShippingHeadersAsync(string whCode, string subwhCode);
        Task<List<TransferShippingLineRow>> GetTransferShippingLinesAsync(string invoiceNo);
        Task<(IReadOnlyList<TransferShippingLineRow> rows, string rtnCode, string rtnMsg)> ScanTransferShippingAsync(TransferShippingScanRequest request);
    }
}
