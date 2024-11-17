using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class HangHoaContext:DbContext
    {
        public HangHoaContext(DbContextOptions<HangHoaContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<HangHoa> tbl_CapQuyen { get; set; }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HangHoa>()
                .ToTable("tbl_HangHoa");
        }
        #endregion
    }
}
