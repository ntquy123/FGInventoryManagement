using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.EF
{
    public class UccStockRow
    {
        public string FrWhCode { get; set; }       // MFSD.WH_CODE
        public string FrSubwhCode { get; set; }    // MFSD.SUBWH_CODE
        public string SubwhName { get; set; }      // subquery tên kho con
        public string LocCode { get; set; }        // MFSD.LOC_CODE
        public string ByrCd { get; set; }          // SUBSTR(MULL.AONO,4,3)
        public string Aono { get; set; }           // MULL.AONO
        public string Stlcd { get; set; }
        //public string Stlnm { get; set; }          // ASMT.STLNM
        public string Stlsiz { get; set; }
        public string Stlcosn { get; set; }
       // public string Stlclrway { get; set; }      // ASMT.STLCLRWAY
        public string Stlrevn { get; set; }
        public decimal? TrQty { get; set; }        // MULL.TOTAL_QTY
        public string CartonId { get; set; }
    }
    public class LocTransferRequest
    {

        public string WhCode { get; set; }
        public string SubwhCode { get; set; }
        public string LocCode { get; set; }
        public string CartonId { get; set; }
        public string? UserId { get; set; }
    }
    public class LabelChangeRequest
    {
        public string WhCode { get; set; }
        public string SubwhCode { get; set; }
        public string FromCartonId { get; set; }   // P_FR_CARTON_ID
        public string ToCartonId { get; set; }     // P_TO_CARTON_ID
        public string UserId { get; set; }         // có thể null
    }
}
