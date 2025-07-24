using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace G3_PerfumeShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly G3_PerfumeShopDB_Iter3Context _context; // Add your DbContext

        public HomeController(ILogger<HomeController> logger, G3_PerfumeShopDB_Iter3Context context)
        {
            _logger = logger;
            _context = context; // Assign the context for database access
        }

        public IActionResult Index()
        {
            // Fetch the 8 newest products along with their sizes and prices
            var latestProducts = _context.Products
                                         .Include(p => p.ProductSizePricings) // Include the size and pricing info
                                         .OrderByDescending(p => p.ReleaseYear) // Sort by ReleaseYear
                                         .Take(8) // Take 8 latest products
                                         .ToList();

            return View(latestProducts); // Pass the product list to the view
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        // Action cho UnauthorizedAccess
        public IActionResult UnauthorizedAccess()
        {
            return View();
        }

    }
}
