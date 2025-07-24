using G3_PerfumeShop.Models; // Import mô hình của dự án, chứa các class như Product, Brand, Gender, v.v.
using G3_PerfumeShop.Service; // Import dịch vụ S3Service để quản lý lưu trữ các file đa phương tiện.
using Microsoft.AspNetCore.Mvc; // Import lớp cơ bản để xây dựng controller trong ASP.NET MVC.
using Microsoft.EntityFrameworkCore; // Import thư viện để làm việc với Entity Framework Core.

namespace G3_PerfumeShop.Controllers
{
    // Controller quản lý các sản phẩm trong hệ thống với các thao tác như xem, chỉnh sửa chi tiết sản phẩm, hiển thị hoặc ẩn sản phẩm.
    public class ProductManagerController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context; // Ngữ cảnh cơ sở dữ liệu để truy vấn và tương tác với dữ liệu.
        private readonly S3Service _s3Service; // Dịch vụ lưu trữ S3 để quản lý ảnh hoặc file đa phương tiện của sản phẩm.

        public ProductManagerController(G3_PerfumeShopDB_Iter3Context context, S3Service s3Service)
        {
            _context = context; // Khởi tạo ngữ cảnh cơ sở dữ liệu.
            _s3Service = s3Service; // Khởi tạo dịch vụ lưu trữ S3.
        }

        // Hiển thị chi tiết của một sản phẩm.
        public ActionResult ProductDetail(int id)
        {
            // Kiểm tra quyền truy cập của người dùng thông qua session RoleId và UserId.
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 2 || userId == null) // Nếu không phải role 2 hoặc UserId null.
            {
                return RedirectToAction("UnauthorizedAccess", "Home"); // Chuyển hướng tới view UnauthorizedAccess.
            }

            // Truy vấn để lấy thông tin sản phẩm cùng các dữ liệu liên quan từ database.
            var product = _context.Products
                .Include(p => p.Brand) // Bao gồm Brand của sản phẩm.
                .Include(p => p.ProductCategory) // Bao gồm Category của sản phẩm.
                .Include(p => p.Gender) // Bao gồm thông tin Gender của sản phẩm.
                .Include(p => p.Origin) // Bao gồm thông tin Origin của sản phẩm.
                .Include(p => p.ProductSizePricings) // Bao gồm danh sách ProductSizePricing.
                .FirstOrDefault(p => p.Id == id); // Lấy sản phẩm với Id trùng với tham số truyền vào.

            if (product == null) // Nếu không tìm thấy sản phẩm.
            {
                return NotFound(); // Trả về trang lỗi 404.
            }

            // Tạo viewModel chứa thông tin chi tiết sản phẩm để gửi sang view.
            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                SizePricings = product.ProductSizePricings.ToList(),
                SelectedPrice = product.ProductSizePricings.FirstOrDefault()?.Price ?? 0
            };

            return View(viewModel); // Trả về view với model chứa thông tin chi tiết sản phẩm.
        }

        // Hiển thị trang chỉnh sửa chi tiết của một sản phẩm.
        public IActionResult EditProductDetail(int id)
        {
            // Kiểm tra quyền truy cập.
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 2 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            // Truy vấn để lấy sản phẩm và các dữ liệu liên quan từ database.
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Origin)
                .Include(p => p.ProductSizePricings)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Trả về trang lỗi 404 nếu không tìm thấy sản phẩm.
            }

            // Tạo viewModel cho trang chỉnh sửa chi tiết sản phẩm.
            var viewModel = new EditProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                ReleaseYear = product.ReleaseYear.Year,
                Longevity = product.Longevity,
                Concentration = product.Concentration,
                Point = product.Point,
                GenderId = product.GenderId,
                BrandId = product.BrandId,
                OriginId = product.OriginId,
                Genders = _context.Genders.ToList(),
                Brands = _context.Brands.ToList(),
                Origins = _context.Origins.ToList(),
            };

            return View(viewModel); // Trả về view với model để chỉnh sửa chi tiết sản phẩm.
        }

        // Phương thức POST để lưu lại các thay đổi của sản phẩm.
        [HttpPost]
        public IActionResult EditProductDetail(EditProductDetailViewModel model)
        {
            // Kiểm tra quyền truy cập.
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 2 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            if (ModelState.IsValid) // Nếu dữ liệu hợp lệ.
            {
                // Truy vấn sản phẩm trong database.
                var product = _context.Products
                    .Include(p => p.ProductSizePricings)
                    .FirstOrDefault(p => p.Id == model.Id);
                if (product == null)
                {
                    return NotFound();
                }

                // Cập nhật các thông tin của sản phẩm từ model.
                product.Name = model.Name;
                product.ImageUrl = model.ImageUrl;
                product.Description = model.Description;
                product.ReleaseYear = new DateTime(model.ReleaseYear, 1, 1);
                product.Longevity = model.Longevity;
                product.Concentration = model.Concentration;
                product.Point = model.Point;
                product.GenderId = model.GenderId;
                product.BrandId = model.BrandId;
                product.OriginId = model.OriginId;
                _context.Update(product); // Cập nhật sản phẩm trong context.
                _context.SaveChanges(); // Lưu các thay đổi vào database.

                return RedirectToAction("ProductDetail", new { id = model.Id }); // Chuyển hướng đến trang chi tiết sản phẩm.
            }

            // In ra các lỗi nếu có.
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

            // Thiết lập lại danh sách cho các dropdown trong trường hợp có lỗi.
            model.Genders = _context.Genders.ToList();
            model.Brands = _context.Brands.ToList();
            model.Origins = _context.Origins.ToList();

            return View(model); // Trả về view cùng model đã cập nhật.
        }

        // Phương thức POST để thay đổi trạng thái hiển thị của sản phẩm.
        [HttpPost]
        public IActionResult ToggleVisibility(int id, bool isHidden)
        {
            // Kiểm tra quyền truy cập.
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 2 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            // Truy vấn sản phẩm từ database.
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Trả về trang lỗi 404 nếu không tìm thấy sản phẩm.
            }

            product.IsHidden = isHidden; // Cập nhật trạng thái hiển thị của sản phẩm.
            _context.SaveChanges(); // Lưu thay đổi vào database.
            return RedirectToAction("ProductDetail", new { id = id }); // Chuyển hướng đến trang chi tiết sản phẩm.
        }
    }
}
