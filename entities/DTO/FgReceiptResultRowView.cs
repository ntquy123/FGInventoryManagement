using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class FgReceiptResultRowView
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
    public class ParamScanQR
    {
        public string whCode { get; set; }
        public string subwhCode { get; set; }
        public string locCode { get; set; }
        public string referInfo { get; set; }
        public string cartonId { get; set; }
        public string userId { get; set; }
    }
   
}
