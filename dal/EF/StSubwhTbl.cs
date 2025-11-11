using System;

namespace erpsolution.dal.EF
{
    public partial class StSubwhTbl
    {
        public string WhCode { get; set; }

        public string SubwhCode { get; set; }

        public string SubwhName { get; set; }

        public string SubwhType { get; set; }

        public string LocControl { get; set; }

        public string LocDefault { get; set; }

        public string UsedFlag { get; set; }

        public string Remark { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Uptid { get; set; }

        public string JobControl { get; set; }

        public string PickRule { get; set; }
    }
}
