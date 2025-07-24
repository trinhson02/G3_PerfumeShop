using Microsoft.AspNetCore.Mvc;
using G3_PerfumeShop.Models;  // Your models namespace
using System.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using Amazon.S3.Model;
using Amazon;
using Microsoft.EntityFrameworkCore;
using G3_PerfumeShop.ViewModels;
using G3_PerfumeShop.Service;


namespace G3_PerfumeShop.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;  // Đối tượng DbContext cho phép truy cập và thao tác với cơ sở dữ liệu
        private readonly IConfiguration _configuration; // Đối tượng IConfiguration để truy cập các thiết lập cấu hình
        private readonly S3Service_Son _s3Service; // Dịch vụ S3 dùng để lưu trữ và quản lý ảnh trên Amazon S3

        // Constructor của OrderDetailController, nhận vào các dịch vụ S3Service_Son, DbContext, và IConfiguration
        public OrderDetailController(S3Service_Son s3Service, G3_PerfumeShopDB_Iter3Context context, IConfiguration configuration)
        {
            _s3Service = s3Service; // Gán dịch vụ S3Service_Son cho biến thành viên _s3Service
            _context = context; // Gán đối tượng DbContext cho biến thành viên _context
            _configuration = configuration; // Gán đối tượng IConfiguration cho biến thành viên _configuration
        }

        // Phương thức Index, hiển thị chi tiết đơn hàng dựa trên orderId
        public IActionResult Index(int orderId)
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


            // Truy vấn cơ sở dữ liệu để lấy thông tin đơn hàng dựa trên orderId
            var order = _context.Orders
                                .Where(o => o.Id == orderId) // Lọc đơn hàng có Id khớp với orderId
                                .Select(o => new OrderDetailViewModel
                                {
                                    OrderId = o.Id, // Gán Id của đơn hàng
                                    UserName = o.User.FirstName + " " + o.User.LastName, // Ghép họ và tên của người dùng
                                    Email = o.User.Email, // Email người dùng
                                    Phone = o.User.Phone, // Số điện thoại người dùng
                                    CreatedAt = o.CreatedAt, // Ngày tạo đơn hàng
                                    ShippingAddress = o.ShippingAddress, // Địa chỉ giao hàng của đơn hàng
                                    Status = o.Status, // Trạng thái đơn hàng
                                    Gender = o.User.Gender, // Giới tính của người dùng
                                    Address = o.User.Address, // Địa chỉ của người dùng
                                    Products = o.OrderDetails.Select(od => new ProductViewModel // Lấy danh sách sản phẩm từ chi tiết đơn hàng
                                    {
                                        Id = od.Id, // Id của sản phẩm trong đơn hàng
                                        ProductSizePricingId = od.ProductSizePricingId, // Id của kích thước sản phẩm
                                        Title = od.Title,  // Tiêu đề của sản phẩm từ OrderDetail
                                        ProductName = od.ProductSizePricing.Product.Name, // Tên sản phẩm từ Product
                                        Size = od.ProductSizePricing.Size, // Kích thước của sản phẩm
                                        Quantity = od.Quantity, // Số lượng sản phẩm
                                        Price = od.ProductSizePricing.Price, // Giá sản phẩm
                                        ImageUrl = od.ImageUrl, // URL ảnh từ OrderDetail (nếu có)
                                        ProductImageUrl = od.ProductSizePricing.Product.ImageUrl // URL ảnh từ Product
                                    }).ToList()
                                }).FirstOrDefault();

            // Kiểm tra nếu đơn hàng không tồn tại
            if (order == null)
            {
                return NotFound(); // Trả về NotFound nếu đơn hàng không tồn tại
            }

            return View(order); // Trả về view với thông tin đơn hàng nếu tìm thấy
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, bool newStatus)
        {
            // Tìm đơn hàng trong cơ sở dữ liệu theo orderId
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

            // Nếu không tìm thấy đơn hàng, trả về NotFound
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng với giá trị mới
            order.Status = newStatus;

            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Chuyển hướng trở lại trang chi tiết đơn hàng, truyền orderId để hiển thị chi tiết đơn hàng vừa cập nhật
            return RedirectToAction("Index", new { orderId = orderId });
        }


        public IActionResult OrderDetail(int orderId)
        {
            // Truy vấn cơ sở dữ liệu để lấy chi tiết đơn hàng dựa trên orderId
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderId == orderId) // Lọc các chi tiết đơn hàng có OrderId khớp với orderId
                .Select(od => new ProductViewModel
                {
                    Id = od.Id, // Lấy Id của chi tiết đơn hàng
                    ProductSizePricingId = od.ProductSizePricingId, // Lấy Id của kích thước sản phẩm trong chi tiết đơn hàng
                    Title = od.Title, // Lấy tiêu đề của sản phẩm từ chi tiết đơn hàng
                    ProductName = od.ProductSizePricing.Product.Name, // Lấy tên sản phẩm từ bảng Product
                    ImageUrl = od.ImageUrl // Lấy URL của ảnh từ chi tiết đơn hàng
                                           // Các thuộc tính khác nếu cần có thể thêm vào đây
                }).ToList();

            // Trả về view "Index" cùng với danh sách chi tiết đơn hàng
            return View("Index", orderDetails);
        }

        [HttpPost]
        public async Task<IActionResult> UploadToS3(IFormFile newImage, int orderId, int productSizePricingId, string title)
        {
            // Kiểm tra nếu có ảnh mới được tải lên
            if (newImage != null)
            {
                // Xác định khóa (key) cho ảnh trong S3 với cấu trúc thư mục theo orderId và productSizePricingId
                string key = $"images/{orderId}/{productSizePricingId}-{newImage.FileName}";

                // Tải ảnh lên S3 và lấy URL của ảnh đã tải lên
                string imageUrl = await _s3Service.UploadFileAsync(newImage, key);

                // Truy vấn cơ sở dữ liệu để lấy chi tiết đơn hàng dựa trên orderId và productSizePricingId
                var orderDetail = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductSizePricingId == productSizePricingId);

                // Nếu tìm thấy chi tiết đơn hàng
                if (orderDetail != null)
                {
                    // Cập nhật URL của ảnh mới và tiêu đề của sản phẩm trong cơ sở dữ liệu
                    orderDetail.ImageUrl = imageUrl;
                    orderDetail.Title = title;

                    // Đánh dấu chi tiết đơn hàng này là đã bị thay đổi để cập nhật trong cơ sở dữ liệu
                    _context.OrderDetails.Update(orderDetail);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                }

                // Trả về JSON chứa URL của ảnh mới sau khi tải lên thành công
                return Json(new { imageUrl });
            }

            // Nếu không có ảnh mới hoặc tải lên thất bại, trả về lỗi BadRequest
            return BadRequest("Image upload failed.");
        }




    }
}
public class OrderDetailViewModel
{
    public int OrderId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ShippingAddress { get; set; }
    public bool Status { get; set; }
    public int? Gender { get; set; }  // Thêm thuộc tính Gender
    public string Address { get; set; }
    public List<ProductViewModel> Products { get; set; }
}

