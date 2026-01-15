using System;

namespace erpsolution.dal.EF
{
    public partial class StMenuTbl
    {
        public string Menuid { get; set; }

        public string Lmnuid { get; set; }

        public string Mmnuid { get; set; }

        public string Smnuid { get; set; }

        public string Menunm { get; set; }

        public string Mndesc { get; set; }

        public string Mnurl { get; set; }

        public string Imgurl { get; set; }

        public string Mndpth { get; set; }

        public decimal? Mnsort { get; set; }

        public string Useyn { get; set; }

        public string Crtid { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Uptid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Dmnuid { get; set; }
    }
}
