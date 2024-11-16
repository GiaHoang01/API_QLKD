using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class GioHangContext:DbContext
    {
        public GioHangContext(DbContextOptions<GioHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<GioHang> tbl_GioHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GioHang>()
                .ToTable("tbl_GioHang");
        }
        #endregion
    }
}
