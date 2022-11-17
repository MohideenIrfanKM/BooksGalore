
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksGalore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitofWork db;
        private readonly IWebHostEnvironment env;
        public ProductController(IUnitofWork db,IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }
        public IActionResult Index()
        {

            //IMPORTANT USE VIEWBAG/VIEWDATA INORDER to PASS MULTIPLE DATA INTO THE VIEW FOR JUST CONSEQUENT REQUEST 

            //  Category category=db.Categories.SingleOrDefault(c => c.id == 1);
            // return View(category);  

            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM pdt = new ProductVM()
            {
                product = new(),
                categlist = db.CategoryRepository.GetAll().Select(
               u => new SelectListItem //ORnew SelectlistItem(){}
               {
                 Text = u.Name, //when users select name ID will be retured as value
                 Value = u.Id.ToString()
               }),
                coverlist= db.CoverTypeRepository.GetAll().Select(
               u => new SelectListItem
               {
                 Text = u.Name, //when users select name ID will be retured as value
                 Value = u.Id.ToString(),
                }
         )

        };
        //if target type known then use Product(target) pdt=new();
            if (id == null || id == 0)
            {
                //for create new

            }
            else
            {
                //for updating
            }
           // ViewBag.Categlist = categlist;
           // ViewBag.Coverlist = coverlist;
            return View(pdt);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM c, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string path = env.WebRootPath;
                if (file != null) { 
                string name = Guid.NewGuid().ToString();
                var upl=Path.Combine(path, @"Images/Products");
                var ext=Path.GetExtension(file.FileName);
                using(var fstream=new FileStream(Path.Combine(upl,name+ext), FileMode.Create))
                 {
                        file.CopyTo(fstream);
                 }
                 c.product.ImageURL = @"Images/Products" + name + ext;
                 db.ProductRepository.Add(c.product);
                db.Save();
                TempData["success"] = "Product Updated Successfully";

                return RedirectToAction("Index");
            } }
            return View(c);

        }
        #region API-CALLS
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = db.ProductRepository.GetAll("Category,Covertype");//GetAll("includeProperties:Category,Covertype")
            return Json(new {data = products });
        }
        #endregion
    }
}
       