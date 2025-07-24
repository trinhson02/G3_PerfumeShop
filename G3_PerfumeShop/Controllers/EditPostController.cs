using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace G3_PerfumeShop.Controllers
{
    public class EditPostController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        public EditPostController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult EditPost(int id)
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
            var blogStatues = _context.BlogStatuses.ToList();
            ViewBag.BlogCategories = blogCategories;
            ViewBag.BlogStatuses = blogStatues;
            var blog = _context.Blogs
            .Include(b => b.BlogCategory)
            .Include(b => b.BlogStatus)
            .SingleOrDefault(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }
            return View("Index" , blog);
        }
        [HttpPost]
        public IActionResult EditPost(Blog model , int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var blog = _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.BlogStatus)
                .SingleOrDefault(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            blog.BlogCategoryId = model.BlogCategoryId;
            blog.Title = model.Title;
            blog.BlogContent = model.BlogContent;
            blog.CreatedAt = model.CreatedAt; 
            blog.ImageUrl = model.ImageUrl;
            blog.Author = model.Author;
            blog.UpdatedAt = DateTime.Now; 
            blog.BlogStatusId = model.BlogStatusId;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                return View("Index", model); 
            }

            return RedirectToAction("Index", "PostManagement");
        }
    }
}
