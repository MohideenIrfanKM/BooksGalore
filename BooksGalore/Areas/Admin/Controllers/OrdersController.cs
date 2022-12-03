using System.Diagnostics;
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
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

        public IActionResult Index()
        {
            return View();
        }
        #region API-CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            

             orderHeaders=db.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");//GetAll("includeProperties:Category,Covertype")
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
