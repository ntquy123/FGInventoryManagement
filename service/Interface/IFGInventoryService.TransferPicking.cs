using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<TransferPickingHeaderRow>> GetTransferPickingHeadersAsync(string whCode, string subwhCode);
        Task<List<TransferPickingLineRow>> GetTransferPickingLinesAsync(string invoiceNo);
        Task<(IReadOnlyList<TransferPickingLineRow> rows, string rtnCode, string rtnMsg)> ScanTransferPickingAsync(TransferPickingScanRequest request);
    }
}
