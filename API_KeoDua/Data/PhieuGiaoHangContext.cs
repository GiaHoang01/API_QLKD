using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class PhieuGiaoHangContext:DbContext
    {
        public PhieuGiaoHangContext(DbContextOptions<PhieuGiaoHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<PhieuGiaoHang> tbl_PhieuGiaoHang { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuGiaoHang>()
                .ToTable("tbl_PhieuGiaoHang");
        }
        #endregion
    }
}
