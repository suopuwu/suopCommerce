using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace suopCommerce.Models
{

    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set;}
        public DbSet<Image> Images { get; set;}

        private static string? connectionString = null;

        private static void GetConnectionString()
        {
            if(connectionString == null)
            {
                const string secretName = "postgres";
                var keyVaultName = "suopCommerceSecrets";
                var kvUri = $"https://{keyVaultName}.vault.azure.net";

                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                connectionString = client.GetSecret(secretName).Value.Value;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //below is the development locally hosted database
                //IConfigurationRoot configuration = new ConfigurationBuilder()
                //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                //.AddJsonFile("appsettings.json")
                //.Build();

                //string connectionString = configuration.GetConnectionString("Postgres") ??
                //    throw new NullReferenceException("Connection string is null");

                GetConnectionString();
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }

    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public double Price { get; set; }

        public string[] Extras { get; set; } = Array.Empty<string>();
        public string[] Tags { get; set; } = Array.Empty<string>();

        public int[] Images { get; set; } = Array.Empty<int>();

    }
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }
}
