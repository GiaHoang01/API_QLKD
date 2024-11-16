using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class LoaiKhachHangContext:DbContext
    {
        public LoaiKhachHangContext(DbContextOptions<LoaiKhachHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LoaiKhachHang> tbl_LoaiKhachHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiKhachHang>()
                .ToTable("tbl_LoaiKhachHang");
        }
        #endregion
    }
}
