using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class FgRequestDetailRow
    {
        public string WhCode { get; set; }
        public string WarehouseNm { get; set; }
        public string SubwhCode { get; set; }
        public string SubwhName { get; set; }
        public string ReqNo { get; set; }
        public int? LineNo { get; set; }

        public string Aono { get; set; }
        public string Stlcd { get; set; }
        public string Stlsiz { get; set; }
        public string Stlcosn { get; set; }
        public string Stlrevn { get; set; }

        public string Stlnm { get; set; }
        public string Stlclrway { get; set; }

        public decimal? RequestQty { get; set; }
        public decimal? ReceiptQty { get; set; }
        public decimal? CancelQty { get; set; }
        public decimal? RemainQty { get; set; }

        public int? Status { get; set; }
        public string StatusNm { get; set; }

        public string LocControl { get; set; }
    }
}
