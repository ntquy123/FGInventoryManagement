using System;

namespace erpsolution.dal.EF
{
    public partial class MtFgStockUcc
    {
        public string WhCode { get; set; }

        public string SubwhCode { get; set; }

        public string LocCode { get; set; }

        public string Byrcd { get; set; }

        public string Aono { get; set; }

        public string Stlcd { get; set; }

        public string Stlsiz { get; set; }

        public string Stlcosn { get; set; }

        public string Stlrevn { get; set; }

        public string CartonId { get; set; }

        public decimal? StockQty { get; set; }

        public decimal? Unitpri { get; set; }

        public decimal? ReserveQty { get; set; }

        public DateTime? Crtdat { get; set; }

        public string Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string Uptid { get; set; }
    }
}

