using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace G3_PerfumeShop.Controllers
{
    public class RegisterController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context db = new G3_PerfumeShopDB_Iter3Context(); // Tạo DbContext để kết nối cơ sở dữ liệu

        [HttpGet]
        // Phương thức GET Index để hiển thị trang đăng ký
        public ActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa bằng cách kiểm tra session UserId
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Nếu đã đăng nhập, chuyển hướng về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // Nếu chưa đăng nhập, hiển thị trang đăng ký
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ chống lại các cuộc tấn công CSRF
        public ActionResult Index(UserViewModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu từ model
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email hoặc số điện thoại đã tồn tại trong hệ thống
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email || u.Phone == model.Phone);
                if (existingUser != null)
                {
                    // Nếu email hoặc số điện thoại đã tồn tại, thêm lỗi vào ModelState và hiển thị lại view
                    ModelState.AddModelError("", "Email hoặc số điện thoại đã được đăng ký.");
                    return View(model);
                }

                // Tạo đối tượng User mới với thông tin từ model
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    Birthdate = model.Birthdate,
                    Password = model.Password,  // Mật khẩu chưa được mã hóa, cần mã hóa để bảo mật
                    StatusId = 1, // Đặt trạng thái mặc định là active
                    RoleId = 4,   // Đặt vai trò mặc định là customer
                    Username = model.Email // Giả sử username sẽ là email
                };

                // Thêm người dùng vào cơ sở dữ liệu
                db.Users.Add(user);
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Thông báo thành công và điều hướng đến trang đăng nhập
                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập.";
                return RedirectToAction("Index", "Login");
            }

            // Nếu model không hợp lệ, hiển thị lại view với thông tin đã nhập
            return View(model);
        }

    }
}
