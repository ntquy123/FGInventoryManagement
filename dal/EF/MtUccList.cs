using System;

namespace erpsolution.dal.EF
{
    public partial class MtUccList
    {
        public string? CartonId { get; set; }

        public string? Byrcd { get; set; }

        public string? LabelType { get; set; }

        public string? Aono { get; set; }

        public string? Stlcd { get; set; }

        public string? Stlsiz { get; set; }

        public string? Stlcosn { get; set; }

        public string? Stlrevn { get; set; }

        public string? ByrPono { get; set; }

        public string? ByrStlcd { get; set; }

        public string? ByrStlname { get; set; }

        public string? ByrStlclr { get; set; }

        public string? ByrStlclrway { get; set; }

        public DateTime? ProdDate { get; set; }

        public decimal? TotalQty { get; set; }

        public decimal? QtyPerCtn { get; set; }

        public string? CtnUnit { get; set; }

        public decimal? CtnQty { get; set; }

        public decimal? CtnNo { get; set; }

        public string? CtnSizUnit { get; set; }

        public decimal? CtnLen { get; set; }

        public decimal? CtnWid { get; set; }

        public decimal? CtnHgt { get; set; }

        public decimal? CtnCbm { get; set; }

        public string? CtnWtUnit { get; set; }

        public decimal? CtnNw { get; set; }

        public decimal? CtnGw { get; set; }

        public string? MixedFlag { get; set; }

        public string? Status { get; set; }

        public string? UsedFlag { get; set; }

        public string? StockFlag { get; set; }

        public string? SubwhCode { get; set; }

        public string? LocCode { get; set; }

        public string? PrtGroup { get; set; }

        public DateTime? Crtdat { get; set; }

        public string? Crtid { get; set; }

        public DateTime? Uptdat { get; set; }

        public string? Uptid { get; set; }

        public string? WhCode { get; set; }

        public string? ToCartonId { get; set; }

        public string? Dest { get; set; }
    }
}
