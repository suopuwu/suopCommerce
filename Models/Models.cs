using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace suopCommerce.Models
{

    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set;}
        public DbSet<Image> Images { get; set;}

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
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CategoryId { get; set; }

        public double Price { get; set; }

        public string[]? Addons { get; set; }
        public string[]? Extras { get; set; }
        public string[]? Images { get; set; }
        public string[]? Tags { get; set; }

    }
    public class Image
    {
        [Key]
        public string Url { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }
}
