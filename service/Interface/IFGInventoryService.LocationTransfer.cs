using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<UccStockRow>> GetUccByCartonAsync(string cartonId);
        Task<(string rtnCode, string rtnMsg)> ScanQRtoTransferLocation(LocTransferRequest param);
    }
}
