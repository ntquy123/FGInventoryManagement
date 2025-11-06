using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.EF
{
    public class FgReceiptResultRow
    {
        public string WhCode { get; set; }
        public string ReqNo { get; set; }
        public int? LineNo { get; set; }
        public decimal? RemainQty { get; set; }
        public decimal? ReceiptQty { get; set; }
        public int? Status { get; set; }
        public string StatusNm { get; set; }
        public string CartonId { get; set; }
    }
    
   
}
