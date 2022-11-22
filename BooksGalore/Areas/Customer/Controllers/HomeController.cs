using System.Diagnostics;
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BooksGalore.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork db;

        public HomeController(ILogger<HomeController> logger,IUnitofWork db)
        {
            _logger = logger;
            this.db = db;

        }

        public IActionResult Index()
        {

            
           IEnumerable<Product> products=db.ProductRepository.GetAll("Category,Covertype");
            return View(products);
         }
        public IActionResult Details(int? id)
        {
            Product pdt = db.ProductRepository.getFirstorDefault(u=>u.Id==id,"Category,Covertype");
            ShoppingCart obj = new()
            {
                product=pdt,
                count=1
            };
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}