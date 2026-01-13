using System;

namespace erpsolution.dal.EF
{
    public class TransferPickingHeaderRow
    {
        public string Invno { get; set; }
        public string InvoiceNo { get; set; }
        public string Cstshtno { get; set; }
        public string Whcode { get; set; }
        public string ToWhcode { get; set; }
        public string Status { get; set; }
        public string JobNo { get; set; }
    }

    public class TransferPickingLineRow
    {
        public string ReqNo { get; set; }
        public int? LineNo { get; set; }
        public string LocCode { get; set; }
        public string Aono { get; set; }
        public string Stlcd { get; set; }
        public string Stlsiz { get; set; }
        public string Stlcosn { get; set; }
        public string Stlrevn { get; set; }
        public decimal? RequestQty { get; set; }
        public decimal? InputPickQty { get; set; }
        public string Status { get; set; }
        public string Attribute2 { get; set; }
    }

    public class TransferPickingScanRequest
    {
        public string WhCode { get; set; }
        public string SubwhCode { get; set; }
        public string? LocCode { get; set; }
        public int TrAction { get; set; }
        public string TrInfo { get; set; }
        public string CartonId { get; set; }
        public string? ContainerNo { get; set; }
        public string? UserId { get; set; }
    }
}
