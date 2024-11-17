using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class LoaiKhachHangContext:DbContext
    {
        public LoaiKhachHangContext(DbContextOptions<LoaiKhachHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LoaiKhachHang> tbl_LoaiKhachHang { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiKhachHang>()
                .ToTable("tbl_LoaiKhachHang");
        }
        #endregion
    }
}
