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

        public async Task<UserMenuRoleViewHeader> GetRole(string UserId,string menuNm, string? typeRole)
        {
            try
            {
                var headerquery = _amtContext.StUserMenuRoleTbl.AsNoTracking();
                if (typeRole != null)
                    headerquery = _amtContext.StUserMenuRoleTbl.Where(x => x.Role == typeRole);
                var data = await (from role in headerquery
                                  join menu in _amtContext.StMenuTbl.AsNoTracking() on role.Menuid equals menu.Menuid
                                  join subwh in _amtContext.StSubwhTbl.AsNoTracking() on role.Fatoy equals subwh.SubwhCode into subwhJoin
                                  from subwh in subwhJoin.DefaultIfEmpty()
                                  join factory in _amtContext.StFactoryTbl.AsNoTracking() on subwh.WhCode equals factory.CorporationcdFormal into factoryJoin
                                  from factory in factoryJoin.DefaultIfEmpty()
                                  where role.UserId == UserId 
                                  && menu.Menunm == menuNm
                                  select new UserMenuRoleView
                                  {
                                      WhCode = subwh != null ? subwh.WhCode : null,
                                      WhName = factory != null ? factory.WmsWhname : null,
                                      SubwhCode = role.Fatoy,
                                      SubwhName = subwh != null ? subwh.SubwhName : null,
                                      LocControl = subwh != null ? subwh.LocControl : null,
                                      SubWHPickRule = subwh != null ? subwh.PickRule : null,
                                      MenuNm = menu.Menunm,
                                      UserId = role.UserId,
                                      Role = role.Role
                                  }).ToListAsync();

                var result = data
    .GroupBy(x => new
    {
        x.UserId,
        x.MenuNm,
        x.Role
    })
    .Select(g => new UserMenuRoleViewHeader
    {
        UserId = g.Key.UserId,
       // WhCode = g.FirstOrDefault().WhCode,
        //WhName = g.FirstOrDefault().WhName,
        MenuNm = g.Key.MenuNm,
        Role = g.Key.Role,
       // SubWHPickRule = g.FirstOrDefault().SubWHPickRule,
        LstSubWh = g
            .GroupBy(x => new { x.WhCode, x.WhName, x.SubWHPickRule, x.SubwhCode, x.SubwhName, x.LocControl })
            .Select(dg => new UserMenuRoleViewDetail
            {
                WhCode = dg.Key.WhCode,
                WhName = dg.Key.WhName,
                SubWHPickRule = dg.Key.SubWHPickRule,
                SubwhCode = dg.Key.SubwhCode,
                SubwhName = dg.Key.SubwhName,
                LocControl = dg.Key.LocControl
            })
            .ToList()
    })
    .FirstOrDefault();


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
                var result = await (from mobile in _amtContext.StMobileMenuTbl.AsNoTracking()
                                    join menu in _amtContext.StMenuTbl.AsNoTracking() on mobile.LinkMenu equals menu.Menunm
                                    join role in _amtContext.StUserMenuRoleTbl.AsNoTracking() on menu.Menuid equals role.Menuid
                                    where role.UserId == userId && mobile.SysCode == "PKFGM"
                                    orderby mobile.MenuSort
                                    select new ZmMasMobileMenuGetModel
                                    {
                                        ParentMenuId = 0,
                                        MenuLevel = 1,
                                        MenuOrder = mobile.MenuSort ?? 0,
                                        MenuName = mobile.MenuName,
                                        MenuCd = mobile.MenuCode,
                                        ProgramCd = mobile.LinkMenu
                                    }).Distinct().ToListAsync();

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
