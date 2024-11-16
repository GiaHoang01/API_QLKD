using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class NhaCungCapContext:DbContext
    {
        public NhaCungCapContext(DbContextOptions<NhaCungCapContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LoaiKhachHang> tbl_LoaiKhachHang { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiKhachHang>()
                .ToTable("tbl_LoaiKhachHang");
        }
        #endregion
    }
}
