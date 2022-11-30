using BooksGalore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BooksGalore.Db
{
    public class Dbcontext :IdentityDbContext //to install identity through migration

        //directly we can use include properties like .Include("covertype") in case of dbcontext. But in Repository  it is not possible.
    {
       //like db.dbsetname.Include("Covertype")
        public Dbcontext(DbContextOptions<Dbcontext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
       base.OnModelCreating(builder);
       // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }  
        public DbSet<Company> Companies { get; set; }   
        public DbSet<ApplicationUser> ApplicationUsers {  get; set; } //add discrimination to NetUsers & Roles added to applicationuser
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        //include dbset properties for proper migration if dbcontext seperated from main project
    }
}
//codegenration for scaffolding
//efcore.tools for migration
//efcore for accessing Dbcontext
//sqlserver for accessing sqlserver
//Identity for accessing Registration $ Login

//For .net version 7 sqlserver checkingsomecertificates so we have to set it to false otherwise 
//some error will come. Google it to overcome it

//[validatenever] Just for model validation
//? for DB and model validation. BOTH