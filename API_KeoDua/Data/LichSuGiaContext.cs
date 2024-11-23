using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class LichSuGiaContext:DbContext
    {
        public LichSuGiaContext(DbContextOptions<LichSuGiaContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LichSuGia> tbl_LichSuGia { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LichSuGia>()
                .ToTable("tbl_LichSuGia");
        }
        #endregion
    }
}
