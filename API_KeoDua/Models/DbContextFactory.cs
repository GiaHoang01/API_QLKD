using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Models
{
    public class DbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateDbContext<T>(string connectionString) where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSqlServer(connectionString);

            return (T)Activator.CreateInstance(typeof(T), optionsBuilder.Options);
        }
    }
}
