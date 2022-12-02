using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
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
		public IActionResult IncCount(int scid)
		{
			ShoppingCart obj = db.ShoppingCartRepository.getFirstorDefault(u => u.Id == scid);
			db.ShoppingCartRepository.IncCount(obj, 1);
			db.Save();
			return RedirectToAction(nameof(Index));
			

		}
		public IActionResult Summary()
		{
			var x = (ClaimsIdentity)User.Identity;
			var claim = x.FindFirst(ClaimTypes.NameIdentifier);


			ShoppingCartVM obj = new()
			{
				scart = db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "product").ToList(),
				orderHeader = new(),

			};
			obj.orderHeader.ApplicationUser = db.ApplicationUserRepository.getFirstorDefault(u => u.Id == claim.Value);
			ApplicationUser usr = obj.orderHeader.ApplicationUser;
			//obj.orderHeader.OrderDate = DateTime.Now;
			obj.orderHeader.Name = usr.Name;
			obj.orderHeader.PhoneNumber = usr.PhoneNumber;
			obj.orderHeader.StreetAddress = usr.StreetAddress;
			obj.orderHeader.City = usr.City;
			obj.orderHeader.State = usr.State;
			obj.orderHeader.PostalCode = usr.PostalCode;
			
		
			//in foreach the  changable datatypes as objects can be changed"
			foreach (var cart in obj.scart)
			{

				cart.price = pricecalc(cart.count, cart.product.price, cart.product.price50, cart.product.price100);
				obj.orderHeader.OrderTotal += (cart.price * cart.count);
			}
			return View(obj);
		}
		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPost(ShoppingCartVM obj)
		{
			var x = (ClaimsIdentity)User.Identity;
			var claim = x.FindFirst(ClaimTypes.NameIdentifier);



			obj.scart = db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "product").ToList();

			obj.orderHeader.OrderDate = DateTime.Now;
			obj.orderHeader.PaymentStatus = Util.PaymentStatusPending;
			obj.orderHeader.OrderStatus = Util.StatusPending;
			obj.orderHeader.ApplicationUserId = claim.Value;


			//in foreach the  changable datatypes as objects can be changed"
			foreach (var cart in obj.scart)
			{

				cart.price = pricecalc(cart.count, cart.product.price, cart.product.price50, cart.product.price100);
				obj.orderHeader.OrderTotal += (cart.price * cart.count);
			}
			db.OrderHeaderRepository.Add(obj.orderHeader);
			db.Save();
			foreach (var cart in obj.scart)
			{
				OrderDetails order = new OrderDetails()
				{
					ProductId = cart.ProductId,
					Count = cart.count,
					OrderId = obj.orderHeader.Id,
					Price=obj.orderHeader.OrderTotal

				};
				db.OrderDetailsRepository.Add(order);
				db.Save();
			}
			//after order is created we can delete the shopping cart
			db.ShoppingCartRepository.RemoveRange(obj.scart);
			db.Save();
			return RedirectToAction("Index","Home");
		}
		public IActionResult DecCount(int scid)
		{

			ShoppingCart obj = db.ShoppingCartRepository.getFirstorDefault(u => u.Id == scid);
			db.ShoppingCartRepository.DecCount(obj, 1);
			if(obj.count<1)
			{
				db.ShoppingCartRepository.Remove(obj);
			}
			db.Save();
			return RedirectToAction(nameof(Index));


		}
		public IActionResult Remove(int scid)
		{
			ShoppingCart obj = db.ShoppingCartRepository.getFirstorDefault(u => u.Id == scid);
			db.ShoppingCartRepository.Remove(obj);
			db.Save();
			return RedirectToAction(nameof(Index));


		}
		public IActionResult Index()
		{
			var x = (ClaimsIdentity)User.Identity;
			var claim = x.FindFirst(ClaimTypes.NameIdentifier);
		

			ShoppingCartVM obj = new()
			{
				scart = db.ShoppingCartRepository.GetAll(u=>u.ApplicationUserId==claim.Value,includeProperties:"product").ToList(),
				orderHeader=new(),
			
			};
			//in foreach the  changable datatypes as objects can be changed"
			foreach (var cart in obj.scart)
			{
			
				cart.price = pricecalc(cart.count, cart.product.price, cart.product.price50,cart.product.price100);
				obj.orderHeader.OrderTotal +=( cart.price * cart.count);
			}

			return View(obj);

			
		}
		//we can have these methods here!!!No Problem
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
