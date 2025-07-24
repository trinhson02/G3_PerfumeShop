using System;
using System.Linq;
using System.Web;
using G3_PerfumeShop.Models;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;


namespace G3_PerfumeShop.Controllers
{
    public class SliderController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;//Là một đối tượng của lớp G3_PerfumeShopDB_Iter3Context, dùng để tương tác với cơ sở dữ liệu (thông qua Entity Framework).
        private readonly IWebHostEnvironment _webHostEnvironment;//cung cấp thông tin về môi trường web, như đường dẫn gốc của thư mục chứa các tài nguyên web

        public SliderController(G3_PerfumeShopDB_Iter3Context context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;//webHostEnvironment (để biết đường dẫn thư mục web).
            _context = context;//context (dùng để truy cập cơ sở dữ liệu)
        }

        // Action để hiển thị chi tiết Slider
        public IActionResult Detail(int id)
        {
            // Truy xuất dữ liệu từ database dựa vào Id
            var slider = _context.SliderDetail.FirstOrDefault(s => s.Id == id);//Tìm một đối tượng SliderDetail trong cơ sở dữ liệu có Id trùng với id đã cung cấp.
            if (slider == null)//Nếu không tìm thấy, trả về NotFound().
            {
                return NotFound();
            }

            return View(slider);
        }

        // Action để upload hình ảnh (giữ nguyên code của bạn)
        [HttpPost]
        public IActionResult UploadImage(Slider slider, IFormFile imageFile)
        {
            if (imageFile != null)//Kiểm tra nếu tệp hình ảnh được chọn trong form không phải là null.
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");//Xác định thư mục chứa các hình ảnh tải lên.
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;//Tạo một tên tệp duy nhất để tránh bị trùng tên khi lưu trữ nhiều tệp với tên giống nhau, sử dụng Guid.NewGuid() để tạo ra một mã ngẫu nhiên.
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);//Xây dựng đường dẫn đầy đủ để lưu tệp hình ảnh vào thư mục images.
                using (var fileStream = new FileStream(filePath, FileMode.Create))//Mở một luồng tệp để lưu tệp hình ảnh vào ổ đĩa.
                {
                    imageFile.CopyTo(fileStream);
                }
                slider.ImagePath = "/images/" + uniqueFileName;
            }

            _context.SliderDetail.Update(slider);//Cập nhật thông tin của slider trong cơ sở dữ liệu.
            _context.SaveChanges();//Lưu thông tin

            return RedirectToAction("Detail", new { id = slider.Id });
        }
    }
}
