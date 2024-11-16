using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class HoaDonBanHangContext:DbContext
    {
        public HoaDonBanHangContext(DbContextOptions<HoaDonBanHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<HoaDonBanHang> tbl_HoaDonBanHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDonBanHang>()
                .ToTable("tbl_HoaDonBanHang");
        }
        #endregion
    }
}
