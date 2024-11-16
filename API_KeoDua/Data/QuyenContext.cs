using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class QuyenContext:DbContext
    {
        public QuyenContext(DbContextOptions<QuyenContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<Quyen> tbl_Quyen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quyen>()
                .ToTable("tbl_Quyen");
        }
        #endregion
    }
}
