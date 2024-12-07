using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class CapQuyenContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public CapQuyenContext(DbContextOptions<CapQuyenContext> options,IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }

        #region DBSet
        public DbSet<CapQuyen> tbl_CapQuyen { get; set; }
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
            modelBuilder.Entity<CapQuyen>()
                .ToTable("tbl_CapQuyen")
                .HasKey(c => new { c.MaQuyen, c.MaNhomQuyen });
        }
    }
}
