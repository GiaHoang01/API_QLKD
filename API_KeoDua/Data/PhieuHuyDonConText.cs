using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class PhieuHuyDonConText:DbContext
    {
        public PhieuHuyDonConText(DbContextOptions<PhieuHuyDonConText> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<PhieuHuyDon> tbl_PhieuHuyDon { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuHuyDon>()
                .ToTable("tbl_PhieuHuyDon");
        }
        #endregion
    }
}
