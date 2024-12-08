using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class ChiTietCT_KhuyenMaiContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public ChiTietCT_KhuyenMaiContext(DbContextOptions<ChiTietCT_KhuyenMaiContext> options, IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }
        #region DBSet
        public DbSet<ChiTietCT_KhuyenMai> tbl_ChiTietCT_KhuyenMai { get; set; }
        public IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionManager.ConnectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }

            return new SqlConnection(_connectionManager.ConnectionString);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!_connectionManager.ConnectionString.Equals(string.Empty))
            {
                optionsBuilder.UseSqlServer(_connectionManager.ConnectionString);
            }
            else
            {
                throw new InvalidOperationException("Connection string has not been initialized.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sử dụng HasKey để chỉ định khóa chính hợp thành
            modelBuilder.Entity<ChiTietCT_KhuyenMai>()
                .HasKey(c => new { c.MaKhuyenMai, c.MaHangHoa, c.NgayBD });

            modelBuilder.Entity<ChiTietCT_KhuyenMai>()
                .ToTable("tbl_ChiTietCT_KhuyenMai");
        }
        #endregion
    }
}
