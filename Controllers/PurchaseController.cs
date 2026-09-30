using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using EnigmaStock.Data;
using EnigmaStock.Models;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PurchaseController(ApplicationDbContext db)
        {
            _db = db;
        }
       
        //GET - Purchase List
        public IActionResult Index()
        {
            var purchases = _db.Purchases
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();

            return View (purchases);
        }


        //GET - Create Purchase
        public IActionResult Create()
        {
            ViewBag.Suppliers = _db.Suppliers.ToList();
            var purchase = new Purchase();

            return View(purchase);
        }


        //POST - Create Purchase

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Purchase obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _db.Suppliers.ToList();
                return View(obj);
            }

            _db.Purchases.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        //GET - Edit Purchase
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var purchase = _db.Purchases.Find(id);

            if (purchase == null)
            {
                return NotFound();
            }

            ViewBag.Suppliers = _db.Suppliers.ToList();

            return View(purchase);
        }



        //POST - Edit Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Purchase obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _db.Suppliers.ToList();
                return View(obj);
            }

            _db.Purchases.Update(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }




        //GET -     Purchase Details
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var purchase = _db.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.purchaseItems)
                .ThenInclude(pi => pi.product)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View (purchase);
        }


        //GET - Add Purchase Item
        public IActionResult AddItem(int? purchaseId)
        {
            if (purchaseId == null || purchaseId == 0)

            {
                return NotFound();
            }


            var purchase = _db.Purchases.Find(purchaseId);

            if (purchase == null)
            {
                return NotFound();
            }


            ViewBag.Products = _db.Products.ToList();

            var purchaseItem = new PurchaseItem
            {
                PurchaseId = purchaseId.Value
            };

            return View(purchaseItem);
        }




        //POST - Add Purchase Item

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(PurchaseItem obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _db.Products.ToList();
                return View(obj);
            }

            var purchase = _db.Purchases.Find(obj.PurchaseId);

            if (purchase == null)
            {
                return NotFound();
            }

            var product = _db.Products.Find(obj.ProductId);

            if (product == null)
            {

                return NotFound();
            }


            //Increase product Quantity
            product.Quantity += obj.Quantity;

            //Create Stock Movement

           var movement = new StockMovement
            {
                ProductId = obj.ProductId,
                Quantity = obj.Quantity,
                MovementType = "Stock In",
                Date = DateTime.Now,
                Note = "Purchase-" + purchase.ReferenceNumber
            };


            //Save Purchase Item
            _db.purchaseItems.Add(obj);

            //Save Stock Movement
            _db.StockMovement.Add(movement);


            _db.SaveChanges();

            return RedirectToAction("Details", new { id = obj.PurchaseId });
        }



        //GET -Edit Purchase Item 
        public IActionResult EditItem(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var purchaseItem = _db.purchaseItems
                .Include(pi => pi.product)
                .FirstOrDefault(pi => pi.Id == id);

            if (purchaseItem == null)
            {
                return NotFound();
            }

            ViewBag.Products = _db.Products.ToList();
            return View(purchaseItem);
        }


        //POST - Edit Purchase Item 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditItem(PurchaseItem obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _db.Products.ToList();
                return View(obj);
            }

            var existingItem = _db.purchaseItems
                .FirstOrDefault(pi => pi.Id == obj.Id);

            if (existingItem == null)
            {
                return NotFound();
            }
            int oldQuantity = existingItem.Quantity;

            var oldProduct = _db.Products
                .FirstOrDefault(p => p.Id == existingItem.ProductId);

            if(oldProduct == null)
            {
                return NotFound();
            }

            var newProduct = _db.Products
                .FirstOrDefault(p => p.Id == obj.ProductId);

            if (newProduct == null)
            {
                return NotFound();
            }

            //Calculate the difference
            int quantityDifference = obj.Quantity - oldQuantity;

            //Update product stock
            oldProduct.Quantity -= existingItem.Quantity;
            newProduct.Quantity += obj.Quantity;

            //Update Purchase Item
            existingItem.ProductId = obj.ProductId;
            existingItem.Quantity = obj.Quantity;
            existingItem.CostPrice = obj.CostPrice;

             // Create stock adjustment only when quantity changes   

            if (quantityDifference != 0)
            {

                var movement = new StockMovement
                {
                    ProductId = obj.ProductId,
                    Quantity = quantityDifference,
                    MovementType = "Stock Adjustment",
                    Date = DateTime.Now,
                    Note = $"Purchase Item {obj.Id} quantity changed from {oldQuantity} to {obj.Quantity}"


                };

                _db.StockMovement.Add(movement);
            }
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = existingItem.PurchaseId });

        }


        //GET - Delete Purchase Item
        public IActionResult DeleteItem(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var purchaseItem = _db.purchaseItems
                .Include(pi => pi.product)
                .FirstOrDefault(p => p.Id == id);

            if (purchaseItem == null)
            {
                return NotFound();
            }
            return View (purchaseItem);
        }


        //POST - Delete Purchase Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteItem(PurchaseItem obj)
        {
            var purchaseItem = _db.purchaseItems
                .FirstOrDefault(pi => pi.Id == obj.Id);

            if (purchaseItem== null)
            {
                return NotFound();
            }

            var product = _db.Products
                .FirstOrDefault(p => p.Id == purchaseItem.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            var purchase = _db.Purchases
                .FirstOrDefault(p => p.Id == purchaseItem.PurchaseId);

            if (purchase == null)
            {
                return NotFound();
            }



            //Remove the purchase item's quantity from current inventory
            product.Quantity -= purchaseItem.Quantity;


            //Record the inventory adjusment
            var movement = new StockMovement
            {
                ProductId = purchaseItem.ProductId,
                Quantity = -purchaseItem.Quantity,
                MovementType = "Stock Deleted",
                Date = DateTime.Now,
                Note = $"purchase Item {purchaseItem.Id} deleted from {purchase.ReferenceNumber}. Removed {purchaseItem.Quantity} units from inventory."
            };

            _db.StockMovement.Add(movement);


            //Delete the purchase item
            _db.purchaseItems.Remove(purchaseItem);

            _db.SaveChanges();

            return RedirectToAction("Details", new { id = purchaseItem.PurchaseId });
        }

    }
}
