using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EnigmaStock.Models;
using EnigmaStock.Data;
using Microsoft.AspNetCore.Mvc;



namespace EnigmaStock.Controllers
{
    [Authorize]
    
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SaleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var sales = _db.Sales
                .Include(s => s.saleItems)
                .OrderByDescending(s => s.SaleDate)
                .ToList();


            return View(sales);
        }




        //GET - Create Sale
        public IActionResult Create()
        {
            var sales = new Sale();

            return View(sales);
        }



        //POST - Create Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sale obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            _db.Sales.Add(obj);
            _db.SaveChanges();

            obj.InvoiceNumber = $"INV-{obj.Id:D6}";

            _db.SaveChanges();

            return RedirectToAction("Index");

        }


        //GET - Sale Details
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var sale = _db.Sales
                .Include(s => s.saleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefault(s => s.Id == id);


            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }


        //GET - Edit Sale 
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == id);


            if(sale == null)
            {
                return NotFound();
            }

            if (sale.Status != "Pending")
            {
                return BadRequest("Only Pending Sales can be edited");
            }

            return View(sale);
        }


        //POST - Edit Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Sale obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == obj.Id);

            if (sale == null)
            {
                return NotFound();
            }

            if(sale.Status != "Pending")
            {
                return BadRequest("Only Pending sales can be edited");
            }

            sale.CustomerName = obj.CustomerName;
            sale.CustomerPhone = obj.CustomerPhone;
            sale.PaymentMethod = obj.PaymentMethod;

            _db.SaveChanges();

            return RedirectToAction("Details", new { id = sale.Id });
        }




        //POST -Delete Cancelled Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var sale = _db.Sales
                .Include(s => s.saleItems)
                .FirstOrDefault(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }
            if (sale.Status != "Cancelled")
            {
                return BadRequest("Only Cancelled sales can be deleted");
            }

            // Remove the sale items
            _db.SaleItems.RemoveRange(sale.saleItems);

            // Remove the sale
            _db.Sales.Remove(sale);
            _db.SaveChanges();
            
            return RedirectToAction("Index");
        }




            //GET - Add Item
        public IActionResult AddItem(int saleId)
        {
            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == saleId);

            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Status != "Pending")
            {
                return BadRequest("Only pending sales can be modified");
            }

            ViewBag.Products = _db.Products.ToList();

            var saleItem = new SaleItem
            {
                SaleId = saleId
            };

            return View(saleItem);
        }


        //POST - Add Item 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(SaleItem obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _db.Products.ToList();

                return View(obj);
            }


            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == obj.SaleId);
            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Status != "Pending")
            {
                return BadRequest("Only Pending sales can be modified");
            }


            var product = _db.Products
                .FirstOrDefault(p => p.Id == obj.ProductId);

            if (product == null)
            {
                return NotFound();
            }


            if (obj.Quantity > product.Quantity)
            {
                ModelState.AddModelError("Quantity", "Insufficient stock available.");

                ViewBag.Products = _db.Products.ToList();
                return View(obj);
            }

            // Remove quantity from stock
            product.Quantity -= obj.Quantity;


            obj.SellingPrice = product.SellingPrice;

            _db.SaleItems.Add(obj);

            //Record stock movement
            var movement = new StockMovement
            {
                ProductId = obj.ProductId,
                Quantity = -obj.Quantity,
                MovementType = "Stock Out",
                Date = DateTime.Now,
                Note = $"Sale item added to {sale.InvoiceNumber}.  Removed {obj.Quantity} units from inventory "

            };


            //Add the sale item
            _db.StockMovement.Add(movement);
            _db.SaveChanges();

            return RedirectToAction("Details", new { id = obj.SaleId });
        }




        //POST Remove Sale Item 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem(int id)
        {
            var saleItem = _db.SaleItems
                .FirstOrDefault(si => si.Id == id);

            if (saleItem == null)
            {
                return NotFound();
            }

            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == saleItem.SaleId);

            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Status != "Pending")
            {
                return BadRequest("Only pending sales can be modified");
            }
            
                var product = _db.Products
                    .FirstOrDefault(p => p.Id == saleItem.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                //Return quantity to inventory
                product.Quantity += saleItem.Quantity;

                //Record stock movement 
                var movement = new StockMovement
                {
                    ProductId = saleItem.ProductId,
                    Quantity = saleItem.Quantity,
                    MovementType = "Stock in",
                    Date = DateTime.Now,
                    Note = $"Sale item removed from {sale.InvoiceNumber}. Returned {saleItem.Quantity} units to inventory."
                };

                _db.StockMovement.Add(movement);

                //Remove the sale item 
                _db.SaleItems.Remove(saleItem);

                _db.SaveChanges();

                return RedirectToAction("Details", new { id = saleItem.SaleId });
            }

          
        



        //POST - Complete Sale 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteSale(int id)
        {
            var sale = _db.Sales
                .FirstOrDefault(s => s.Id == id);


            if (sale == null)
            {
                return NotFound();
            }



            if (sale.Status != "Pending")
            {
                return BadRequest("Only pending sales can be completed.");

            }

            sale.Status = "Completed";

            _db.SaveChanges();

            return RedirectToAction("Details", new { id = sale.Id });

        }



        //POST - Cancel Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelSale(int id)
        {
            var sale = _db.Sales
                .Include(s => s.saleItems)
                .FirstOrDefault(s => s.Id == id);


            if (sale == null)
            {
                return NotFound();
            }


            if (sale.Status != "Pending")
            {
                return BadRequest("Only Pending sales can be cancelled.");
            }


            // Return quantities to inventory
            foreach (var saleItem in sale.saleItems)
            {
                var product = _db.Products
                    .FirstOrDefault(p => p.Id == saleItem.ProductId);


                if (product != null)
                {
                    product.Quantity += saleItem.Quantity;



             // Record stock movement
                    var movement = new StockMovement
                    {
                        ProductId = saleItem.ProductId,
                        Quantity = saleItem.Quantity,
                        MovementType = "Stock in",
                        Date = DateTime.Now,
                        Note = $"Sale item removed from {sale.InvoiceNumber}. Returned {saleItem.Quantity} units to inventory."
                    };
                    _db.StockMovement.Add(movement);
                }
            }
            sale.Status = "Cancelled";
            _db.SaveChanges();
            return RedirectToAction("Details", new { id = sale.Id });


        }


        //Invoice 
        public IActionResult Invoice(int? id)
        {
            if ( id == null || id== 0)
            {
                return NotFound();
            }


            var sale = _db.Sales
                .Include(s => s.saleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefault(s => s.Id == id);


            if (sale == null)
            {
                return NotFound();
            }


            return View(sale);
        }

    }
}

