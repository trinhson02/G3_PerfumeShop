using G3_PerfumeShop.Models;
using G3_PerfumeShop.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G3_PerfumeShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private readonly ISessionService _sessionService;
        public LoginController(G3_PerfumeShopDB_Iter3Context context,  ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Người dùng đã đăng nhập, chuyển hướng về trang chủ
                return RedirectToAction("Index", "Home");
            }
         

            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            // Kiểm tra xem username có tồn tại không
            var user = _context.Users.FirstOrDefault(u => u.Username == username || u.Email == username || u.Phone == username);


            if (user == null)
            {
                // Không tìm thấy người dùng
                ViewBag.ErrorMessage = "Người dùng không tồn tại.";
                return View();
            }

            // Kiểm tra mật khẩu
            if (user.Password != password)
            {
                // Mật khẩu không đúng
                ViewBag.ErrorMessage = "Mật khẩu không đúng.";
                return View();
            }
            if(user.StatusId == 2)
            {
                ViewBag.ErrorMessage = "Tài khoản đã bị cấm";
                return View();
            }

            // Gọi service để lưu session
            _sessionService.SetUserSession(user);


            // Điều hướng đến trang chính
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            _sessionService.ClearUserSession();  // Xóa session khi đăng xuất
            return RedirectToAction("Index", "Home");
        }
    }
}
