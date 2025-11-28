using System;

namespace erpsolution.dal.EF
{
    public class ExFactoryShippingHeaderRow
    {
        public string? Shppkg { get; set; }
        public string? InvoiceNo { get; set; }
        public string? Dest { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string? Status { get; set; }
        public string? JobNo { get; set; }
        public string? Remark { get; set; }
    }

    public class ExFactoryShippingLineRow
    {
        public string? Shppkg { get; set; }
        public int? LineNo { get; set; }
        public string? Aono { get; set; }
        public string? Stlcd { get; set; }
        public string? Stlsiz { get; set; }
        public string? Stlcosn { get; set; }
        public string? Stlrevn { get; set; }
        public decimal? ReleaseQty { get; set; }
        public decimal? PickQty { get; set; }
        public string? Status { get; set; }
    }

    public class ExFactoryShippingScanRequest
    {
        public string? WhCode { get; set; }
        public string? SubwhCode { get; set; }
        public string? LocCode { get; set; }
        public int TrAction { get; set; }
        public string? TrInfo { get; set; }
        public string? CartonId { get; set; }
        public string? ContainerNo { get; set; }
        public string? UserId { get; set; }
    }
}
