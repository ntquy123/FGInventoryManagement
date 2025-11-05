using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace erpsolution.dal.EF
{
    public partial class AmtContext : DbContext
    {
        public AmtContext()
        {
        }

        public AmtContext(DbContextOptions<AmtContext> options) : base(options)
        {
        }
 
        public virtual DbSet<AoStlmstTbl> AoStlmstTbl { get; set; }

        public virtual DbSet<MtUccList> MtUccList { get; set; }

        public virtual DbSet<MtFgStockUcc> MtFgStockUcc { get; set; }
       // public virtual DbSet<UserMenuRoleView> UserMenuRoleView { get; set; }
      //  public virtual DbSet<UserMenuInfo> UserMenuInfo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AoStlmstTbl>(entity =>
            {
                entity.ToTable("AO_STLMST_TBL");

                entity.HasKey(e => new { e.Stlpkg, e.Stlcd, e.Stlsiz, e.Stlcosn, e.Stlrevn });

                entity.Property(e => e.Stlpkg)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("STLPKG");

                entity.Property(e => e.Stlcd)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("STLCD");

                entity.Property(e => e.Stlsiz)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLSIZ");

                entity.Property(e => e.Stlcosn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLCOSN");

                entity.Property(e => e.Stlrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLREVN");

                entity.Property(e => e.Stlnm)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("STLNM");

                entity.Property(e => e.Stlbyr)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STLBYR");

                entity.Property(e => e.Stlbyrcol)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("STLBYRCOL");

                entity.Property(e => e.Byrcd)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("BYRCD");

                entity.Property(e => e.Byrnm)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BYRNM");

                entity.Property(e => e.Status)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Curncy)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("CURNCY");

                entity.Property(e => e.Mstgrp)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MSTGRP");

                entity.Property(e => e.Subgrp)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SUBGRP");

                entity.Property(e => e.Stlsunt)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("STLSUNT");

                entity.Property(e => e.Stlslen)
                    .HasColumnType("NUMBER(6,2)")
                    .HasColumnName("STLSLEN");

                entity.Property(e => e.Stlswth)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("STLSWTH");

                entity.Property(e => e.Stlshgt)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("STLSHGT");

                entity.Property(e => e.Stlwunt)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("STLWUNT");

                entity.Property(e => e.Stlwght)
                    .HasColumnType("NUMBER(28,8)")
                    .HasColumnName("STLWGHT");

                entity.Property(e => e.Pakunt)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("PAKUNT");

                entity.Property(e => e.Paklen)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("PAKLEN");

                entity.Property(e => e.Pakwth)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("PAKWTH");

                entity.Property(e => e.Pakhgt)
                    .HasColumnType("NUMBER(5,2)")
                    .HasColumnName("PAKHGT");

                entity.Property(e => e.Pkgunt)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("PKGUNT");

                entity.Property(e => e.Pkgqty)
                    .HasColumnType("NUMBER(15,3)")
                    .HasColumnName("PKGQTY");

                entity.Property(e => e.Stdpri)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("STDPRI");

                entity.Property(e => e.Chdgrp)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CHDGRP");

                entity.Property(e => e.Stlvunt)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("STLVUNT");

                entity.Property(e => e.Trtcost)
                    .HasColumnType("NUMBER(15,3)")
                    .HasColumnName("TRTCOST");

                entity.Property(e => e.Mrocost)
                    .HasColumnType("NUMBER(15,3)")
                    .HasColumnName("MROCOST");

                entity.Property(e => e.Remrk)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("REMRK");

                entity.Property(e => e.Stlpic)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("STLPIC");

                entity.Property(e => e.Regid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("REGID");

                entity.Property(e => e.Regdat)
                    .HasColumnType("DATE")
                    .HasColumnName("REGDAT");

                entity.Property(e => e.Conid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CONID");

                entity.Property(e => e.Condat)
                    .HasColumnType("DATE")
                    .HasColumnName("CONDAT");

                entity.Property(e => e.Updtor)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("UPDTOR");

                entity.Property(e => e.Upddate)
                    .HasColumnType("DATE")
                    .HasColumnName("UPDDATE");

                entity.Property(e => e.Bomrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("BOMREVN");

                entity.Property(e => e.Ptnrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("PTNREVN");

                entity.Property(e => e.Opsrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("OPSREVN");

                entity.Property(e => e.Oprvno)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("OPRVNO");

                entity.Property(e => e.Designer)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DESIGNER");

                entity.Property(e => e.Itemmanager)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("ITEMMANAGER");

                entity.Property(e => e.Qtyassumer)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("QTYASSUMER");

                entity.Property(e => e.Opplanner)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("OPPLANNER");

                entity.Property(e => e.Technician)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("TECHNICIAN");

                entity.Property(e => e.Itemdman)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("ITEMDMAN");

                entity.Property(e => e.Stlclrway)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STLCLRWAY");

                entity.Property(e => e.Stlclr)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLCLR");

                entity.Property(e => e.Seasoncode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SEASONCODE");

                entity.Property(e => e.Rmid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("RMID");

                entity.Property(e => e.Rmdat)
                    .HasColumnType("DATE")
                    .HasColumnName("RMDAT");

                entity.Property(e => e.Stlbyrnm)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("STLBYRNM");

                entity.Property(e => e.Devteam)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DEVTEAM");

                entity.Property(e => e.Kind)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("KIND");

                entity.Property(e => e.Year)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("YEAR");

                entity.Property(e => e.Upc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPC");

                entity.Property(e => e.Sku)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SKU");

                entity.Property(e => e.Ean)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EAN");

                entity.Property(e => e.Jan)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("JAN");

                entity.Property(e => e.Hscode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HSCODE");
            });

            modelBuilder.Entity<MtUccList>(entity =>
            {
                entity.ToTable("MT_UCC_LIST");

                entity.HasKey(e => new { e.CartonId, e.Byrcd, e.LabelType });

                entity.Property(e => e.CartonId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CARTON_ID");

                entity.Property(e => e.Byrcd)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("BYRCD");

                entity.Property(e => e.LabelType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LABEL_TYPE");

                entity.Property(e => e.Aono)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("AONO");

                entity.Property(e => e.Stlcd)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("STLCD");

                entity.Property(e => e.Stlsiz)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLSIZ");

                entity.Property(e => e.Stlcosn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLCOSN");

                entity.Property(e => e.Stlrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLREVN");

                entity.Property(e => e.ByrPono)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BYR_PONO");

                entity.Property(e => e.ByrStlcd)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BYR_STLCD");

                entity.Property(e => e.ByrStlname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BYR_STLNAME");

                entity.Property(e => e.ByrStlclr)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BYR_STLCLR");

                entity.Property(e => e.ByrStlclrway)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BYR_STLCLRWAY");

                entity.Property(e => e.ProdDate)
                    .HasColumnType("DATE")
                    .HasColumnName("PROD_DATE");

                entity.Property(e => e.TotalQty)
                    .HasColumnType("NUMBER")
                    .HasColumnName("TOTAL_QTY");

                entity.Property(e => e.QtyPerCtn)
                    .HasColumnType("NUMBER")
                    .HasColumnName("QTY_PER_CTN");

                entity.Property(e => e.CtnUnit)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("CTN_UNIT");

                entity.Property(e => e.CtnQty)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_QTY");

                entity.Property(e => e.CtnNo)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_NO");

                entity.Property(e => e.CtnSizUnit)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("CTN_SIZ_UNIT");

                entity.Property(e => e.CtnLen)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_LEN");

                entity.Property(e => e.CtnWid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_WID");

                entity.Property(e => e.CtnHgt)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_HGT");

                entity.Property(e => e.CtnCbm)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_CBM");

                entity.Property(e => e.CtnWtUnit)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasColumnName("CTN_WT_UNIT");

                entity.Property(e => e.CtnNw)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_NW");

                entity.Property(e => e.CtnGw)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CTN_GW");

                entity.Property(e => e.MixedFlag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("MIXED_FLAG");

                entity.Property(e => e.Status)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.UsedFlag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("USED_FLAG");

                entity.Property(e => e.StockFlag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STOCK_FLAG");

                entity.Property(e => e.SubwhCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUBWH_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.PrtGroup)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("PRT_GROUP");

                entity.Property(e => e.Crtdat)
                    .HasColumnType("DATE")
                    .HasColumnName("CRTDAT");

                entity.Property(e => e.Crtid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("CRTID");

                entity.Property(e => e.Uptdat)
                    .HasColumnType("DATE")
                    .HasColumnName("UPTDAT");

                entity.Property(e => e.Uptid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("UPTID");

                entity.Property(e => e.WhCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("WH_CODE");

                entity.Property(e => e.ToCartonId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("TO_CARTON_ID");

                entity.Property(e => e.Dest)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("DEST");
            });

            modelBuilder.Entity<MtFgStockUcc>(entity =>
            {
                entity.ToTable("MT_FG_STOCK_UCC");

                entity.HasKey(e => new { e.WhCode, e.SubwhCode, e.LocCode, e.Aono, e.Stlcd, e.Stlsiz, e.Stlcosn, e.Stlrevn, e.CartonId });

                entity.Property(e => e.WhCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("WH_CODE");

                entity.Property(e => e.SubwhCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUBWH_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.Byrcd)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("BYRCD");

                entity.Property(e => e.Aono)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("AONO");

                entity.Property(e => e.Stlcd)
                    .HasMaxLength(7)
                    .IsUnicode(false)
                    .HasColumnName("STLCD");

                entity.Property(e => e.Stlsiz)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLSIZ");

                entity.Property(e => e.Stlcosn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLCOSN");

                entity.Property(e => e.Stlrevn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("STLREVN");

                entity.Property(e => e.CartonId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CARTON_ID");

                entity.Property(e => e.StockQty)
                    .HasColumnType("NUMBER")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.Unitpri)
                    .HasColumnType("NUMBER")
                    .HasColumnName("UNITPRI");

                entity.Property(e => e.ReserveQty)
                    .HasColumnType("NUMBER")
                    .HasColumnName("RESERVE_QTY");

                entity.Property(e => e.Crtdat)
                    .HasColumnType("DATE")
                    .HasColumnName("CRTDAT");

                entity.Property(e => e.Crtid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("CRTID");

                entity.Property(e => e.Uptdat)
                    .HasColumnType("DATE")
                    .HasColumnName("UPTDAT");

                entity.Property(e => e.Uptid)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("UPTID");
            });

           // modelBuilder.Entity<UserMenuRoleView>().HasNoKey();
            //modelBuilder.Entity<UserMenuInfo>().HasNoKey();


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
