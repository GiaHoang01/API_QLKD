using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class KhachHangContext:DbContext
    {
        public KhachHangContext(DbContextOptions<KhachHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<KhachHang> tbl_KhachHang { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KhachHang>()
                .ToTable("tbl_KhachHang");
        }
        #endregion
    }
}
