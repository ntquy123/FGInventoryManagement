using erpsolution.dal.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<(string rtnCode, string rtnMsg)> ScanQRtoLabelChange(LabelChangeRequest param);
        Task<List<UccListDetailDto>> ScanQRtoChangeLabelForCarton(string cartonId);
        Task<List<UccListBoxDto>> ScanQRtoChangeLabelForBuyer(string cartonId);
    }
}
