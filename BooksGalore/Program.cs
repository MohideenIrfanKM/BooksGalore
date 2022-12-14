using BooksGalore.Db;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Dbcontext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Connect")/*,b=>b.MigrationsAssembly("BooksGalore.Models")*/));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(/*options => options.SignIn.RequireConfirmedAccount = true*/).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<Dbcontext>();
builder.Services.AddSingleton<IEmailSender, EmailSender>(); //dummy email sender in first place as we override Default identity inorder to include roles, default email sender will not be available
builder.Services.AddScoped<IDbInitializer,DbInitializer>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.Configure<BooksGalore.Utility.Stripe>(builder.Configuration.GetSection("Stripe"));//getting connection from appsettings
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";

});
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1286809618839890";
    options.AppSecret = "1d4ef69a8850a35b2817ec0dc5782f15";
    //retrieved from developers.facebook.com
});
//builder.Services.AddAuthentication().AddTwitter(options =>
//{
//    options.ConsumerKey = "NkpDMkRadFFPdGNLTER5dWY1QnA6MTpjaQ";
//    options.ConsumerSecret = "RyL6da-bBVJOCcaTc1dFhZDXut_N2ShO1ZjzcjrTgQsYlRSf6y";
//});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "714170082848-fbapgp0bh3hfm60g77dtmkqafhvabshb.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();//this is for using css,bootrsap etc which are static files

app.UseRouting();
//stripe configuration
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();//for this we have to use get not tostring
execute();//to execute the seed database process


app.UseAuthentication();


app.UseAuthorization();

app.MapRazorPages();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void execute()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbi=scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbi.Initialize();

        //to get the builder services here itself...
    }
}