using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class ChiTietGioHangConText:DbContext
    {
        public ChiTietGioHangConText(DbContextOptions<ChiTietGioHangConText> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<ChiTietGioHang> tbl_ChiTietGioHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chỉ định khóa chính hợp thành
            modelBuilder.Entity<ChiTietGioHang>()
                .HasKey(c => new { c.MaGioHang, c.MaHangHoa });

            modelBuilder.Entity<ChiTietGioHang>()
                .ToTable("tbl_ChiTietGioHang");
        }
        #endregion
    }
}
