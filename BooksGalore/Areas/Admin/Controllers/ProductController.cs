
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksGalore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitofWork db;
        public ProductController(IUnitofWork db)
        {
            this.db = db;
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
            IEnumerable<SelectListItem> categlist = db.CategoryRepository.GetAll().Select(
    u => new SelectListItem
    {
        Text = u.Name, //when users select name ID will be retured as value
                    Value = u.Id.ToString()
    }
    );
            IEnumerable<SelectListItem> coverlist = db.CoverTypeRepository.GetAll().Select(
             u => new SelectListItem
             {
                 Text = u.Name, //when users select name ID will be retured as value
                 Value = u.Id.ToString(),
             }
         );
            var product = new Product(); //if target type known then use Product(target) pdt=new();
            if (id == null || id == 0)
            {
                //for create new

            }
            else
            {
                //for updating
            }
            ViewBag.Categlist = categlist;
            ViewBag.Coverlist = coverlist;
            return View(product);
        }
        //[HttpPost]
        //public IActionResult Upsert(Product c)
        //{
        //    int val;
        //    bool d = int.TryParse(c.Name, out _);
        //    if (d == true)
        //    {
        //        ModelState.AddModelError("Name", "Name should not be a Number!!");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        db.CategoryRepository.Update(c);
        //        db.Save();
        //        TempData["success"] = "Category Updated Successfully";

        //        return RedirectToAction("Index");
        //    }
        //    return View(c);

        //}
    } 
}
       