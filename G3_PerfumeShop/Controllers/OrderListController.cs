using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace  G3_PerfumeShop.Controllers
{
    public class OrderListController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        private const int DefaultPageSize = 5;

        public OrderListController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = DefaultPageSize,
        DateTime? fromDate = null, DateTime? toDate = null, string status = null, string searchTerm = null)
        {

            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (roleId != 3 || userId == null) // Kiểm tra quyền truy cập
            {
                return RedirectToAction("UnauthorizedAccess", "Home"); // Chuyển hướng tới UnauthorizedAccess view
            }



            // Lấy danh sách đơn hàng từ cơ sở dữ liệu, bao gồm thông tin người dùng và chi tiết sản phẩm của từng đơn hàng
            var orders = _context.Orders
                .Include(o => o.User) // Bao gồm thông tin của người dùng trong đơn hàng
                .Include(o => o.OrderDetails) // Bao gồm chi tiết đơn hàng
                    .ThenInclude(od => od.ProductSizePricing) // Bao gồm thông tin về giá kích thước sản phẩm trong chi tiết đơn hàng
                        .ThenInclude(psp => psp.Product) // Bao gồm thông tin về sản phẩm trong giá kích thước sản phẩm
                .AsQueryable(); // Chuyển thành truy vấn có thể lọc và sắp xếp

            // Chỉ lọc theo ngày nếu cả fromDate và toDate đều có giá trị
            if (fromDate.HasValue && toDate.HasValue)
            {
                // Lọc các đơn hàng có ngày tạo nằm trong khoảng từ fromDate đến toDate
                orders = orders.Where(o => o.CreatedAt >= fromDate.Value && o.CreatedAt <= toDate.Value);
            }

            // Lọc theo trạng thái của đơn hàng nếu có giá trị
            if (!string.IsNullOrEmpty(status))
            {
                // Chuyển đổi trạng thái thành giá trị boolean để lọc
                bool isCompleted = status == "completed";
                // Lọc các đơn hàng có trạng thái khớp với trạng thái được chọn
                orders = orders.Where(o => o.Status == isCompleted);
            }

            // Lọc theo tên người dùng nếu có giá trị tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Lọc các đơn hàng mà tên người dùng hoặc họ người dùng chứa chuỗi tìm kiếm
                orders = orders.Where(o => o.User.FirstName.Contains(searchTerm) || o.User.LastName.Contains(searchTerm));
            }

            // Thiết lập thông tin phân trang
            ViewBag.CurrentPage = page; // Trang hiện tại
            ViewBag.PageSize = pageSize; // Số lượng đơn hàng trên mỗi trang
            ViewBag.TotalPages = (int)Math.Ceiling((double)orders.Count() / pageSize); // Tổng số trang dựa trên tổng số đơn hàng

            // Lấy danh sách đơn hàng với phân trang, bỏ qua các đơn hàng trước trang hiện tại và chỉ lấy số lượng theo pageSize
            var paginatedOrders = orders
                .Skip((page - 1) * pageSize) // Bỏ qua các đơn hàng của các trang trước
                .Take(pageSize) // Lấy số lượng đơn hàng cho trang hiện tại
                .ToList(); // Chuyển truy vấn thành danh sách

            // Truyền các thông tin lọc cho View để hiển thị lại trên giao diện
            ViewBag.FromDate = fromDate; // Ngày bắt đầu lọc
            ViewBag.ToDate = toDate; // Ngày kết thúc lọc
            ViewBag.Status = status; // Trạng thái đơn hàng
            ViewBag.SearchTerm = searchTerm; // Từ khóa tìm kiếm

            return View(paginatedOrders); // Trả về View với danh sách đơn hàng đã phân trang
        }
        [HttpPost]
        public IActionResult ChangePageSize(int change)
        {
            // Lấy kích thước trang hiện tại từ form (Request.Form["pageSize"]),
            // nếu không có thì sử dụng DefaultPageSize làm mặc định
            var currentPageSize = int.TryParse(Request.Form["pageSize"], out int size) ? size : DefaultPageSize;

            // Tính toán kích thước trang mới bằng cách cộng thêm giá trị change
            var newPageSize = currentPageSize + change;

            // Đảm bảo kích thước trang (pageSize) không nhỏ hơn 1
            if (newPageSize < 1) newPageSize = 1;

            // Chuyển hướng trở lại trang Index với kích thước trang mới và trang hiện tại
            return RedirectToAction("Index", new { page = ViewBag.CurrentPage, pageSize = newPageSize });
        }

    }
}
