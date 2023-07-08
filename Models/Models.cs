﻿using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public double Price { get; set; }

        public string[] Extras { get; set; } = new string[0];
        public string[] Tags { get; set; } = new string[0];

        public int[] Addons { get; set; } = new int[0];
        public int[] Images { get; set; } = new int[0];

    }
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }
}
