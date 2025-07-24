using G3_PerfumeShop.Models; // Import model từ dự án G3_PerfumeShop để làm việc với dữ liệu
using G3_PerfumeShop.Service;
using Microsoft.AspNetCore.Mvc; // Thư viện ASP.NET MVC cho phép tạo các controller và action
using Microsoft.EntityFrameworkCore; // Thư viện hỗ trợ kết nối cơ sở dữ liệu qua Entity Framework

namespace G3_PerfumeShop.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context; // Kết nối với cơ sở dữ liệu
        public FeedbackController(G3_PerfumeShopDB_Iter3Context context) // Constructor để khởi tạo context
        {
            _context = context;
        }

        // Phương thức hiển thị danh sách Feedbacks với các tùy chọn tìm kiếm, sắp xếp và phân trang
        public IActionResult FeedbackList(string searchTerm, int? rating, bool? status, string sortOrder, string[] columns, int? rowCount = null, int page = 1, int pageSize = 5)
        {
            // Lấy RoleId và UserId từ session để kiểm tra quyền truy cập
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId == 3 || roleId == 4 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            // Khởi tạo truy vấn
            var query = _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Order)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductSizePricing)
                            .ThenInclude(psp => psp.Product)
                .AsQueryable();

            // Bộ lọc tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f =>
                    f.User.FirstName.Contains(searchTerm) ||
                    f.User.LastName.Contains(searchTerm) ||
                    f.Content.Contains(searchTerm) ||
                    f.Order.OrderDetails.Any(od => od.ProductSizePricing.Product.Name.Contains(searchTerm)));
            }

            // Bộ lọc rating
            if (rating.HasValue)
            {
                query = query.Where(f => f.Rating == rating.Value);
            }

            // Bộ lọc trạng thái
            if (status.HasValue)
            {
                query = query.Where(f => f.Status == status.Value);
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "username_asc":
                    query = query.OrderBy(f => f.User.FirstName).ThenBy(f => f.User.LastName);
                    break;
                case "username_desc":
                    query = query.OrderByDescending(f => f.User.FirstName).ThenByDescending(f => f.User.LastName);
                    break;
                case "productname_asc":
                    query = query.OrderBy(f => f.Order.OrderDetails.First().ProductSizePricing.Product.Name);
                    break;
                case "productname_desc":
                    query = query.OrderByDescending(f => f.Order.OrderDetails.First().ProductSizePricing.Product.Name);
                    break;
                case "rating_asc":
                    query = query.OrderBy(f => f.Rating);
                    break;
                case "rating_desc":
                    query = query.OrderByDescending(f => f.Rating);
                    break;
                case "status_asc":
                    query = query.OrderBy(f => f.Status);
                    break;
                case "status_desc":
                    query = query.OrderByDescending(f => f.Status);
                    break;
                default:
                    query = query.OrderBy(f => f.Id);
                    break;
            }

            // Thiết lập `pageSize` dựa trên `rowCount`
            pageSize = rowCount ?? pageSize;
            var totalItems = query.Count();

            // Lấy danh sách feedback với phân trang
            var feedbacks = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Thiết lập các cột động mặc định nếu `columns` chưa được chỉ định
            if (columns == null || columns.Length == 0)
            {
                columns = new[] { "ID", "UserName", "ProductName", "Rating", "Status" };
            }

            // Tạo view model và truyền dữ liệu cho view
            var model = new FeedbackListViewModel
            {
                Feedbacks = feedbacks,
                SearchTerm = searchTerm,
                Rating = rating,
                Status = status,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                DynamicColumns = columns.ToList()
            };

            return View(model);
        }

        // Phương thức POST để thay đổi trạng thái của feedback
        [HttpPost]
        public IActionResult ChangeFeedbackStatus(int id, bool status)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == id); // Tìm feedback theo ID
            if (feedback == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy feedback
            }

            feedback.Status = status; // Cập nhật trạng thái của feedback
            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            return RedirectToAction("FeedbackList"); // Chuyển hướng về danh sách feedback
        }

        // Phương thức hiển thị chi tiết của một feedback
        public IActionResult FeedbackDetail(int id)
        {
            var feedback = _context.Feedbacks
                .Include(f => f.User) // Bao gồm thông tin của User
                .Include(f => f.Order)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.ProductSizePricing)
                            .ThenInclude(psp => psp.Product) // Bao gồm tên sản phẩm
                .Include(f => f.Sources) // Bao gồm các nguồn ảnh/video
                .FirstOrDefault(f => f.Id == id); // Tìm feedback theo ID

            if (feedback == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy feedback
            }

            return View(feedback); // Trả về view với chi tiết feedback
        }
        [HttpPost]
        public async Task<IActionResult> SendResponse(int feedbackId, string responseContent, List<IFormFile> files, string[] captions, [FromServices] S3Service s3Service)
        {
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.Id == feedbackId);
            if (feedback == null)
            {
                return NotFound();
            }

            // Tạo phản hồi mới
            var feedbackResponse = new FeedbackResponse
            {
                FeedbackId = feedbackId,
                ResponseContent = responseContent,
                ResponseDate = DateTime.Now,
                UserId = HttpContext.Session.GetInt32("UserId") ?? 0 // Lấy UserId từ session
            };

            _context.FeedbackResponses.Add(feedbackResponse);
            await _context.SaveChangesAsync();

            // Xử lý các file tải lên và lưu vào S3
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    var fileUrl = await s3Service.UploadFileAsync(file);
                    var source = new SourcesFeedbackRespone
                    {
                        Url = fileUrl,
                        FeedbackResponseId = feedbackResponse.Id,
                        Caption = captions[i] // Sử dụng caption từ input
                    };
                    _context.SourcesFeedbackRespones.Add(source);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("FeedbackDetail", new { id = feedbackId });
        }
    }
}
