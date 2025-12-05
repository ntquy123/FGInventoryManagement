using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<(string rtnCode, string rtnMsg)> ScanQRtoLabelChange(LabelChangeRequest param);
        Task<List<UccListDetailDto>> ScanQRtoChangeLabelForCarton(string cartonId);
        Task<List<BuyerLabelDto>> ScanQRtoChangeLabelForBuyer(string cartonId);
    }
}
