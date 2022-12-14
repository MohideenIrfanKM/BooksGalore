using System.ComponentModel;
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
        //IMPPRTANT here you cann add or define the composite PRIMARY key for a table 
    }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }  
        public DbSet<Company> Companies { get; set; } 
        //before adding ApplicationUser We will be using Default <IdentityUser> in below line.
        public DbSet<ApplicationUser> ApplicationUsers {  get; set; } //add discrimination to NetUsers & Roles added to applicationuser
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        //include dbset properties for proper migration if dbcontext seperated from main project
    }
}
//codegenration for scaffolding
//efcore.tools for migration
//efcore for accessing Dbcontext
//sqlserver for accessing sqlserver
//Identity for accessing Registration $ Login
//stripe for payments
//mailkit and mimekit for emailsender

//For .net version 7 sqlserver checkingsomecertificates so we have to set it to false otherwise 
//some error will come. Google it to overcome it

//[validatenever] Just for model validation
//? for DB and model validation. BOTH

//in foreach the  changable datatypes as objects can be changed"

//[bindproperty] automaticallly binds the populated object in the post action method

//IMPORTANT
//asp-route-id  its just optional, we can even have a action method public IactionResult(without this param),it still works

//readonly post the exact id orvalues to controller but disabled make the value null and post it to controller

//use "return functionname" on onclick attribute inorder to delay or cancel post action untill the function completes
//we can use bind property ( [bindproperty] )to use entities as in pages(without passing the objects to view from controllers everytime)

//sessions are key value pairs//viewcomponent is another way to use data inside views but without any tag helpers



//IMPORTANT LAYOUT is the main page which gets executed in a view


//read the microsoft documentation to provide authentication support . iT's not tough!

//htt://stackoverflow.com/questions/17676974/changing-objects-value-in-foreach-loop
//FOREACH IENUMERABLE DIFFERENT LOOP //WE CAN ONLY CHANCE OBJECT MEMBERS BUT WE CAN"T CHANGE OR INITIALIZE OTHER TYPES


//dont't use update backtoback with no edits even made after the first update. it will result in runtime error. snd dont extract same entity two times