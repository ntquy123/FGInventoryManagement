using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class OspFileinfoTbl
    {
        public string SubContractNo { get; set; }
        public string TypeValue { get; set; }
        public decimal LineNo { get; set; }
        public string FileName { get; set; }
        public DateTime? CrtDat { get; set; }
        public string CrtId { get; set; }
        public DateTime? UptDat { get; set; }
        public string UptId { get; set; }
    }
}
