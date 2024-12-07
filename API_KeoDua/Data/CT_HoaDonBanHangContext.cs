using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class CT_HoaDonBanHangContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public CT_HoaDonBanHangContext(DbContextOptions<CT_HoaDonBanHangContext> options, IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }
        #region DBSet
        public DbSet<CT_HoaDonBanHang> tbl_CT_HoaDonBanHang { get; set; }
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
            modelBuilder.Entity<CT_HoaDonBanHang>()
                .HasKey(c => new { c.MaHoaDon, c.MaHangHoa });
            modelBuilder.Entity<CT_HoaDonBanHang>()
                .ToTable("tbl_CT_HoaDonBanHang");

        }
        #endregion
    }
}
