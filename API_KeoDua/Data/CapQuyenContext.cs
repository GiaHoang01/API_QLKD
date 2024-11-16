using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class CapQuyenContext:DbContext
    {
        public CapQuyenContext(DbContextOptions<CapQuyenContext> options) : base(options)
        {
        }

        #region DBSet
        public DbSet<CapQuyen> tbl_CapQuyen { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CapQuyen>()
                .ToTable("tbl_CapQuyen")
                .HasKey(c => new { c.MaQuyen, c.MaNhomQuyen });
        }
    }
}
