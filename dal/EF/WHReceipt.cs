using System;

namespace erpsolution.dal.EF
{
    public class WHReceiptHeaderRow
    {
        public string? Invno { get; set; }
        public string? InvoiceNo { get; set; }
        public string? Cstshtno { get; set; }
        public string? Whcode { get; set; }
        public string? FromWhcode { get; set; }
        public string? Status { get; set; }
        public string? JobNo { get; set; }
    }

    public class WHReceiptLineRow
    {
        public string ReqNo { get; set; }
        public int? LineNo { get; set; }
        public string Aono { get; set; }
        public string Stlcd { get; set; }
        public string Stlsiz { get; set; }
        public string Stlcosn { get; set; }
        public string Stlrevn { get; set; }
        public decimal? RequestQty { get; set; }
        public decimal? ReceiptQty { get; set; }
        public string Status { get; set; }
        public string StatusNm { get; set; }
    }

    public class WHReceiptScanRequest
    {
        public string WhCode { get; set; }
        public string SubwhCode { get; set; }
        public string? LocCode { get; set; }
        //public string InvoiceNo { get; set; }
        public string InvNo { get; set; }
        public string CartonId { get; set; }
        public string? UserId { get; set; }
    }
}
