using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class ThongTinGiaoHangContext:DbContext
    {
        public ThongTinGiaoHangContext(DbContextOptions<ThongTinGiaoHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<ThongTinGiaoHang> tbl_ThongTinGiaoHang { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThongTinGiaoHang>()
                .ToTable("tbl_ThongTinGiaoHang");
        }
        #endregion
    }
}
