using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class NhanVienContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public NhanVienContext(DbContextOptions<NhanVienContext> options,IConnectionManager connectionManager) : base(options)
        {
            _connectionManager = connectionManager;
        }
        #region DBSet
        public DbSet<NhanVien> tbl_NhanVien { get; set; }

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
            modelBuilder.Entity<NhanVien>()
                .ToTable("tbl_NhanVien");
        }
        #endregion
    }
}
