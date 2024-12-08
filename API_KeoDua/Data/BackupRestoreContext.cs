using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Data
{
    public class BackupRestoreContext : DbContext
    {
        public BackupRestoreContext(DbContextOptions<BackupRestoreContext> options) : base(options)
        {
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(Database.GetConnectionString());
        }
    }
}
