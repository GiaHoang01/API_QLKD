using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class LoaiHangHoaContext:DbContext
    {
        public LoaiHangHoaContext(DbContextOptions<LoaiHangHoaContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LoaiHangHoa> tbl_LoaiHangHoa { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiHangHoa>()
                .ToTable("tbl_LoaiHangHoa");
        }
        #endregion
    }
}
