using System.Diagnostics;
using System.Security.Claims;
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
using BooksGalore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BooksGalore.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IUnitofWork db;

        public OrdersController(IUnitofWork db)
        {
            this.db = db;
        }
        [Authorize]
        public IActionResult Index()//we can even have this without(status) parameter, it still accepts
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderVM obj = new()
            {
                OrderHeader = db.OrderHeaderRepository.getFirstorDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = db.OrderDetailsRepository.GetAll(u => u.OrderId == id,includeProperties:"product")
            };
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrder(OrderVM obj)//or we can use bind property
        {
            OrderHeader orderHeader = db.OrderHeaderRepository.getFirstorDefault(u => u.Id == obj.OrderHeader.Id,tracked:false);
            //Here We are Using Tracked , because whenever an entity is created it gets automatically updated in database before even calling updated.so we don't need to use Update method explicitly
            //since it defeats Updates purpose we will be using other approach
            //inorder to avoid that we will be passing additional parameter, if you want to see that implementation look into implementation
            orderHeader.Name = obj.OrderHeader.Name;
            orderHeader.StreetAddress = obj.OrderHeader.StreetAddress;
            if(obj.OrderHeader.Carrier!=null && obj.OrderHeader.TrackingNumber!=null)
            {
                orderHeader.TrackingNumber = obj.OrderHeader.TrackingNumber;
                orderHeader.Carrier = obj.OrderHeader.Carrier;
            }
            orderHeader.City = obj.OrderHeader.City;
            orderHeader.PhoneNumber = obj.OrderHeader.PhoneNumber;
            orderHeader.State = obj.OrderHeader.State;
            orderHeader.PostalCode = obj.OrderHeader.PostalCode;
            db.OrderHeaderRepository.Update(orderHeader);
            TempData["success"] = "Order Details Updated";
            db.Save();
            return View("Index");
        }

        [Authorize(Roles =Util._Adm+","+Util._Emp)]
        public IActionResult OrderProcessing(int OrderHeaderId) {
            OrderHeader obj=db.OrderHeaderRepository.getFirstorDefault(u=>u.Id== OrderHeaderId,tracked:false);
            db.OrderHeaderRepository.UpdateStatus(OrderHeaderId, Util.StatusInProcess);
           
			//db.OrderHeaderRepository.Update(obj);



			db.Save();
            TempData["success"] = "Order Status Updated Successfully";//not responding after first request
            return RedirectToAction("Details","Orders",new { id=OrderHeaderId});
        }
		[Authorize(Roles = Util._Adm + "," + Util._Emp)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Shipping(OrderVM order)
        {
			OrderHeader obj=db.OrderHeaderRepository.getFirstorDefault(u=>u.Id== order.OrderHeader.Id,tracked:false);
            obj.ShippingDate = DateTime.Now;
            //obj.PaymentDueDate = DateTime.Now.AddDays(28);
            obj.Carrier = order.OrderHeader.Carrier;
            obj.TrackingNumber= order.OrderHeader.TrackingNumber;
            if (obj.PaymentStatus == Util.PaymentStatusDelayedPayment)
            {
                obj.PaymentDueDate = DateTime.Now.AddDays(30);
                db.OrderHeaderRepository.Update(obj);
            }
            db.OrderHeaderRepository.UpdateStatus(order.OrderHeader.Id, Util.StatusShipped);

           

			db.Save();
			TempData["success"] = "Order Status Updated Successfully";
			return RedirectToAction("Details", "Orders", new { id = order.OrderHeader.Id });




		}
		[Authorize(Roles = Util._Adm + "," + Util._Emp)] //inorder to authorize only selected roles
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder(OrderVM obj)
        {
			if(obj.OrderHeader.PaymentStatus==Util.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = obj.OrderHeader.PaymentId,
                    //Amount is default paid amount. if you want to be explicit then you can populate the value for Amount
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

				db.OrderHeaderRepository.UpdateStatus(obj.OrderHeader.Id, Util.StatusRefunded);
                TempData["success"] = "Order Cancelled!";


            }
            else
            {
				db.OrderHeaderRepository.UpdateStatus(obj.OrderHeader.Id, Util.StatusCancelled);
                TempData["success"] = "Order Cancelled! & PaymentRefunded";


            }
            db.Save();
            
			return RedirectToAction("Details","Orders",new { id= obj.OrderHeader.Id }); 
        }

        [HttpGet]
        public IActionResult PaymentConfirmation(int? id)
        {
            OrderHeader orderHeader = db.OrderHeaderRepository.getFirstorDefault(u => u.Id == id);
            if (orderHeader.PaymentStatus == Util.PaymentStatusDelayedPayment)
            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);//here only payment id available
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.PaymentDate = DateTime.Now;
                    db.OrderHeaderRepository.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    db.OrderHeaderRepository.UpdateStatus(orderHeader.Id, Util.StatusShipped, Util.PaymentStatusApproved);
                    db.Save();
                }
            }
            return View(id);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow(int OrderHeaderId) 
        {

            OrderVM obj = new()
            {
                OrderHeader=db.OrderHeaderRepository.getFirstorDefault(u=>u.Id==OrderHeaderId,tracked:false),
                OrderDetails=db.OrderDetailsRepository.GetAll(u=>u.OrderId==OrderHeaderId,includeProperties:"product"),
            };
                var domain = "https://localhost:7028/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"Admin/Orders/PaymentConfirmation?id={obj.OrderHeader.Id}",
                    CancelUrl = domain + $"Admin/Orders/Details?id={obj.OrderHeader.Id}",
                };
                foreach (var item in obj.OrderDetails)
                {
                    var data = new SessionLineItemOptions
                    {
                        // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                        //Price = item.price.ToString(), only can add price or price data
                        Quantity = item.Count,
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100),//don't know why??? DOUBT----solved as it defaulty coverts integer to double(as we already have double point will move btwo step back) convers
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
                db.OrderHeaderRepository.UpdateStripePaymentId(obj.OrderHeader.Id, session.Id, session.PaymentIntentId);
                db.Save();


                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            
            

			//return RedirectToAction("PaymentConfirmation","Orders",new {id=obj.OrderHeader.Id });   
        }


		#region API-CALLS
		[HttpGet]
        
        public IActionResult GetAll(string status)//we can even have this without(status) parameter, it still accepts
        {
            IEnumerable<OrderHeader> orderHeaders;

            var x = (ClaimsIdentity)User.Identity;
            var claim = x.FindFirst(ClaimTypes.NameIdentifier);

            //we can use user object to get the roles and we can have rolemanager obj to add the roles
            if (User.IsInRole(Util._Emp)|| User.IsInRole(Util._Adm))
            {
                orderHeaders = db.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();//GetAll("includeProperties:Category,Covertype")
                orderHeaders = orderHeaders.OrderByDescending(u => u.Id);// working but not eworking inside view

			}
            else
            {
                orderHeaders = db.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"ApplicationUser").OrderByDescending(u => u.Id);
            }
            //orderHeaders =db.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");//GetAll("includeProperties:Category,Covertype")
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == Util.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Util.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Util.StatusShipped );
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Util.StatusApproved);
                    break;
                default:
                    
                    break;
            }


            return Json(new { data = orderHeaders });

        }
        #endregion
    }
}
