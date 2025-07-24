using Microsoft.AspNetCore.Mvc;// Thư viện giúp xây dựng các controller trong ứng dụng ASP.NET Core MVC.
using G3_PerfumeShop.Models;// Chứa các model dữ liệu của ứng dụng, giúp biểu diễn các bảng trong cơ sở dữ liệu.
using Microsoft.EntityFrameworkCore;// Thư viện hỗ trợ truy vấn và thao tác với cơ sở dữ liệu thông qua Entity Framework Core.
using G3_PerfumeShop.Service;// Sử dụng dịch vụ S3 để lưu trữ file, upload file lên đám mây (S3Service).
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using G3_PerfumeShop.ViewModels;
//using G3_PerfumeShop.ViewModels;

namespace G3_PerfumeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;  // Biến `_context` dùng để kết nối tới cơ sở dữ liệu của ứng dụng.
        private readonly S3Service _s3Service;  // Biến `_s3Service` được sử dụng để tương tác với dịch vụ lưu trữ file trên S3.
        private JsonSerializerOptions jsonOpt;  // Cài đặt tùy chỉnh cho việc serialize JSON.

        public ProductController(G3_PerfumeShopDB_Iter3Context context, S3Service s3Service)
        {
            _context = context; // Gán đối tượng kết nối cơ sở dữ liệu.
            _s3Service = s3Service; // Gán đối tượng dịch vụ S3.

            // Cấu hình cho đối tượng `jsonOpt` để tùy chỉnh việc chuyển đổi dữ liệu sang JSON:
            jsonOpt = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles, // Bỏ qua các vòng lặp trong mối quan hệ giữa các đối tượng.
                MaxDepth = 32, // Giới hạn độ sâu tối đa để tránh lỗi vòng lặp vô hạn.
                WriteIndented = true, // Format JSON với thụt lề cho dễ đọc.
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // Hỗ trợ mã hóa tất cả ký tự Unicode.
            };
        }

        // Action method `Index` trả về giao diện mặc định cho controller..
        public async Task<IActionResult> Index()
        {
            return View(); // Trả về view mặc định của `Index`.
        }


        public async Task<JsonResult> GetProductList(string? keyword, int? size, int? page, string? brand, float? priceFrom, float? priceTo, float? capFrom, float? capTo, string? countryCode)
        {
            // Kiểm tra và xử lý giá trị của tham số `page`.
            if (page.HasValue)
            {
                if (page < 0) // Nếu `page` nhỏ hơn 0, đặt lại giá trị mặc định là 1.
                {
                    page = 1;
                }
            }
            // Kiểm tra và xử lý giá trị của tham số `size`.
            if (size.HasValue)
            {
                if (size < 0) // Nếu `size` nhỏ hơn 0, đặt lại giá trị mặc định là 10.
                {
                    size = 10;
                }
            }

            // Khởi tạo truy vấn sản phẩm bao gồm các bảng liên quan bằng cách sử dụng `Include`.
            IQueryable<Product> products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Origin)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductSizePricings);

            // Lọc theo mã quốc gia (countryCode).
            if (countryCode is not null)
            {
                if (countryCode.Length > 0) // Kiểm tra mã quốc gia có giá trị không.
                {
                    products = products.Where(p => p.Origin.CountryCode.Equals(countryCode));
                }
            }

            // Lọc theo thương hiệu (brand).
            if (brand is not null)
            {
                if (brand.Length > 0) // Kiểm tra tên thương hiệu có giá trị không.
                {
                    products = products.Where(p => p.Brand.Name.Equals(brand));
                }
            }

            // Lọc theo từ khóa (keyword) trong tên sản phẩm.
            if (keyword is not null)
            {
                products = products.Where(p => p.Name.Contains(keyword));
            }

            // Các cờ để kiểm tra xem có sắp xếp theo giá hoặc dung tích không.
            bool isAscPrice = false;
            bool isAscCap = false;

            // Lọc theo giá từ (priceFrom).
            if (priceFrom.HasValue)
            {
                products = products.Where(p => p.ProductSizePricings.Any(ps => priceFrom <= (float)ps.Price));
                isAscPrice = true; // Đặt cờ sắp xếp theo giá.
            }

            // Lọc theo giá đến (priceTo).
            if (priceTo.HasValue)
            {
                products = products.Where(p => p.ProductSizePricings.Any(ps => priceTo >= (float)ps.Price));
                isAscPrice = true; // Đặt cờ sắp xếp theo giá.
            }

            // Lọc theo dung tích từ (capFrom).
            if (capFrom.HasValue)
            {
                products = products.Where(p => p.ProductSizePricings.Any(ps => capFrom <= (float)ps.Size));
                isAscCap = true; // Đặt cờ sắp xếp theo dung tích.
            }

            // Lọc theo dung tích đến (capTo).
            if (capTo.HasValue)
            {
                products = products.Where(p => p.ProductSizePricings.Any(ps => capTo >= (float)ps.Size));
                isAscCap = true; // Đặt cờ sắp xếp theo dung tích.
            }

            // Sắp xếp theo giá nếu có lọc giá.
            if (isAscPrice)
            {
                products = products.OrderBy(p => p.ProductSizePricings.OrderBy(psp => psp.Price).FirstOrDefault().Price);
            }

            // Sắp xếp theo dung tích nếu có lọc dung tích.
            if (isAscCap)
            {
                products = products.OrderBy(p => p.ProductSizePricings.OrderBy(psp => psp.Size).FirstOrDefault().Size);
            }

            // Phân trang
            int resultNum = products.Count(); // Tổng số sản phẩm tìm được dựa trên bộ lọc.
            int pagingNum = (int)Math.Ceiling((decimal)resultNum / (size ?? 10)); // Tổng số trang được phân.
            products = products.Skip((page - 1) * (size ?? 10) ?? 0).Take(size ?? 10); // Lấy sản phẩm của trang hiện tại.

            // Đóng gói dữ liệu trả về bao gồm số lượng kết quả, số trang, và danh sách sản phẩm.
            var productListWrapper = new
            {
                resultNum, // Tổng số kết quả.
                pagingNum, // Số trang.
                products   // Danh sách sản phẩm.
            };

            // Trả về dữ liệu dưới dạng JSON với cấu hình `jsonOpt`.
            return Json(productListWrapper, jsonOpt);
        }


        public async Task<JsonResult> GetFilterDetail()
        {
            var filterWrapper = new
            {
                origins = await _context.Origins.ToListAsync(),
                brands = await _context.Brands.ToListAsync()
            };
            return Json(filterWrapper, jsonOpt);
        }
        // Hàm `ProductDetail` để hiển thị chi tiết sản phẩm dựa trên `id` sản phẩm.




        [HttpGet]
        public async Task<IActionResult> ProductDetail(int id, string query, int page = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");// Lấy `UserId` từ session của người dùng đang đăng nhập.
            ViewBag.UserId = userId;
            if (userId.HasValue)  // Kiểm tra xem người dùng đã từng đặt hàng sản phẩm này chưa.
            {
                var hasOrder = await _context.Orders
                    .Include(o => o.OrderDetails) // Bao gồm các chi tiết đơn hàng.
                    .AnyAsync(o => o.UserId == userId.Value &&
                                   o.OrderDetails.Any(od => od.ProductSizePricing.ProductId == id));
                ViewBag.HasOrder = hasOrder; // Cập nhật trạng thái `HasOrder` cho ViewBag.
            }
            else
            {
                ViewBag.HasOrder = false;
            }
            // Lấy sản phẩm theo `id`, bao gồm các thông tin thương hiệu, danh mục sản phẩm, bảng giá kích thước, giới tính và nguồn gốc.
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductSizePricings)
                .Include(p => p.Gender)
                .Include(p => p.Origin)
                .FirstOrDefaultAsync(p => p.Id == id);
            // Kiểm tra sản phẩm có tồn tại hay không. Nếu không, trả về NotFound.
            if (product == null)
            {
                return NotFound();
            }
            // Lấy danh sách phản hồi cho sản phẩm, phân trang với `page`.
            var feedbacks = await GetFeedbacksForProduct(id, page);
            ViewBag.Feedbacks = feedbacks;
            ViewBag.Page = page;

            // Tính điểm đánh giá trung bình, nếu có phản hồi thì tính điểm trung bình, nếu không thì mặc định là 0.
            ViewBag.AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
            // Đếm tổng số phản hồi liên quan đến sản phẩm.
            var totalFeedbacks = await _context.Feedbacks.CountAsync(f => f.ProductId == id);
            ViewBag.TotalFeedbacks = totalFeedbacks;
            // Tính tổng số trang, mỗi trang chứa 3 phản hồi.
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalFeedbacks / 3);
            // Đếm số phản hồi cho từng mức đánh giá (từ 1 đến 5 sao) và tính tỷ lệ phần trăm.
            var ratingCounts = new int[5];
            foreach (var feedback in feedbacks)
            {
                if (feedback.Rating >= 1 && feedback.Rating <= 5)
                {
                    ratingCounts[feedback.Rating - 1]++;
                }
            }
            ViewBag.RatingPercentages = ratingCounts.Select(count => (count * 100.0) / ViewBag.TotalFeedbacks).ToArray();
            // Lấy danh sách các danh mục sản phẩm và truyền vào view.
            var categories = await _context.ProductCategories.ToListAsync();
            ViewBag.Categories = categories;
            // Lấy 5 sản phẩm mới nhất để hiển thị trong phần "Latest Products".
            var latestProducts = await _context.Products
                .OrderByDescending(p => p.ReleaseYear)
                .Take(5)
                .ToListAsync();
            ViewBag.LatestProducts = latestProducts;
            // Tìm kiếm sản phẩm liên quan theo tên dựa vào `query`.
            IEnumerable<Product> relatedProducts;
            if (!string.IsNullOrWhiteSpace(query))
            {
                relatedProducts = await _context.Products
                    .Where(p => p.Name.Contains(query))
                    .Include(p => p.Brand)
                    .ToListAsync();
            }
            else
            {
                relatedProducts = new List<Product>();
            }
            ViewBag.FeedbackResponses = await GetFeedbackResponses(feedbacks);
            // Truyền `query` vào ViewBag để hiển thị lại khi người dùng tìm kiếm.
            ViewBag.SearchQuery = query;
            // Tạo model kết hợp sản phẩm hiện tại và sản phẩm liên quan để hiển thị trong view.
            var model = new Tuple<Product, IEnumerable<Product>>(product, relatedProducts);
            return View(model);
        }
        // Hàm `GetFeedbacksForProduct` lấy danh sách feedback cho sản phẩm dựa vào `productId` và trang.



        private async Task<IEnumerable<Feedback>> GetFeedbacksForProduct(int productId, int page = 1, int pageSize = 3)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Sources)
                .Where(f => f.ProductId == productId) // Lọc theo ProductId
                .Skip((page - 1) * pageSize) // Bỏ qua các feedback của trang trước.
                .Take(pageSize) // Lấy số lượng feedback cần thiết cho trang hiện tại.
                .ToListAsync();
        }

        private async Task<IEnumerable<FeedbackResponse>> GetFeedbackResponses(IEnumerable<Feedback> feedbacks)
        {
            var feedbackIds = feedbacks.Select(f => f.Id).ToList();
            return await _context.FeedbackResponses
                .Where(fr => feedbackIds.Contains(fr.FeedbackId))
                .Include(fr => fr.User) // Bao gồm thông tin người dùng đã phản hồi
                .Include(fr => fr.SourcesFeedbackRespones)
                .ToListAsync();
        }
        // Phương thức `SubmitFeedback` xử lý phản hồi từ người dùng.
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(int productId, int rating, string content, List<IFormFile> files, List<string> captions)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Forbid();
            }
            // Kiểm tra xem người dùng có đơn hàng nào không, nếu không thì chặn.
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == userId.Value);
            if (!hasOrders)
            {
                return Forbid();
            }
            // Tạo đối tượng `feedback` mới từ các thông tin phản hồi.
            var feedback = new Feedback
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now,
                Content = content,
                Rating = rating,
                ProductId = productId,
                OrderId = await GetLatestOrderId(userId.Value, productId)
            };
            _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
            // Upload file và thêm nguồn (Source) với chú thích cho từng ảnh/video.
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    var uploadUrl = await _s3Service.UploadFileAsync(file); // Upload file lên S3.
                    var source = new Source
                    {
                        Url = uploadUrl,
                        Caption = captions[i], // chú thích của ảnh/video tương ứng
                        FeedbackId = feedback.Id
                    };
                    _context.Sources.Add(source);
                }
            }
            await _context.SaveChangesAsync();
            // Chuyển hướng về trang `ProductDetail` sau khi gửi phản hồi.
            return RedirectToAction("ProductDetail", "Product", new { id = productId });
        }
        // Hàm `GetLatestOrderId` để lấy ID của đơn hàng mới nhất của người dùng.
        private async Task<int> GetLatestOrderId(int userId, int productId)
        {
            // Lấy chi tiết đơn hàng có chứa sản phẩm với `productId` và người dùng `userId`.
            var latestOrderDetail = await _context.OrderDetails
                 .Include(od => od.Order)
                .Where(od => od.Order.UserId == userId && od.ProductSizePricing.ProductId == productId)
                .OrderByDescending(od => od.Order.CreatedAt)
                .FirstOrDefaultAsync();

            // Trả về `OrderId` nếu tìm thấy, nếu không thì trả về 0.
            return latestOrderDetail.OrderId;
        }

    }
}
