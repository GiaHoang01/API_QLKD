using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class PhieuHuyDonConText:DbContext
    {
        public PhieuHuyDonConText(DbContextOptions<PhieuHuyDonConText> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<PhieuHuyDon> tbl_PhieuHuyDon { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuHuyDon>()
                .ToTable("tbl_PhieuHuyDon");
        }
        #endregion
    }
}
