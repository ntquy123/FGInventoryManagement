using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<TransferShippingHeaderRow>> GetTransferShippingHeadersAsync(string invoiceNo);
        Task<List<TransferShippingLineRow>> GetTransferShippingLinesAsync(string invoiceNo);
        Task<(IReadOnlyList<TransferShippingLineRow> rows, string rtnCode, string rtnMsg)> ScanTransferShippingAsync(TransferShippingScanRequest request);
    }
}
