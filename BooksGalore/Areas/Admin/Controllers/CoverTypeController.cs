using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BooksGalore.Controllers
{
    [Area("Admin")]

    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork db;
        public CoverTypeController(IUnitofWork db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> Ccovertypes = db.CoverTypeRepository.GetAll();

            //  Category category=db.Categories.SingleOrDefault(c => c.id == 1);
            // return View(category);  
            return View(Ccovertypes);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType c)
        {
            int val;
            bool d = int.TryParse(c.Name, out _);
            if (d == true)
            {
                ModelState.AddModelError("Name", "Name should not be a Number!!");
            }
            if (ModelState.IsValid)
            {
                db.CoverTypeRepository.Add(c);
                db.Save();
                TempData["success"] = "Cover Type Added Successfully";

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
                var temp = db.CoverTypeRepository.getFirstorDefault(u => u.Id == id);
                return View(temp);
            }
        }
        [HttpPost]
        public IActionResult Edit(CoverType c)
        {
            int val;
            bool d = int.TryParse(c.Name, out _);
            if (d == true)
            {
                ModelState.AddModelError("Name", "Name should not be a Number!!");
            }
            if (ModelState.IsValid)
            {
                db.CoverTypeRepository.Update(c);
                db.Save();
                TempData["success"] = "Cover Type Updated Successfully";

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
                var temp = db.CoverTypeRepository.getFirstorDefault(c => c.Id == id);
                return View(temp);
            }
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var temp = db.CoverTypeRepository.getFirstorDefault(c => c.Id == id);

            db.CoverTypeRepository.Remove(temp);
            db.Save();
            TempData["success"] = "Cover Type Deleted Successfully";

            return RedirectToAction("Index");

        }
    }
}