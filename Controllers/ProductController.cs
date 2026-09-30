using EnigmaStock.Data;
using Microsoft.AspNetCore.Mvc;
using EnigmaStock.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            var products = _db.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        //GET
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            var categories = _db.Categories.ToList();
            ViewBag.categories = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            var categories = _db.Categories.ToList();
            ViewBag.categories = categories;

            return View(obj);
        }



        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _db.Categories.ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = _db.Categories.ToList();

            return View(obj);
        }



        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            { return NotFound();

            }
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}