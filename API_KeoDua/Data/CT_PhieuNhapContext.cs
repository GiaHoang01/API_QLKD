using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class CT_PhieuNhapContext:DbContext
    {
        public CT_PhieuNhapContext(DbContextOptions<CT_PhieuNhapContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<CT_PhieuNhap> tbl_CT_PhieuNhap { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chỉ định khóa chính hợp thành
            modelBuilder.Entity<CT_PhieuNhap>()
                .HasKey(c => new { c.MaPhieuNhap, c.MaHangHoa }); // Thay vì sử dụng [Key] trên mỗi thuộc tính

            modelBuilder.Entity<CT_PhieuNhap>()
                .ToTable("tbl_CT_PhieuNhap");
        }
        #endregion
    }
}
