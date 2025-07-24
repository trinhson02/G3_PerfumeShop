using Microsoft.AspNetCore.Mvc; // Cung cấp các lớp và phương thức để xây dựng các controller MVC
using G3_PerfumeShop.Models;    // Thư viện cho phép kết nối và thao tác với các model trong dự án, ví dụ như Blog, BlogCategory

// BlogController: Controller này quản lý các chức năng liên quan đến blog trong dự án G3_PerfumeShop
public class BlogController : Controller
{
    // Tạo một instance của G3_PerfumeShopDB_Iter3Context để kết nối với cơ sở dữ liệu
    // Context này giúp truy xuất và lưu trữ dữ liệu liên quan đến các bảng Blogs và BlogCategories
    private readonly G3_PerfumeShopDB_Iter3Context _context = new G3_PerfumeShopDB_Iter3Context();

    // Action BlogList: Hiển thị danh sách blog với khả năng tìm kiếm và phân trang
    public ActionResult BlogList(string search, int? page)
    {
        int pageSize = 9;       // pageSize xác định số lượng blog hiển thị trên mỗi trang
        int pageNumber = page ?? 1;  // Xác định số trang hiện tại (mặc định là 1 nếu chưa được chọn)

        // Đếm tổng số blog dựa trên từ khóa tìm kiếm (nếu có) để tính tổng số trang
        var totalPosts = _context.Blogs
            .Where(b => string.IsNullOrEmpty(search) || b.Title.Contains(search) || b.BlogContent.Contains(search))
            .Count();

        // Truy vấn danh sách blog, áp dụng bộ lọc tìm kiếm và phân trang
        var blogs = _context.Blogs
            .Where(b => string.IsNullOrEmpty(search) || b.Title.Contains(search) || b.BlogContent.Contains(search))
            .OrderByDescending(b => b.UpdatedAt)  // Sắp xếp các bài viết theo thứ tự giảm dần của thời gian cập nhật
            .Skip((pageNumber - 1) * pageSize)    // Bỏ qua các bài viết của các trang trước (dựa trên số trang hiện tại)
            .Take(pageSize)                       // Lấy số lượng bài viết giới hạn theo kích thước trang
            .ToList(); 

        // Tính tổng số trang và lưu vào ViewBag để sử dụng trong view
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize); // Tính tổng số trang và làm tròn lên
        ViewBag.CurrentPage = pageNumber; // Lưu lại trang hiện tại vào ViewBag để hiển thị

        // Lấy tất cả các danh mục blog và lưu vào ViewBag.Categories để hiển thị trong view
        ViewBag.Categories = _context.BlogCategories.ToList();

        // Lấy 5 bài viết mới nhất để hiển thị danh sách các bài viết gần đây
        ViewBag.LatestPosts = _context.Blogs.OrderByDescending(b => b.UpdatedAt).Take(5).ToList();

        // Trả về view "BlogList" cùng với danh sách blog lấy được
        return View("BlogList", blogs);
    }

    // Action Details: Hiển thị chi tiết của một bài viết blog
    public ActionResult Details(int id)
    {
        // Truy vấn một bài viết theo id được truyền vào
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);

        // Nếu bài viết không tồn tại, trả về NotFound để báo lỗi
        if (blog == null)
        {
            return NotFound();
        }

        // Trả về view Details cùng với dữ liệu bài viết
        return View("Details",blog);
    }
}
