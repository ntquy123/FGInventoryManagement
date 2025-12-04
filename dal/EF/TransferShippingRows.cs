using System;

namespace erpsolution.dal.EF
{
    public class TransferShippingHeaderRow
    {
        public string Invno { get; set; }
        public string InvoiceNo { get; set; }
        public string Cstshtno { get; set; }
        public string ToWhcode { get; set; }
        public string Status { get; set; }
        public string JobNo { get; set; }
    }

    public class TransferShippingLineRow
    {
        public string Shppkg { get; set; }
        public int? LineNo { get; set; }
        public string Aono { get; set; }
        public string Stlcd { get; set; }
        public string Stlsiz { get; set; }
        public string Stlcosn { get; set; }
        public string Stlrevn { get; set; }
        public decimal? ReleaseQty { get; set; }
        public decimal? PickQty { get; set; }
        public decimal? ShipQty { get; set; }
        public string Status { get; set; }
    }

    public class TransferShippingScanRequest
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
