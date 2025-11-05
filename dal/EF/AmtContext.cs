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
 
        public virtual DbSet<UserMenuRoleView> UserMenuRoleView { get; set; }
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
            modelBuilder.Entity<UserMenuInfo>().HasNoKey();


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
