using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace erpsolution.dal.DTO
{
    public class Pccount
    {
        [Column("PC_NAME")]
        public string? Pc_name { get; set; }

        [Column("CRTDAT")]
        public DateTime? Crtdat { get; set; }

        [Column("FR_LOC")]
        public string? Fr_loc { get; set; }

        [Column("TO_LOC")]
        public string? To_loc { get; set; }

        // Giá trị lấy từ subquery (C_CODE)
        [Column("STATUS")]
        public string? Status { get; set; }
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
