using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
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
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders;
             orderHeaders=db.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");//GetAll("includeProperties:Category,Covertype")
            return Json(new { data = orderHeaders });

        }
        #endregion
    }
}
