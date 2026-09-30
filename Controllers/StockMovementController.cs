using Microsoft.AspNetCore.Mvc;
using EnigmaStock.Data;
using EnigmaStock.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize]
    public class StockMovementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StockMovementController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET
        public IActionResult Index()
        {

            var movements= _db.StockMovement
                .Include(m=> m.Product)
                .OrderByDescending(m => m.Date)
                .ToList();

            return View(movements);
        }


        //GET- Stock In
        public IActionResult Create()
        {
            ViewBag.Products = _db.Products.ToList();

            return View();
        }

        //POST - Stock In

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StockMovement obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _db.Products.ToList();
                return View(obj);
            }

            var product = _db.Products.Find(obj.ProductId);

            if (product == null)
            {
                return NotFound();
            }


            product.Quantity += obj.Quantity;

            obj.MovementType = "Stock In";
            obj.Date = DateTime.Now;

            _db.StockMovement.Add(obj);

            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        //GET - Stock Out
        public IActionResult StockOut()
        {
            ViewBag.Products = _db.Products.ToList();

            return View();
        }


        //POST - Stock Out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockOut(StockMovement obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _db.Products.ToList();
                return View(obj);
            }

            var product = _db.Products.Find(obj.ProductId);

            if (product == null)
            {
                return NotFound();
            }
        

            if (obj.Quantity > product.Quantity)
            {
                ModelState.AddModelError("Quantity", "Stock Out Quantity cannot be greater than availabale srock");
                    ViewBag.products = _db.Products.ToList();

                return View(obj);
            }

            product.Quantity -= obj.Quantity;

            obj.MovementType = "Stock Out";
            obj.Date = DateTime.Now;

            _db.StockMovement.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");

            





        }
    }

}





