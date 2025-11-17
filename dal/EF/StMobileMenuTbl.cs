using System;

namespace erpsolution.dal.EF
{
    public partial class StMobileMenuTbl
    {
        public string SysCode { get; set; }

        public string MenuCode { get; set; }

        public string MenuName { get; set; }

        public decimal? MenuSort { get; set; }

        public string LinkMenu { get; set; }

        public string Crtid { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Uptid { get; set; }

        public DateTime? Uptdat { get; set; }
    }
}
