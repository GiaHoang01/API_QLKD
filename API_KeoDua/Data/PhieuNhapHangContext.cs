using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class PhieuNhapHangContext:DbContext
    {
        public PhieuNhapHangContext(DbContextOptions<PhieuNhapHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<PhieuNhapHang> tbl_PhieuNhapHang { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuNhapHang>()
                .ToTable("tbl_PhieuNhapHang");
        }
        #endregion
    }
}
