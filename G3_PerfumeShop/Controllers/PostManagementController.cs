using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace G3_PerfumeShop.Controllers
{
    public class PostManagementController : Controller
    {
        public readonly G3_PerfumeShopDB_Iter3Context _context;

        public PostManagementController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
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
            var blogCategories = _context.BlogCategories.ToList();
            var authors = _context.Blogs.Select(b => b.Author).Distinct().ToList();

            ViewBag.BlogCategories = blogCategories;
            ViewBag.Authors = authors;
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.BlogStatus).ToList();

            return View(blogs);
        }

        [HttpPost]

        public IActionResult ManagePosts(int? BlogCategoryId, string? Title , string? Author)
        {

            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.BlogStatus).AsQueryable();
            if (string.IsNullOrEmpty(Title) && !BlogCategoryId.HasValue && string.IsNullOrEmpty(Author))
            {
                return RedirectToAction("Index");
            }
            if (BlogCategoryId.HasValue)
            {
                blogs = blogs.Where(b => b.BlogCategoryId == BlogCategoryId.Value);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                blogs = blogs.Where(b => b.Title.Contains(Title));
            }

            if (!string.IsNullOrEmpty(Author))
            {
                blogs = blogs.Where(b => b.Author.Contains(Author));
            }

            ViewBag.BlogCategories = _context.BlogCategories.ToList();
            ViewBag.Authors = _context.Blogs.Select(b => b.Author).Distinct().ToList();
            return View("Index",blogs.ToList());
        }
    } 
}
