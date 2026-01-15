using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<ExFactoryShippingHeaderRow>> GetExFactoryShippingHeadersAsync(string whCode);
        Task<List<ExFactoryShippingLineRow>> GetExFactoryShippingLinesAsync(string shpPkg);
        Task<(IReadOnlyList<ExFactoryShippingLineRow> rows, string rtnCode, string rtnMsg)> ScanExFactoryShippingAsync(ExFactoryShippingScanRequest request);
    }
}
