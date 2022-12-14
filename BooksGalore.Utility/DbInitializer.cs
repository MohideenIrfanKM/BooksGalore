using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksGalore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BooksGalore.Db;

namespace BooksGalore.Utility
{
    public class DbInitializer : IDbInitializer
	{
		public readonly Dbcontext db;
		public readonly RoleManager<IdentityRole> _roleManager;
		public readonly UserManager<IdentityUser> _userManager;
		public DbInitializer(
			   UserManager<IdentityUser> userManager,
			   RoleManager<IdentityRole> roleManager,
			   Dbcontext db)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			this.db = db;
		}
		public void Initialize()
		{
			try
			{
				if (db.Database.GetPendingMigrations().Count() > 0)
				{
					db.Database.Migrate();
				}
			}
			catch (Exception e)
			{

			}
			//First do migrations if they are not done yet
			

			//Create Roles and Admin Account if Admin ACcount is not created

			if (!_roleManager.RoleExistsAsync(Util._Adm).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(Util._ind)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._Emp)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._com)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._Adm)).GetAwaiter().GetResult();

				//create admin account

				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "AdminBooksGalore@bg.com",
					Email = "AdminBooksGalore@bg.com",
					PhoneNumber = "1112223333",
					StreetAddress = "test 123 Ave",
					State = "IL",
					PostalCode = "23422",
					City = "Chicago",
					Name="Irfan"

				},"Admin@123");//password

				ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Email == "AdminBooksGalore@bg.com");
				_userManager.AddToRoleAsync(user, Util._Adm).GetAwaiter().GetResult();
			}
			return;

		
		}
	}
}
