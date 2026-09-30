using Microsoft.AspNetCore.Mvc;
using EnigmaStock.Data;
using EnigmaStock.Models;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var categories = _db.Categories.ToList();

            return View(categories);
        }


        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Create()

        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Category obj)
        {
            _db.Categories.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }



        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id== null || id==0)
            {
                return NotFound();
            }

            var categoryFromDb = _db.Categories.Find(id);
            if(categoryFromDb==null)
            {
                return NotFound();

            }

            return View(categoryFromDb);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");

            }
            return View(obj);
        }



        //GET- Delete
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id==null || id==0)
            {
                return NotFound();
            }

            var categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb==null)
            {
                return NotFound();

            }
            return View(categoryFromDb);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Deletepost(int? id)
        {
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }        
             
 }


