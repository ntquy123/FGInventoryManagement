using erpsolution.dal.DTO;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Interface;
using Microsoft.EntityFrameworkCore;
using service.Common.Base;



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
                //var menuNm = "SUBWH RECEIPT";
                var sqlQuery = $@"
                                    SELECT
                                  (SELECT C.WH_CODE
                                     FROM ST_SUBWH_TBL C, ST_FACTORY_TBL D
                                    WHERE C.WH_CODE = D.CORPORATIONCD_FORMAL
                                      AND C.SUBWH_CODE = A.FATOY
                                      AND ROWNUM = 1) AS WhCode,
                                  (SELECT D.WMS_WHNAME
                                     FROM ST_SUBWH_TBL C, ST_FACTORY_TBL D
                                    WHERE C.WH_CODE = D.CORPORATIONCD_FORMAL
                                      AND C.SUBWH_CODE = A.FATOY
                                      AND ROWNUM = 1) AS WhName,
                                  A.FATOY AS SubwhCode,
                                  (SELECT B.SUBWH_NAME FROM ST_SUBWH_TBL B WHERE B.SUBWH_CODE = A.FATOY AND ROWNUM = 1) AS SubwhName,
                                  (SELECT B.LOC_CONTROL FROM ST_SUBWH_TBL B WHERE B.SUBWH_CODE = A.FATOY AND ROWNUM = 1) AS LocControl,
                                  B.MENUNM AS MenuNm,
                                  A.USER_ID AS UserId,
                                  A.ROLE AS Role
                                FROM ST_USER_MENU_ROLE_TBL A, ST_MENU_TBL B
                                WHERE A.MENUID = B.MENUID
                                  AND A.USER_ID = '{UserId}'
                                  AND B.MENUNM = '{menuNm}'
                                 ";
                var data = await _amtContext.UserMenuRoleView.FromSqlRaw(sqlQuery).ToListAsync();
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
                    UserId=g.Key.UserId,
                    LocControl=g.Key.LocControl,
                    WhCode = g.Key.WhCode,
                    WhName = g.Key.WhName,
                    MenuNm = g.Key.MenuNm,
                    Role = g.Key.Role,
                    LstSubWh = g.Select(x=> new UserMenuRoleViewDetail
                    {
                       SubwhCode= x.SubwhCode,
                        SubwhName= x.SubwhName,
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
                // Sử dụng StringBuilder hoặc string literal để tạo query SQL
                var sqlQuery = $@"
                SELECT DISTINCT
                    B.MENUID AS MenuId,
                    B.MENUNM AS MenuName
                FROM 
                    ST_USER_MENU_ROLE_TBL A
                INNER JOIN 
                    ST_MENU_TBL B ON A.MENUID = B.MENUID  
                WHERE 
                    A.USER_ID = '{userId}' 
                    AND B.USEYN = 'Y'
            ";
                var menuList = await _amtContext.UserMenuInfo.FromSqlRaw(sqlQuery)
                                            .Select(x => new
                                            {
                                                x.MenuId,
                                                x.MenuName
                                            })
                                            .ToListAsync(); // Thực thi truy vấn SQL

                var result = menuList.Select((x, index) => new ZmMasMobileMenuGetModel // Sử dụng Select có tham số index
                {
                    //MenuId = x.MenuId,
                    ParentMenuId = 0,
                    MenuLevel = 1,
                    MenuOrder = index + 1, 
                    MenuName = x.MenuName,
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
