using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace G3_PerfumeShop.Controllers
{
    public class SettingListController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        // Đối tượng DbContext để truy cập và thao tác với cơ sở dữ liệu

        private const int DefaultPageSize = 10;
        // Hằng số dùng để xác định kích thước mặc định của trang (số lượng bản ghi trên mỗi trang)

        public SettingListController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
            // Khởi tạo đối tượng _context bằng cách truyền vào DbContext để có thể dùng cho các thao tác cơ sở dữ liệu
        }

        public IActionResult Index(int page = 1, int pageSize = DefaultPageSize, string searchTerm = null)
        {

            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (roleId != 1 || userId == null) // Kiểm tra quyền truy cập
            {
                return RedirectToAction("UnauthorizedAccess", "Home"); // Chuyển hướng tới UnauthorizedAccess view
            }


            var users = _context.Users
                .Include(u => u.Status) // Bao gồm thông tin trạng thái của người dùng trong kết quả truy vấn
                .Include(u => u.Role) // Bao gồm thông tin vai trò của người dùng
                .AsQueryable(); // Chuyển thành truy vấn có thể lọc và phân trang

            // Tìm kiếm người dùng theo tên nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Lọc người dùng có tên hoặc họ chứa chuỗi tìm kiếm
                users = users.Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm));
            }

            // Phân trang
            ViewBag.CurrentPage = page; // Gán số trang hiện tại vào ViewBag để sử dụng trong view
            ViewBag.PageSize = pageSize; // Gán kích thước trang vào ViewBag để sử dụng trong view
            ViewBag.TotalPages = (int)Math.Ceiling((double)users.Count() / pageSize);
            // Tính tổng số trang dựa trên số lượng người dùng và kích thước trang

            var paginatedUsers = users
                .Skip((page - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước đó
                .Take(pageSize) // Lấy số lượng bản ghi cho trang hiện tại
                .ToList(); // Chuyển truy vấn thành danh sách

            // Truyền từ khóa tìm kiếm vào ViewBag để hiển thị lại trên giao diện tìm kiếm
            ViewBag.SearchTerm = searchTerm;

            return View(paginatedUsers); // Trả về view với danh sách người dùng đã phân trang
        }

        [HttpPost]
        public IActionResult ChangePageSize(int change)
        {
            // Lấy kích thước trang hiện tại từ form (Request.Form["pageSize"]),
            // nếu không có thì sử dụng DefaultPageSize làm mặc định
            var currentPageSize = int.TryParse(Request.Form["pageSize"], out int size) ? size : DefaultPageSize;

            // Tính kích thước trang mới bằng cách cộng thêm giá trị change
            var newPageSize = currentPageSize + change;

            // Đảm bảo kích thước trang không nhỏ hơn 1
            if (newPageSize < 1) newPageSize = 1;

            // Chuyển hướng trở lại trang Index với kích thước trang mới và trang hiện tại
            return RedirectToAction("Index", new { page = ViewBag.CurrentPage, pageSize = newPageSize });
        }

    }
}
