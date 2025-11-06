using System;

namespace erpsolution.dal.EF
{
    public partial class MtFgPcount
    {
        public string WhCode { get; set; }

        public string SubwhCode { get; set; }

        public string PcName { get; set; }

        public string FrLoc { get; set; }

        public string ToLoc { get; set; }

        public decimal? Status { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Uptid { get; set; }
    }
}
