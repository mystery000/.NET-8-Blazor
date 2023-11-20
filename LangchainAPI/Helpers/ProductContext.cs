using LangchainAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LangchainAPI.Helpers
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
    }
}
