using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class ChiTietCT_KhuyenMaiContext:DbContext
    {
        public ChiTietCT_KhuyenMaiContext(DbContextOptions<ChiTietCT_KhuyenMaiContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<ChiTietCT_KhuyenMai> tbl_ChiTietCT_KhuyenMai { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sử dụng HasKey để chỉ định khóa chính hợp thành
            modelBuilder.Entity<ChiTietCT_KhuyenMai>()
                .HasKey(c => new { c.MaKhuyenMai, c.MaHangHoa, c.NgayBD });

            modelBuilder.Entity<ChiTietCT_KhuyenMai>()
                .ToTable("tbl_ChiTietCT_KhuyenMai");
        }
        #endregion
    }
}
