
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

    public class CompanyController : Controller
    {
        private readonly IUnitofWork db;
        public CompanyController(IUnitofWork db, IWebHostEnvironment env)
        {
            this.db = db;
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
            Company cmp = new();

            //if target type known then use Product(target) pdt=new();
            if (id == null || id == 0)
            {
                //for create new


            }
            else
            {
                cmp = db.CompanyRepository.getFirstorDefault(u => u.Id == id);

            }
            // ViewBag.Categlist = categlist;
            // ViewBag.Coverlist = coverlist;
            return View(cmp);
        }

        [HttpPost]
        public IActionResult Upsert(Company c)
        {

            if (ModelState.IsValid)
            {

                if (c.Id == 0)
                {
                    db.CompanyRepository.Add(c);
                    db.Save();
                    TempData["success"] = "Company Added Successfully";

                }
                else
                {
                    db.CompanyRepository.Update(c);
                    db.Save();
                    TempData["success"] = "Company Updated Successfully";

                }


                return RedirectToAction("Index");
            }
            return View(c);

        }
        #region API-CALLS
        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = db.CompanyRepository.GetAll();//GetAll("includeProperties:Category,Covertype")
            return Json(new { data = companies });

        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = db.CompanyRepository.getFirstorDefault(u => u.Id == id);
            if (obj == null)
            {

                return Json(new { success = false, msg = "Error While  deleting!!" });
            }
            else
            {

                db.CompanyRepository.Remove(obj);
                
                db.Save();
                return Json(new { success = true, msg = "Company Deleted Successfully!!" });
            }
        }

        #endregion

    }
}




