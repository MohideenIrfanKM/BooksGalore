using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using BooksGalore.Repository.IRepository;
using BooksGalore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksGalore.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitofWork db;

		public CartController(IUnitofWork db)
		{
			this.db = db;
		}

		public IActionResult Index()
		{
			var x = (ClaimsIdentity)User.Identity;
			var claim = x.FindFirst(ClaimTypes.NameIdentifier);
		

			ShoppingCartVM obj = new()
			{
				scart = db.ShoppingCartRepository.GetAll(u=>u.ApplicationUserId==claim.Value,includeProperties:"product").ToList(),
			};
			foreach(var cart in obj.scart)
			{
				cart.price = pricecalc(cart.count, cart.product.price, cart.product.price50,cart.product.price100);

			}

			return View(obj);

			
		}
		private double pricecalc(int count,double p,double p50,double p100)
		{
			if(count<50)
			{
				return p;
			}
			else
			{
				if (count < 100)
				{
					return p50;
				}
				else
				{
					return p100;
				}
			}

		}
	}
}
