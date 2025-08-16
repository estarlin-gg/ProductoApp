using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using ProductoApp.Models;

namespace ProductoApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("User") != null;


        public IActionResult Index(string search, int? lowStock, string sortOrder)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var products = _context.Products.AsQueryable();


            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }


            if (lowStock.HasValue)
            {
                products = products.Where(p => p.Stock < lowStock.Value);
            }


            ViewBag.NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSort = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.StockSort = sortOrder == "stock" ? "stock_desc" : "stock";

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "stock":
                    products = products.OrderBy(p => p.Stock);
                    break;
                case "stock_desc":
                    products = products.OrderByDescending(p => p.Stock);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            var productList = products.ToList();

            if (!productList.Any())
            {
                ViewBag.Message = "No se encontraron productos.";
            }

            return View(productList);
        }

        public IActionResult ExportToExcel()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var products = _context.Products.ToList();

            if (!products.Any())
            {
                TempData["Error"] = "No hay datos para exportar.";
                return RedirectToAction("Index");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Productos");
                worksheet.Cell(1, 1).Value = "Nombre";
                worksheet.Cell(1, 2).Value = "Precio";
                worksheet.Cell(1, 3).Value = "Stock";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = products[i].Name;
                    worksheet.Cell(i + 2, 2).Value = products[i].Price;
                    worksheet.Cell(i + 2, 3).Value = products[i].Stock;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
                }
            }
        }

        public IActionResult Details(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
