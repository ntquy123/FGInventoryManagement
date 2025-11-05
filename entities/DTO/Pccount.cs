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
}
