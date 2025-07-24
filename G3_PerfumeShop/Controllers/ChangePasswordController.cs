using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace G3_PerfumeShop.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;

        public ChangePasswordController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        private bool IsUserLoggedIn()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId != null;
        }
        public IActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            // Lấy thông tin UserId từ session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Kiểm tra nếu session đã hết hạn hoặc người dùng chưa đăng nhập
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Tìm người dùng trong cơ sở dữ liệu dựa trên UserId
            var user = _context.Users.SingleOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View("Index", model);  // Giữ người dùng ở trang ChangePassword với lỗi hiển thị
            }

            // Kiểm tra mật khẩu cũ có đúng không
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View("Index", model);  // Trả về view với lỗi sai mật khẩu cũ
            }

            // Kiểm tra xem mật khẩu mới và xác nhận mật khẩu có khớp không
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirmation password do not match.");
                return View("Index", model);  // Giữ người dùng ở trang ChangePassword
            }

            // Cập nhật mật khẩu mới
            user.Password = model.NewPassword;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Thông báo thành công
            ViewBag.SuccessMessage = "Password changed successfully.";

            return View("Index", model);  // Trả về view sau khi cập nhật thành công
        }
    }
}
