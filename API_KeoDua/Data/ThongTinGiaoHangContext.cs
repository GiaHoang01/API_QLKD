using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class ThongTinGiaoHangContext:DbContext
    {
        public ThongTinGiaoHangContext(DbContextOptions<ThongTinGiaoHangContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<ThongTinGiaoHang> tbl_ThongTinGiaoHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThongTinGiaoHang>()
                .ToTable("tbl_ThongTinGiaoHang");
        }
        #endregion
    }
}
