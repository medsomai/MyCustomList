using Microsoft.EntityFrameworkCore;
using MyCustomList.Models;

namespace MyCustomList.Data
{
    public class MyCustomListContext : DbContext
    {
        public MyCustomListContext(DbContextOptions<MyCustomListContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<MyProduct> MyProducts => Set<MyProduct>();
    }
}