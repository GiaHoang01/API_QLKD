using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class HoaDonBanHangContext : DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public HoaDonBanHangContext(DbContextOptions<HoaDonBanHangContext> options, IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }
        #region DBSet
        public DbSet<HoaDonBanHang> tbl_HoaDonBanHang { get; set; }
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
            modelBuilder.Entity<HoaDonBanHang>()
                .ToTable("tbl_HoaDonBanHang");

            modelBuilder.Entity<CT_HoaDonBanHang>()
               .HasKey(c => new { c.MaHoaDon, c.MaHangHoa }); // Khóa chính hợp thành

            modelBuilder.Entity<CT_HoaDonBanHang>()
                .ToTable("tbl_CT_HoaDonBanHang");

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
