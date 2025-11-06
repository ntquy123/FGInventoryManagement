using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class UserMenuRoleView
    {
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public string SubwhCode { get; set; }
        public string SubwhName { get; set; }
        public string LocControl { get; set; }
        public string MenuNm { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
    public class UserMenuRoleViewHeader
    {
        public string WhCode { get; set; }
        public string WhName { get; set; }
        public string LocControl { get; set; }
        public string MenuNm { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public List<UserMenuRoleViewDetail> LstSubWh { get; set; }
    }
    public class UserMenuRoleViewDetail
    {
        public string SubwhCode { get; set; }
        public string SubwhName { get; set; }

    }
    public class UserMenuInfo
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
    }
}
public class ZmMasMobileMenuGetModel
{
    public decimal MenuId { get; set; }
    public string MenuName { get; set; }
    public decimal ParentMenuId { get; set; }
    public decimal MenuLevel { get; set; }
    public decimal MenuOrder { get; set; }
    public string ProgramCd { get; set; }
    public string UseYn { get; set; }
    public string Description { get; set; }
    public string MenuCd { get; set; }
    public List<ZmMasMobileMenuModel> ListMenu { get; set; }
}