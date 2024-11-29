using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class CT_HoaDonBanHangContext:DbContext
    {
        public CT_HoaDonBanHangContext(DbContextOptions<CT_HoaDonBanHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<CT_HoaDonBanHang> tbl_CT_HoaDonBanHang { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CT_HoaDonBanHang>()
                .HasKey(c => new { c.MaHoaDon, c.MaHangHoa });
            modelBuilder.Entity<CT_HoaDonBanHang>()
                .ToTable("tbl_CT_HoaDonBanHang");

        }
        #endregion
    }
}
