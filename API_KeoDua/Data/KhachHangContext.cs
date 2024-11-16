using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class KhachHangContext:DbContext
    {
        public KhachHangContext(DbContextOptions<KhachHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<KhachHang> tbl_KhachHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KhachHang>()
                .ToTable("tbl_KhachHang");
        }
        #endregion
    }
}
