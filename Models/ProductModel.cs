using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace suopCommerce.Models
{

    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("Postgres") ??
                throw new NullReferenceException("Connection string is null");

            optionsBuilder.UseNpgsql(connectionString);
            
        }
    }
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? CategoryId { get; set; }

        public double Price { get; set; }
        public string[]? Images { get; set; }
        public string[]? Tags { get; set; }

    }
}
