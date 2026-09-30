using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnigmaStock.Models;
using EnigmaStock.Data;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize]
    public class SupplierController : Controller
        
    {   private readonly ApplicationDbContext _db;

        public  SupplierController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            var suppliers = _db.Suppliers.ToList();
            return View(suppliers);
        }


        //GET - Create supplier

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }



        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Supplier obj)
        {
            if (ModelState.IsValid)
            {
                _db.Suppliers.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(obj);
        }




        //GET- Edit Supplier
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var supplier = _db.Suppliers.Find(id);

            if (supplier == null)
            {
                return NotFound();

            }

            return View(supplier);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Supplier obj)
        {
            if (ModelState.IsValid)
            {
                _db.Suppliers.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");

            }
            return View(obj);
        }

    }
}
