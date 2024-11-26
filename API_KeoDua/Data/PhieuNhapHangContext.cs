using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class PhieuNhapHangContext : DbContext
    {
        public PhieuNhapHangContext(DbContextOptions<PhieuNhapHangContext> options) : base(options) { }

        #region DBSet
        public DbSet<PhieuNhapHang> tbl_PhieuNhapHang { get; set; }
        public DbSet<CT_PhieuNhap> tbl_CT_PhieuNhap { get; set; }
        #endregion

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình cho bảng PhieuNhapHang
            modelBuilder.Entity<PhieuNhapHang>()
                .ToTable("tbl_PhieuNhapHang");

            // Cấu hình cho bảng CT_PhieuNhap
            modelBuilder.Entity<CT_PhieuNhap>()
                .HasKey(c => new { c.MaPhieuNhap, c.MaHangHoa }); // Khóa chính hợp thành

            modelBuilder.Entity<CT_PhieuNhap>()
                .ToTable("tbl_CT_PhieuNhap");



            base.OnModelCreating(modelBuilder);
        }
    }
}
