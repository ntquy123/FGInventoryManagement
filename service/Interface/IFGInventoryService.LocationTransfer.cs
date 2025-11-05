using erpsolution.dal.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<UccStockRow>> GetUccByCartonAsync(string cartonId);
        Task<(string rtnCode, string rtnMsg)> ScanQRtoTransferLocation(LocTransferRequest param);
    }
}
