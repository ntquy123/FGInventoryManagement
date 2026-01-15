using System;

namespace erpsolution.dal.EF
{
    public partial class StTypecodeTbl
    {
        public string CType { get; set; }

        public decimal? CId { get; set; }

        public string CCode { get; set; }

        public string CName { get; set; }

        public decimal? CSeq { get; set; }

        public string UseYn { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Uptid { get; set; }
    }
}
