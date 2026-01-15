using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class FgRequestRowView
    {
        public string ReqNo { get; set; }
        public string JobNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public string FrSubwh { get; set; }
        public string FrSubwhName { get; set; }
        public string JobControl { get; set; }
        public string Status { get; set; }    
        public string CrtId { get; set; }
        public string Remark { get; set; }
    }

}
