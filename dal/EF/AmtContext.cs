using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace dal.EF
{
    public partial class AmtContext : DbContext
    {
        public AmtContext()
        {
        }

        public AmtContext(DbContextOptions<AmtContext> options) : base(options)
        {
        }
 
        public virtual DbSet<UserMenuRoleView> UserMenuRoleView { get; set; }
        public virtual DbSet<FgRequestRow> FgRequestRow { get; set; }
        public virtual DbSet<FgRequestDetailRow> FgRequestDetailRow { get; set; }
        public virtual DbSet<FgReceiptResultRow> FgReceiptResultRow { get; set; }
        public virtual DbSet<UccStockRow> UccStockRow { get; set; }
        public virtual DbSet<TCMUSMT> TCMUSMT { get; set; }
        public virtual DbSet<UccListDetailDto> UccListDetailDto { get; set; }
        public virtual DbSet<UccListBoxDto> UccListBoxDto { get; set; }
        public virtual DbSet<StByrmstTbl> StByrmstTbl { get; set; }
        public virtual DbSet<Pccount> Pccount { get; set; }
        public virtual DbSet<Pcinput> Pcinput { get; set; }
        public virtual DbSet<UserMenuInfo> UserMenuInfo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMenuRoleView>().HasNoKey();
            modelBuilder.Entity<FgRequestRow>().HasNoKey();
            modelBuilder.Entity<FgRequestDetailRow>().HasNoKey();
            modelBuilder.Entity<FgReceiptResultRow>().HasNoKey();
            modelBuilder.Entity<UccStockRow>().HasNoKey();
            modelBuilder.Entity<TCMUSMT>().HasNoKey();
            modelBuilder.Entity<UccListDetailDto>().HasNoKey();
            modelBuilder.Entity<UccListBoxDto>().HasNoKey();
            modelBuilder.Entity<StByrmstTbl>().HasNoKey();
            modelBuilder.Entity<Pccount>().HasNoKey();
            modelBuilder.Entity<Pcinput>().HasNoKey();
            modelBuilder.Entity<UserMenuInfo>().HasNoKey();


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
