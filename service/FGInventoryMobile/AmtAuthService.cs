using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Interface;
using Microsoft.EntityFrameworkCore;
using service.Common.Base;
using System;
using System.Linq;



namespace erpsolution.service.FGInventoryMobile
{
    public class AmtAuthService : ServiceBase<FgRequestRow>, IAmtAuthService
    {
        public AmtAuthService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override string PrimaryKey => throw new NotImplementedException();

        public async Task<UserMenuRoleViewHeader> GetRole(string UserId,string menuNm)
        {
            try
            {
                var data = await (from role in _amtContext.StUserMenuRoleTbl.AsNoTracking()
                                  join menu in _amtContext.StMenuTbl.AsNoTracking() on role.Menuid equals menu.Menuid
                                  join subwh in _amtContext.StSubwhTbl.AsNoTracking() on role.Fatoy equals subwh.SubwhCode into subwhJoin
                                  from subwh in subwhJoin.DefaultIfEmpty()
                                  join factory in _amtContext.StFactoryTbl.AsNoTracking() on subwh.WhCode equals factory.CorporationcdFormal into factoryJoin
                                  from factory in factoryJoin.DefaultIfEmpty()
                                  where role.UserId == UserId && menu.Menunm == menuNm
                                  select new UserMenuRoleView
                                  {
                                      WhCode = subwh != null ? subwh.WhCode : null,
                                      WhName = factory != null ? factory.WmsWhname : null,
                                      SubwhCode = role.Fatoy,
                                      SubwhName = subwh != null ? subwh.SubwhName : null,
                                      LocControl = subwh != null ? subwh.LocControl : null,
                                      MenuNm = menu.Menunm,
                                      UserId = role.UserId,
                                      Role = role.Role
                                  }).ToListAsync();

                var result = data.GroupBy(x => new
                {
                    x.UserId,
                    x.LocControl,
                    x.WhCode,
                    x.WhName,
                    x.MenuNm,
                    x.Role
                }).Select(g => new UserMenuRoleViewHeader
                {
                    UserId = g.Key.UserId,
                    LocControl = g.Key.LocControl,
                    WhCode = g.Key.WhCode,
                    WhName = g.Key.WhName,
                    MenuNm = g.Key.MenuNm,
                    Role = g.Key.Role,
                    LstSubWh = g.Select(x => new UserMenuRoleViewDetail
                    {
                        SubwhCode = x.SubwhCode,
                        SubwhName = x.SubwhName,
                    }).ToList()
                }).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TCMUSMT CheckLoginERP(LoginModel pUser)
        {
            try
            {
                var userERP = _amtContext.TCMUSMT
     .FromSqlInterpolated($@"
        select 
            USERID  as ""UserId"",
            NAME    as ""Name"",
            EPASSWD as ""EPassWd""
        from T_CM_USMT@ERP_LINK
        where USERID = {pUser.Username} AND STATUS = 'OK' ")
     .AsNoTracking()
     .FirstOrDefault();


                if (userERP != null)
                {
                    bool Istrue = lib.HashHelper.IsEqualHashValue512(pUser.Password, userERP.EPassWd);
                    if (Istrue)
                    {
                        return userERP;
                    }
                    else
                        throw new Exception("Incorrect password");

                }
                else
                    throw new Exception("This account was not found.");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #region [=============== MOBILE ================]
        public async Task<List<ZmMasMobileMenuGetModel>> GetMobileMenu(string userId)
        {

            try
            {
                var menuList = await (from role in _amtContext.StUserMenuRoleTbl.AsNoTracking()
                                      join menu in _amtContext.StMenuTbl.AsNoTracking() on role.Menuid equals menu.Menuid
                                      where role.UserId == userId && menu.Useyn == "Y"
                                      select new
                                      {
                                          menu.Menuid,
                                          menu.Menunm
                                      })
                                      .Distinct()
                                      .OrderBy(x => x.Menuid)
                                      .ToListAsync();

                var result = menuList.Select((x, index) => new ZmMasMobileMenuGetModel
                {
                    ParentMenuId = 0,
                    MenuLevel = 1,
                    MenuOrder = index + 1,
                    MenuName = x.Menunm,
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
