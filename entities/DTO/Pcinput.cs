using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class Pcinput
    {
        [Column("WH_CODE")]
        public string? Wh_code { get; set; }

        [Column("FR_SUBWH_CODE")]
        public string? Fr_subwh_code { get; set; }

        [Column("SUBWH_NAME")]
        public string? Subwh_name { get; set; }

        [Column("LOC_CODE")]
        public string? Loc_code { get; set; }

        [Column("BYRCD")]
        public string? Byrcd { get; set; }

        [Column("AONO")]
        public string? Aono { get; set; }

        [Column("STLCD")]
        public string? Stlcd { get; set; }

        [Column("STLNM")]
        public string? Stlnm { get; set; }

        [Column("STLSIZ")]
        public string? Stlsiz { get; set; }

        [Column("STLCOSN")]
        public string? Stlcosn { get; set; }

        [Column("STLCLRWAY")]
        public string? Stlclrway { get; set; }

        [Column("STLREVN")]
        public string? Stlrevn { get; set; }

        [Column("QTY")]
        public decimal? Qty { get; set; }

        [Column("TOTAL_QTY")]
        public decimal? Total_qty { get; set; }

        [Column("CARTON_ID")]
        public string? Carton_id { get; set; }
        [NotMapped]
        public string? Statuscode { get; set; }    // C / E / ...
        [NotMapped]
        public string Status { get; set; } = "Unknown"; // C: Complete, E: Error
        [NotMapped]
        public string? Errormsg { get; set; }     
    }
    public class Pcinputrequest
    {
        public string WhCode { get; set; } = default!;
        public string SubwhCode { get; set; } = default!;
        public string PcName { get; set; } = default!;
        public string LocCode { get; set; } = default!;
        /// <summary>Input: 1, Cancel: -1</summary>
        public int TrAction { get; set; }
        public string CartonId { get; set; } = default!;
        public string UserId { get; set; } = default!;
    }
 


}
