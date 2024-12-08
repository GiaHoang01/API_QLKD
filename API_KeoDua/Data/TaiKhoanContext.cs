using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class TaiKhoanContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public TaiKhoanContext(DbContextOptions<TaiKhoanContext> options, IConnectionManager connectionManager) : base(options)
        {
            _connectionManager = connectionManager;
        }
        #region DBSet
        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public IDbConnection CreateConnection()
        {
            // Lấy chuỗi kết nối từ _connectionManager (một đối tượng của IConnectionManager)
            if (string.IsNullOrEmpty(_connectionManager.ConnectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }

            return new SqlConnection(_connectionManager.ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaiKhoan>()
                .ToTable("tbl_TaiKhoan");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _connectionManager.ConnectionString != null)
            {
                optionsBuilder.UseSqlServer(_connectionManager.ConnectionString);
            }
        }
        #endregion
    }
}
