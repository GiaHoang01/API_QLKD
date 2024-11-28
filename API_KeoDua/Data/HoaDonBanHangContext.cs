using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class HoaDonBanHangContext:DbContext
    {
        public HoaDonBanHangContext(DbContextOptions<HoaDonBanHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<HoaDonBanHang> tbl_HoaDonBanHang { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDonBanHang>()
                .ToTable("tbl_HoaDonBanHang");

            modelBuilder.Entity<CT_HoaDonBanHang>()
               .HasKey(c => new { c.MaHoaDon, c.MaHangHoa }); // Khóa chính hợp thành

            modelBuilder.Entity<CT_HoaDonBanHang>()
                .ToTable("tbl_CT_HoaDonBanHang");

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
