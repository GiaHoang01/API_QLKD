﻿using API_KeoDua.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class LichSuGiaContext:DbContext
    {
        private readonly IConnectionManager _connectionManager;
        public LichSuGiaContext(DbContextOptions<LichSuGiaContext> options, IConnectionManager _connectionManager) : base(options)
        {
            this._connectionManager = _connectionManager;
        }
        #region DBSet
        public DbSet<LichSuGia> tbl_LichSuGia { get; set; }
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
            modelBuilder.Entity<LichSuGia>()
                .ToTable("tbl_LichSuGia");
        }
        #endregion
    }
}
