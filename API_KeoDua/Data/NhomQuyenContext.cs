using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class NhomQuyenContext:DbContext
    {
        public NhomQuyenContext(DbContextOptions<NhomQuyenContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<NhomQuyen> tbl_NhomQuyen { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhomQuyen>()
                .ToTable("tbl_NhomQuyen");
        }
        #endregion
    }
}
