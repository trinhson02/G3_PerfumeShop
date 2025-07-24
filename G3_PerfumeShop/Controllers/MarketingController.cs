using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace G3_PerfumeShop.Controllers
{
    public class MarketingController : Controller
    {

        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private JsonSerializerOptions jsonOpt;
        public MarketingController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
            jsonOpt = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles, // Sử dụng Preserve để xử lý vòng lặp
                MaxDepth = 32, // Bạn có thể tăng giá trị MaxDepth tùy theo yêu cầu
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
        }

        public IActionResult Completion()
        {
            return View();
        }

        public IActionResult Index()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            //if (!(roleId == 1 || roleId == 2)) trường hợp set cả role admin và manager
            if (roleId != 2)
            {
                // Chưa đăng nhập!
                return RedirectToAction("Index", "Login");
            }
            else
            {
                User u = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
                ViewBag.User = u;
          
                return View();
            }
        }


        public JsonResult GetBlogCount(string? code)
        {
            int blogCount = _context.Blogs.Count();

            var result = new
            {
                blogCount = blogCount
            };

            if (code is null)
            {
                return Json(result);
            }

            switch (code)
            {
                case "CXB_LAST7D": //chờ xuất bản (ID=2) 7 ngày gần đây
                    {
                        blogCount = _context.Blogs.Where(b => b.BlogStatusId == 2 && b.UpdatedAt >= DateTime.Now.AddDays(7)).Count();
                        break;
                    }
            }

            return Json(result);
        }

        public JsonResult GetBlogCountWeek(DateTime? anchorDate)
        {
            DateTime selectDate = DateTime.Now;
            
            if (anchorDate.HasValue)
            {
                selectDate = (DateTime) anchorDate;
            }
            DayOfWeek selectDayOfWeek = selectDate.DayOfWeek;
            DateTime start = selectDate.AddDays(-(int)selectDayOfWeek);
            DateTime end = start.AddDays(7);


            var weekHolder = new List<object>();

            var selectWeek = _context.Blogs.Where(b => b.CreatedAt >= start && b.CreatedAt <= end).AsEnumerable();
            for (int i = (int) DayOfWeek.Sunday; i<= (int) DayOfWeek.Saturday; i++)
            {
                weekHolder.Add(new { DayOfWeek = i, Count = selectWeek.Count(w => w.CreatedAt.DayOfWeek == (DayOfWeek) i) });
            }
            return Json(weekHolder, jsonOpt);
        }

        public JsonResult GetBlogCountMonth(DateTime? anchorDate)
        {
            DateTime selectDate = DateTime.Now;

            if (anchorDate.HasValue)
            {
                selectDate = (DateTime)anchorDate;
            }
            int selectDayInMonth = DateTime.DaysInMonth(selectDate.Year, selectDate.Month);
            DateTime start = new DateTime(selectDate.Year, selectDate.Month, 1);
            DateTime end = new DateTime(selectDate.Year, selectDate.Month, selectDayInMonth);


            var monthHolder = new List<object>();

            var selectMonth = _context.Blogs.Where(b => b.CreatedAt >= start && b.CreatedAt <= end).AsEnumerable();
            for (int i = 1; i <= selectDayInMonth; i++)
            {
                monthHolder.Add(new { Day = i, Count = selectMonth.Count(w => w.CreatedAt.Day == i) });
            }

            var result = new
            {
                DayInMonth = selectDayInMonth,
                Holder = monthHolder
            };

            return Json(result, jsonOpt);
        }

        public JsonResult GetParam()
        {
            var x = _context.DashboardSections.Include(d => d.FilterDashboardParameters).Where(d => d.Name == "Blog" && d.FilterDashboardParameters.Any(f => f.DashboardSectionId == 1));

            return Json(x, jsonOpt);
        }

        public JsonResult GetBlogDashboard()
        {
            int blogCount = _context.Blogs.Count();

            var result = new
            {
                blogCount = blogCount
            };



            return Json(result, jsonOpt);
        }

        public async Task<JsonResult> GetFilteredData(int n)
        {
            //return Json(_context.Blogs.ToList().Take(n));
            var mockProduct = new List<object>
            {
                new {Date = DateTime.Now, Name = "Lâm", Price = 5000},
                new {Date = DateTime.Now, Name = "Lam", Price = 80},
                new {Date = DateTime.Now, Name = "Tuan", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "Trường", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "VN", Price = 50},
                new {Date = DateTime.Now, Name = "US", Price = 80},
                new {Date = DateTime.Now, Name = "US", Price = 80}
            };
            var x = mockProduct.Take(n);

            var y = new
            {
                listProduct = x,
                pagingNum = (int)Math.Ceiling((double)mockProduct.Count()/n),
                countProduct = mockProduct.Count()
            };
            return Json(y);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa session khi người dùng đăng xuất
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
