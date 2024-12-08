using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class CT_PhieuNhapContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public CT_PhieuNhapContext(DbContextOptions<CT_PhieuNhapContext> options, IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }
        #region DBSet
        public DbSet<CT_PhieuNhap> tbl_CT_PhieuNhap { get; set; }
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
            // Chỉ định khóa chính hợp thành
            modelBuilder.Entity<CT_PhieuNhap>()
                .HasKey(c => new { c.MaPhieuNhap, c.MaHangHoa }); // Thay vì sử dụng [Key] trên mỗi thuộc tính

            modelBuilder.Entity<CT_PhieuNhap>()
                .ToTable("tbl_CT_PhieuNhap");
        }
        #endregion
    }
}
