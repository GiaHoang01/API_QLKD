using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class PhieuGiaoHangContext:DbContext
    {
        public PhieuGiaoHangContext(DbContextOptions<PhieuGiaoHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<PhieuGiaoHang> tbl_PhieuGiaoHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuGiaoHang>()
                .ToTable("tbl_PhieuGiaoHang");
        }
        #endregion
    }
}
