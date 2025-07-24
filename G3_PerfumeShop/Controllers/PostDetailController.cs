using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

public class PostDetailController : Controller
{
    public readonly G3_PerfumeShopDB_Iter3Context _context;
    public PostDetailController(G3_PerfumeShopDB_Iter3Context context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Detail(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Index", "Login");
        }
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return NotFound();
        }
        if (user.RoleId != 2)
        {
            return Unauthorized();
        }
        var blog = _context.Blogs
            .Include(b => b.BlogCategory)
            .Include(b => b.BlogStatus)
            .SingleOrDefault(b => b.Id == id);

        if (blog == null)
        {
            return NotFound();
        }

        return View("Index",blog);
    }

    [HttpGet]
    public JsonResult GetMockData()
    {
        var x = new { toi = "Tung Lam",
        hotoi = "Nguyen"};
        return Json(x);
    }
}