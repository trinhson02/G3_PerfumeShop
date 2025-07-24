using Microsoft.AspNetCore.Mvc;
using G3_PerfumeShop.Models;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using G3_PerfumeShop.Service;

namespace G3_PerfumeShop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;  // Đối tượng DbContext cho phép truy cập và thao tác với cơ sở dữ liệu
        private readonly IConfiguration _configuration; // Đối tượng IConfiguration để truy cập các thiết lập cấu hình
        private readonly S3Service_Son _s3Service; // Dịch vụ S3 dùng để lưu trữ và quản lý ảnh trên Amazon S3

        public ProfileController(S3Service_Son s3Service, G3_PerfumeShopDB_Iter3Context context, IConfiguration configuration)
        {
            _s3Service = s3Service; // Gán dịch vụ S3Service_Son cho biến thành viên _s3Service
            _context = context; // Gán đối tượng DbContext cho biến thành viên _context
            _configuration = configuration; // Gán đối tượng IConfiguration cho biến thành viên _configuration
        }

        [HttpGet]
        public IActionResult Index()
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var gender = _context.Genders.ToList();

            ViewBag.Genders = gender;

            return View(user); // Pass the user data to the view
        }

        [HttpPost]
        public async Task<ActionResult> Index(User model , IFormFile ImageFile)
        {
            
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (string.IsNullOrEmpty(model.LastName)) {
                ModelState.AddModelError("LastName", "Họ không được để trống.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.FirstName))
            {
                ModelState.AddModelError("FirstName", "Tên không được để trống.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                ModelState.AddModelError("Address", "Địa chỉ không được để trống.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Phone))
            {
                ModelState.AddModelError("Phone", "Số điện thoại không được để trống.");
                return View(model);
            }
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // up ảnh lên amazons3 sử dụng _s3Service
                var imageUrl = await _s3Service.UploadFileAsync(ImageFile, "images-folder"); // Assuming "images-folder" is the target S3 folder
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.ImageUrl = imageUrl; // Cập nhật đường dẫn ảnh mới từ S3
                }
            }

            Console.WriteLine($"Before Edit - FirstName: {user.FirstName}, LastName: {user.LastName}");

            // Update user details
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Birthdate = model.Birthdate;
            user.Gender = model.Gender;



            _context.SaveChanges();
            await SendEmailAsync(user);
            Console.WriteLine($"After Edit - FirstName: {user.FirstName}, LastName: {user.LastName}");

            // After saving, redirect to the same page
            return RedirectToAction("Index");
        }

        public async Task SendEmailAsync(User user)
        {
            // Đọc nội dung từ file HTML
            string templatePath = "wwwroot/back/mailTemplate/changeProfile.html";
            string emailBody = System.IO.File.ReadAllText(templatePath);
            Console.WriteLine(emailBody);

            // Thay thế các placeholders bằng dữ liệu người dùng
            emailBody = emailBody
                .Replace("{FirstName}", user.FirstName)
                .Replace("{LastName}", user.LastName)
                .Replace("{Phone}", user.Phone)
                .Replace("{Address}", user.Address)
                .Replace("{Birthdate}", user.Birthdate.ToString("dd/MM/yyyy"))
                .Replace("{Gender}", user.Gender.ToString());

            // Khởi tạo HttpClient và gửi yêu cầu
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("9db8722fc724b9831c22ff62:"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("from", "support@g3perfume.click"),
            new KeyValuePair<string, string>("to", user.Email),
            new KeyValuePair<string, string>("subject", "Thông báo cập nhật thông tin người dùng"),
            new KeyValuePair<string, string>("html", emailBody),
            new KeyValuePair<string, string>("sender", "Hỗ trợ từ G3 Perfume")
        });

                var response = await httpClient.PostAsync("https://api.forwardemail.net/v1/emails", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Email sent successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to send email. Status Code: " + response.StatusCode);
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error Details: " + errorDetails);
                }
            }
        }


    }
}
