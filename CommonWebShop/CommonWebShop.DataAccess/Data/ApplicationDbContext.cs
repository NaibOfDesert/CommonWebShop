using CommonWebShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;


namespace CommonWebShop.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                       


        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Shonen", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Shojo", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Josei", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "Teneno", Author = "Takao", Price = 20, Price10 = 15, Description = "Manga", CategoryId = 1, ImageUrl=""},
                new Product { Id = 2, Title = "Sakeneo", Author = "Sakao", Price = 20, Price10 = 15, Description = "Manga", CategoryId = 2, ImageUrl = "" },
                new Product { Id = 3, Title = "Ono", Author = "Kokao", Price = 20, Price10 = 15, Description = "Manga", CategoryId = 3, ImageUrl = "" }                
                );
        }
    }
}
