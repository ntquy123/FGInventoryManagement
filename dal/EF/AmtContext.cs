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
