using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class PhieuNhapHangContext : DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public PhieuNhapHangContext(DbContextOptions<PhieuNhapHangContext> options, IConnectionManager connectionManager) : base(options) {
            this._connectionManager = connectionManager;
        }
        #region DBSet
        public DbSet<PhieuNhapHang> tbl_PhieuNhapHang { get; set; }
        public DbSet<CT_PhieuNhap> tbl_CT_PhieuNhap { get; set; }
        #endregion

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
            // Cấu hình cho bảng PhieuNhapHang
            modelBuilder.Entity<PhieuNhapHang>()
                .ToTable("tbl_PhieuNhapHang");

            // Cấu hình cho bảng CT_PhieuNhap
            modelBuilder.Entity<CT_PhieuNhap>()
                .HasKey(c => new { c.MaPhieuNhap, c.MaHangHoa }); // Khóa chính hợp thành

            modelBuilder.Entity<CT_PhieuNhap>()
                .ToTable("tbl_CT_PhieuNhap");



            base.OnModelCreating(modelBuilder);
        }
    }
}
