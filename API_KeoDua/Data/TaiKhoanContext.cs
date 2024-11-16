using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class TaiKhoanContext:DbContext
    {
        public TaiKhoanContext(DbContextOptions<TaiKhoanContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<TaiKhoan> TaiKhoan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaiKhoan>()
                .ToTable("tbl_TaiKhoan");
        }
        #endregion
    }
}
