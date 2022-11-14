using BooksGalore.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksGalore.Db
{
    public class Dbcontext :DbContext
    {
        public Dbcontext(DbContextOptions<Dbcontext> options):base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }    
    }
}
