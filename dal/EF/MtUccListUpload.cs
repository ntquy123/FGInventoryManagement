using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace erpsolution.dal.EF
{
    
    [Table("MT_UCC_LIST_UPLOAD")]
    public class MtUccListUpload
    {
        [Column("XLS_ID")]
        public string? XlsId { get; set; }

        [Column("XLS_SQ")]
        public int XlsSq { get; set; }

        [Column("WH_CODE")]
        public string? WhCode { get; set; }

        [Column("CARTON_ID")]
        public string? CartonId { get; set; }

        [Column("BYRCD")]
        public string? ByrCd { get; set; }

        [Column("LABEL_TYPE")]
        public string? LabelType { get; set; }

        [Column("AONO")]
        public string? AoNo { get; set; }

        [Column("STLCD")]
        public string? StlCd { get; set; }

        [Column("STLSIZ")]
        public string? StlSiz { get; set; }

        [Column("STLCOSN")]
        public string? StlCosn { get; set; }

        [Column("STLREVN")]
        public string? StlRevn { get; set; }

        [Column("BYR_PONO")]
        public string? ByrPono { get; set; }

        [Column("BYR_STLCD")]
        public string? ByrStlCd { get; set; }

        [Column("BYR_STLNAME")]
        public string? ByrStlName { get; set; }

        [Column("BYR_STLCLR")]
        public string? ByrStlClr { get; set; }

        [Column("BYR_STLCLRWAY")]
        public string? ByrStlClrWay { get; set; }

        [Column("PROD_DATE")]
        public DateTime? ProdDate { get; set; }

        [Column("TOTAL_QTY")]
        public decimal? TotalQty { get; set; }

        [Column("QTY_PER_CTN")]
        public decimal? QtyPerCtn { get; set; }

        [Column("CTN_UNIT")]
        public string? CtnUnit { get; set; }

        [Column("CTN_QTY")]
        public decimal? CtnQty { get; set; }

        [Column("CTN_NO")]
        public decimal? CtnNo { get; set; }

        [Column("CTN_SIZ_UNIT")]
        public string? CtnSizUnit { get; set; }

        [Column("CTN_LEN")]
        public decimal? CtnLen { get; set; }

        [Column("CTN_WID")]
        public decimal? CtnWid { get; set; }

        [Column("CTN_HGT")]
        public decimal? CtnHgt { get; set; }

        [Column("CTN_CBM")]
        public decimal? CtnCbm { get; set; }

        [Column("CTN_WT_UNIT")]
        public string? CtnWtUnit { get; set; }

        [Column("CTN_NW")]
        public decimal? CtnNw { get; set; }

        [Column("CTN_GW")]
        public decimal? CtnGw { get; set; }

        [Column("MIXED_FLAG")]
        public string? MixedFlag { get; set; }

        [Column("STATUS")]
        public string? Status { get; set; }

        [Column("ERR_MSG")]
        public string? ErrMsg { get; set; }

        [Column("CRTDAT")]
        public DateTime? CrtDat { get; set; }

        [Column("CRTID")]
        public string? CrtId { get; set; }

        [Column("UPTDAT")]
        public DateTime? UptDat { get; set; }

        [Column("UPTID")]
        public string? UptId { get; set; }
    }
    public class DataSaveLableUpload
    {
        public string BuyerCd { get; set; }
        public string UserId { get; set; }
        public string WhCode { get; set; }
        public List<string> lstCartonId { get; set;}

    }
}
