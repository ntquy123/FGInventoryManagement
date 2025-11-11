

using erpsolution.dal.DTO;
using erpsolution.entities;
using service.Common.Base.Interface;

namespace erpsolution.service.Interface
{
    public interface IAmtAuthService  
    {
        new string PrimaryKey { get; }
        Task<UserMenuRoleViewHeader> GetRole(string UserId, string menuNm);
        TCMUSMT CheckLoginERP(LoginModel pUser);
        Task<List<ZmMasMobileMenuGetModel>> GetMobileMenu(string userId);
    }
}
