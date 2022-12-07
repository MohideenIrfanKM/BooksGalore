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
        public IActionResult Shipping(OrderVM order)
        {
			OrderHeader obj=db.OrderHeaderRepository.getFirstorDefault(u=>u.Id== order.OrderHeader.Id,tracked:false);
            obj.ShippingDate = DateTime.Now;
            obj.Carrier = order.OrderHeader.Carrier;
            obj.TrackingNumber= order.OrderHeader.TrackingNumber;   
			db.OrderHeaderRepository.UpdateStatus(order.OrderHeader.Id, Util.StatusShipped);
           

			db.Save();
			TempData["success"] = "Order Status Updated Successfully";
			return RedirectToAction("Details", "Orders", new { id = order.OrderHeader.Id });




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
                orderHeaders = db.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");//GetAll("includeProperties:Category,Covertype")
            }
            else
            {
                orderHeaders = db.OrderHeaderRepository.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"ApplicationUser");
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
