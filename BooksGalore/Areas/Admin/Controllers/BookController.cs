using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BooksGalore.Controllers
{
    public class BookController : Controller
    {
        private readonly IUnitofWork db;
        public BookController(IUnitofWork db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = db.CategoryRepository.GetAll();

            //  Category category=db.Categories.SingleOrDefault(c => c.id == 1);
            // return View(category);  
            return View(categories);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category c)
        {
            int val;
            bool d = int.TryParse(c.Name, out _);
            if (d == true)
            {
                ModelState.AddModelError("Name", "Name should not be a Number!!");
            }
            if (ModelState.IsValid)
            {
                db.CategoryRepository.Add(c);
                db.Save();
                TempData["success"] = "Category Added Successfully";

                return RedirectToAction("Index");
            }
            return View(c);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }
            else
            {
                var temp = db.CategoryRepository.getFirstorDefault(u=>u.Id==id);
                return View(temp);
            }
        }
        [HttpPost]
        public IActionResult Edit(Category c)
        {
            int val;
            bool d = int.TryParse(c.Name, out _);
            if (d == true)
            {
                ModelState.AddModelError("Name", "Name should not be a Number!!");
            }
            if (ModelState.IsValid)
            {
                db.CategoryRepository.Update(c);
                db.Save();
                TempData["success"] = "Category Updated Successfully";

                return RedirectToAction("Index");
            }
            return View(c);

        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }
            else
            {
                //var temp = db.Categories.Find(id);
                var temp=db.CategoryRepository.getFirstorDefault(c => c.Id == id);
                return View(temp);
            }
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var temp  = db.CategoryRepository.getFirstorDefault(c => c.Id == id);

            db.CategoryRepository.Remove(temp);
                db.Save();
            TempData["success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
            
        }
    }
}