using erpsolution.dal.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace erpsolution.service.Interface
{
    public partial interface IFGInventoryService
    {
        Task<List<Pccount>> GetPcCountAsync(string whCode, string subwhCode);
        Task<List<Pcinput>> ExecutePcInputAndQueryAsync(Pcinputrequest req);
    }
}
