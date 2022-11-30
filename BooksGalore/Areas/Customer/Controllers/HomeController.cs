using System.Diagnostics;
using System.Security.Claims;
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Details(int pdtid)
        {
            Product pdt = db.ProductRepository.getFirstorDefault(u=>u.Id==pdtid,"Category,Covertype");
            ShoppingCart obj = new()
            {
                product=pdt,
                ProductId= pdtid,
                count=1
            };
            return View(obj);
        }
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart obj)
        {
            var x=(ClaimsIdentity)User.Identity;
            var claim = x.FindFirst(ClaimTypes.NameIdentifier);
            obj.ApplicationUserId = claim.Value;
            //toget the appid the above process
            ShoppingCart cart = db.ShoppingCartRepository.getFirstorDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == obj.ProductId);
            if(cart != null)//incrementing only count if record already exist in shopping cart
            {
                db.ShoppingCartRepository.IncCount(cart, obj.count);
                db.Save();
                
            }
            else
            {
				db.ShoppingCartRepository.Add(obj);
                db.Save();

			}
            return RedirectToAction(nameof(Index));



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