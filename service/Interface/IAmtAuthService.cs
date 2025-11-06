

using erpsolution.dal.DTO;
using erpsolution.entities;
using service.Common.Base.Interface;

namespace erpsolution.service.Interface
{
    public interface IAmtAuthService : IServiceBase<FgRequestRow>
    {
        new string PrimaryKey { get; }
        Task<UserMenuRoleViewHeader> GetRole(string UserId, string menuNm);
        TCMUSMT CheckLoginERP(LoginModel pUser);
        bool isHasAccountERP(string Username);
        Task<List<ZmMasMobileMenuGetModel>> GetMobileMenu(string userId);
    }
}
