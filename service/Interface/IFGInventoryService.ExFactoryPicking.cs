using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<ExFactoryPickingHeaderRow>> GetExFactoryPickingHeadersAsync(string whCode);
        Task<List<ExFactoryPickingLineRow>> GetExFactoryPickingLinesAsync(string shpPkg);
        Task<(IReadOnlyList<ExFactoryPickingLineRow> rows, string rtnCode, string rtnMsg)> ScanExFactoryPickingAsync(ExFactoryPickingScanRequest request);
    }
}
