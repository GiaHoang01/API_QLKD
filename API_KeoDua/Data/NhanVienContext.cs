using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class NhanVienContext:DbContext
    {
        public NhanVienContext(DbContextOptions<NhanVienContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<NhanVien> tbl_NhanVien { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhanVien>()
                .ToTable("tbl_NhanVien");
        }
        #endregion
    }
}
