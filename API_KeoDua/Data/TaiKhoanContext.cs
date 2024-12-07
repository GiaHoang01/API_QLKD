using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class TaiKhoanContext:DbContext
    {
        public TaiKhoanContext(DbContextOptions<TaiKhoanContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaiKhoan>()
                .ToTable("tbl_TaiKhoan");
        }
        #endregion
    }
}
