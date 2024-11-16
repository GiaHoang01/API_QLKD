using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class LichSuGiaContext:DbContext
    {
        public LichSuGiaContext(DbContextOptions<LichSuGiaContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<LichSuGia> tbl_LichSuGia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LichSuGia>()
                .ToTable("tbl_LichSuGia");
        }
        #endregion
    }
}
