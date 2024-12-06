using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class ChuongTrinhKhuyenMaiContext:DbContext
    {
        public ChuongTrinhKhuyenMaiContext(DbContextOptions<ChuongTrinhKhuyenMaiContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<ChuongTrinhKhuyenMai> tbl_ChuongTrinhKhuyenMai { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChuongTrinhKhuyenMai>()
                .ToTable("tbl_ChuongTrinhKhuyenMai");
        }
        #endregion
    }
}
