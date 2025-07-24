using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace G3_PerfumeShop.Controllers
{
    public class PostAddController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        public PostAddController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddPost()
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
            // Lấy danh sách BlogCategory
            var blogCategories = _context.BlogCategories.ToList();
            // Truyền danh sách vào ViewBag
            ViewBag.BlogCategories = blogCategories;
            
            return View("Index");
        }
        [HttpPost]
        public IActionResult PostAdd(Blog model)
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

            if (ModelState.IsValid)
            {
                // Tạo bài viết mới
                var newBlog = new Blog
                {
                    BlogCategoryId = model.BlogCategoryId,
                    Title = model.Title,
                    BlogContent = model.BlogContent,
                    CreatedAt = DateTime.Now, 
                    UpdatedAt = DateTime.Now, 
                    ImageUrl = model.ImageUrl,
                    Author = model.Author,
                    BlogStatusId = model.BlogStatusId
                };

                _context.Blogs.Add(newBlog);
                _context.SaveChanges();  

                return RedirectToAction("Index", "MyPosts");
            }

            // Nếu ModelState không hợp lệ, trả lại view với các dữ liệu hiện tại
            ViewBag.BlogCategories = _context.BlogCategories.ToList();
            ViewBag.BlogStatuses = _context.BlogStatuses.ToList();
            return View("Index",model);
        }
    }
}
