using System;

namespace erpsolution.dal.EF
{
    public partial class StUserMenuRoleTbl
    {
        public string UserId { get; set; }

        public string Fatoy { get; set; }

        public string Menuid { get; set; }

        public string Role { get; set; }

        public string Description { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Uptid { get; set; }

        public string LinkName { get; set; }
    }
}
