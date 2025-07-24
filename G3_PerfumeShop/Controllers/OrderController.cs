using CodeMegaVNPay.Services;
using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G3_PerfumeShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        public OrderController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        //Hiện ra các thông tin cảu trang my order
        public IActionResult MyOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSizePricing)
                .ThenInclude(psp => psp.Product)
                .Where(o => o.UserId == userId && o.Status == false).ToList();

            return View(cart);
        }
    }
}
