
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksGalore.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly IUnitofWork db;
        private readonly IWebHostEnvironment env;
        public ProductController(IUnitofWork db,IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
            //if we use db context then we can use db.dbsetname.Include("Covertype")
        }
        public IActionResult Index()
        {

            //IMPORTANT USE VIEWBAG/VIEWDATA INORDER to PASS MULTIPLE DATA INTO THE VIEW FOR JUST CONSEQUENT REQUEST 

            //  Category category=db.Categories.SingleOrDefault(c => c.id == 1);
            // return View(category);  

            return View();
        }
        //ALTERNATIVE TO AJAX FOR DISPLAYING PRODUCT DATA
        //public IActionResult temp(int? id)
        //{
        //    IEnumerable<Product> st = db.ProductRepository.GetAll("Category,Covertype");
        //    return View(st);
        //}
        [HttpGet]
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
                coverlist = db.CoverTypeRepository.GetAll().Select(
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
                pdt.product=db.ProductRepository.getFirstorDefault(u=>u.Id==id);    
                
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
                if (file != null)//only a new file added
                {
                    if (c.product.ImageURL != null)
                    {
                        if (System.IO.File.Exists(Path.Combine(path, c.product.ImageURL)))
                            System.IO.File.Delete(Path.Combine(path, c.product.ImageURL));//if to trim use c.product.ImageURL.trimStart("/");
                    }
                    string name = Guid.NewGuid().ToString();
                    var upl = Path.Combine(path, @"Images/Products");
                    var ext = Path.GetExtension(file.FileName);
                    using (var fstream = new FileStream(Path.Combine(upl, name + ext), FileMode.Create))//this adds '/' inbetween
                    {
                        file.CopyTo(fstream);
                    }
                    c.product.ImageURL = @"Images/Products/" + name + ext;
                }
                    if (c.product.Id==0)
                    {
                        db.ProductRepository.Add(c.product);
                    db.Save();
                    TempData["success"] = "Product Added Successfully";

                }
                    else
                    {
                        db.ProductRepository.Update(c.product);
                    db.Save();
                    TempData["success"] = "Product Updated Successfully";

                }
   

                return RedirectToAction("Index");
             }
            return View(c);

        }
        #region API-CALLS
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = db.ProductRepository.GetAll(includeProperties:"Category,Covertype");//GetAll("includeProperties:Category,Covertype")
            return Json(new {data = products });

        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = db.ProductRepository.getFirstorDefault(u => u.Id == id);
            if (obj == null)
            {

                return Json(new { success = false, msg = "Error While  deleting!!" });
            }
            else
            {
                string path = env.WebRootPath;

                db.ProductRepository.Remove(obj);
                if (System.IO.File.Exists(Path.Combine(path, obj.ImageURL)))
                    System.IO.File.Delete(Path.Combine(path, obj.ImageURL));
                db.Save();
                return Json(new { success = true, msg = "Product Deleted Successfully!!" });
            }
        }

        #endregion

    }
}




