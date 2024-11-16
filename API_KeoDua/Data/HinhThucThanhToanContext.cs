using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class HinhThucThanhToanContext:DbContext
    {
        public HinhThucThanhToanContext(DbContextOptions<HinhThucThanhToanContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<HinhThucThanhToan> tbl_HinhThucThanhToan{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HinhThucThanhToan>()
                .ToTable("tbl_HinhThucThanhToan");
        }
        #endregion
    }
}
