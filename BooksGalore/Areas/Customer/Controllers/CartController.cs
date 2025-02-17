﻿using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Xml.Schema;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
using BooksGalore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace BooksGalore.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitofWork db;
		public readonly IEmailSender _emailSender;

		public CartController(IUnitofWork db,IEmailSender _emailSender)
		{
			this.db = db;
			this._emailSender = _emailSender;
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
			
			obj.orderHeader.ApplicationUserId = claim.Value;


			//in foreach the  changable datatypes as objects can be changed"
			foreach (var cart in obj.scart)
			{

				cart.price = pricecalc(cart.count, cart.product.price, cart.product.price50, cart.product.price100);
				obj.orderHeader.OrderTotal += (cart.price * cart.count);
			}
			ApplicationUser applicationUser = db.ApplicationUserRepository.getFirstorDefault(u => u.Id == claim.Value);
			if(applicationUser.CompanyId.GetValueOrDefault()==0)
			{
                obj.orderHeader.PaymentStatus = Util.PaymentStatusPending;
                obj.orderHeader.OrderStatus = Util.StatusPending;

            }
			else
			{
				obj.orderHeader.PaymentStatus = Util.PaymentStatusDelayedPayment;
				obj.orderHeader.OrderStatus = Util.StatusApproved;
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
					Price=cart.price

				};
				db.OrderDetailsRepository.Add(order);
				db.Save();
			}
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)//adding getvalueordefault because if companyid is not even added it will be null
			{
				//adding stripe settings. These are inbuilt and we are customizing it
				var domain = "https://localhost:7028/";
				var options = new SessionCreateOptions
				{
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={obj.orderHeader.Id}",
					CancelUrl = domain + $"Customer/Cart/Index",
				};
				foreach (var item in obj.scart)
				{
					var data = new SessionLineItemOptions
					{
						// Provide the exact Price ID (for example, pr_1234) of the product you want to sell
						//Price = item.price.ToString(), only can add price or price data
						Quantity = item.count,
						PriceData = new SessionLineItemPriceDataOptions()
						{
							UnitAmount = (long)(item.price * 100),//don't know why??? DOUBT----solved as it defaulty coverts integer to double(as we already have double point will move btwo step back) convers
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions()
							{
								Name = item.product.Name,
							}
						}


					};
					options.LineItems.Add(data);
				}
				//session will be created here which was injected in the get view
				var service = new SessionService();
				Session session = service.Create(options);//after payment all the session details will be populated here
														  //after Session Creation We have to update the session id and payment Id in the Model..in order to confirm order
				db.OrderHeaderRepository.UpdateStripePaymentId(obj.orderHeader.Id, session.Id, session.PaymentIntentId);
				db.Save();


				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			else
			{

				return RedirectToAction("OrderConfirmation", "cart", new { id = obj.orderHeader.Id });
				//redirect to actio n with actionmethod,controllername and ID
			}
			
			//after order is created we can delete the shopping cart
			//db.ShoppingCartRepository.RemoveRange(obj.scart);
			//db.Save();
			//return RedirectToAction("Index","Home");
		}
		//we are using orderconfirmation in order to confirm that the payment is done before approving
		public IActionResult OrderConfirmation(int id)
		{
			
			OrderHeader orderHeader = db.OrderHeaderRepository.getFirstorDefault(u => u.Id == id,includeProperties:"ApplicationUser");
			IEnumerable<OrderDetails> orderDetails = db.OrderDetailsRepository.GetAll(u => u.OrderId == orderHeader.Id, includeProperties: "product");
			string items = "";
			foreach(var i in orderDetails)
			{
				items= items + i.product.Name.ToString()+" - Count("+i.Count.ToString()+")";
			}
			items=items.Replace(")",")"+System.Environment.NewLine);//to include new line in string
			
			if (orderHeader.PaymentStatus != Util.PaymentStatusDelayedPayment)
			{
			
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);//here only payment id available
				if (session.PaymentStatus.ToLower() == "paid")
				{
					orderHeader.PaymentDate = DateTime.Now;
					db.OrderHeaderRepository.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					db.OrderHeaderRepository.UpdateStatus(id, Util.StatusApproved, Util.PaymentStatusApproved);
					db.Save();
				}
			}
			
			_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Order PLACED!!!", $"<p>Your Order for the items of worth ${orderHeader.OrderTotal} gets <b>APPROVED</b></p><p>Your Order Items are :</p>{items}<p>Your Order Reference Id is {id}.<p><b>Thank You for Ordering</b><p>.");
            List<ShoppingCart> shoppingCarts = db.ShoppingCartRepository.GetAll(u=>u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			if (shoppingCarts.Count > 0)
			{
				db.ShoppingCartRepository.RemoveRange(shoppingCarts);
				db.Save();
			}
			//HttpContext.Session.SetInt32(Util.SessionCart, db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
			//or we can use clear
			HttpContext.Session.Clear();
            return View(id);
		}
		public IActionResult DecCount(int scid)
		{
            var x = (ClaimsIdentity)User.Identity;
            var claim = x.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCart obj = db.ShoppingCartRepository.getFirstorDefault(u => u.Id == scid);
			db.ShoppingCartRepository.DecCount(obj, 1);
			if(obj.count<1)
			{
				db.ShoppingCartRepository.Remove(obj);

            }
            db.Save();
            HttpContext.Session.SetInt32(Util.SessionCart, db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);

            return RedirectToAction(nameof(Index));


		}
		public IActionResult Remove(int scid)
		{
			var x = (ClaimsIdentity)User.Identity;
			var claim = x.FindFirst(ClaimTypes.NameIdentifier);
			ShoppingCart obj = db.ShoppingCartRepository.getFirstorDefault(u => u.Id == scid);
			db.ShoppingCartRepository.Remove(obj);
			db.Save();
			HttpContext.Session.SetInt32(Util.SessionCart, db.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
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
