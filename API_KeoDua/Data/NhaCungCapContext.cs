﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_KeoDua.Data
{
    public class NhaCungCapContext:DbContext
    {
        public NhaCungCapContext(DbContextOptions<NhaCungCapContext> options) : base(options)
        {

        }
        #region DBSet
        public DbSet<NhaCungCap> tbl_NhaCungCap { get; set; }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhaCungCap>()
                .ToTable("tbl_NhaCungCap");
        }
        #endregion
    }
}
