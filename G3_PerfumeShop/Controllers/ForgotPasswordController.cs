using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using G3_PerfumeShop.Models;
using System.Net.Http.Headers;

namespace G3_PerfumeShop.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;

        public ForgotPasswordController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        // Gửi OTP qua email
        [HttpPost]
        public async Task<JsonResult> SendOtp([FromBody] EmailDto emailDto)
        {
            // Tìm người dùng qua email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailDto.Email);
            if (user == null)
            {
                return Json(new { success = false }); // Trả về false nếu người dùng không tồn tại
            }

            // Tạo mã OTP ngẫu nhiên
            var otp = new Random().Next(100000, 999999).ToString();
            user.PasswordResetOtp = otp;
            user.OtpExpiration = DateTime.Now.AddMinutes(10);


            await SendEmailAsync(user);
            // OTP hết hạn sau 10 phút
            await _context.SaveChangesAsync();

            // Gọi phương thức gửi email
            // await _gmailServiceHelper.SendEmailAsync(emailDto.Email, "ngocsons2001@gmail.com", "Mã OTP để đặt lại mật khẩu", $"Mã OTP của bạn là: {otp}");

            return Json(new { success = true });
        }

        public async Task SendEmailAsync(User user)
        {
            // Đọc nội dung từ file HTML
            string templatePath = "wwwroot/back/mailTemplate/forgotPasswordVerify.html";
            string emailBody = System.IO.File.ReadAllText(templatePath);
            Console.WriteLine(emailBody);

            // Thay thế các placeholders bằng dữ liệu người dùng
            emailBody = emailBody
                .Replace("{UserName}", user.Email)
                .Replace("{VerificationCode}", user.PasswordResetOtp)
                .Replace("{ExpiryTime}", ((DateTime)user.OtpExpiration).ToString("dd MMM HH:mm:ss 'GMT' zzz", System.Globalization.CultureInfo.InvariantCulture));

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
            new KeyValuePair<string, string>("subject", "Yêu cầu thay đổi mật khẩu"),
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

        // Hiển thị trang Index
        [HttpGet]
        public IActionResult Index()
        {  
            
            // Kiểm tra nếu người dùng đã đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Người dùng đã đăng nhập, chuyển hướng về trang chủ
                return RedirectToAction("Index", "Home");
            }



            return View();
        }

        // Lớp DTO cho yêu cầu email
        public class EmailDto
        {
            public string Email { get; set; }
        }

        // Xác thực OTP
        [HttpPost]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                // Lấy người dùng từ email
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null || user.PasswordResetOtp != request.Otp || user.OtpExpiration < DateTime.Now)
                {
                    return Json(new { success = false }); // Trả về false nếu OTP không hợp lệ hoặc đã hết hạn
                }

                return Json(new { success = true }); // OTP hợp lệ
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log lỗi
                return Json(new { success = false, error = "Có lỗi xảy ra khi xác thực OTP." });
            }
        }

        // Đặt lại mật khẩu
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordResetOtp != null && u.OtpExpiration >= DateTime.Now);

                if (user == null)
                {
                    return Json(new { success = false, error = "OTP không hợp lệ, đã hết hạn." });
                }

                user.Password = ConvertToHash(request.NewPassword); // Mã hóa mật khẩu mới
                user.PasswordResetOtp = null; // Xóa OTP sau khi đổi mật khẩu thành công
                user.OtpExpiration = null;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Mật khẩu đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, error = "Có lỗi xảy ra khi cập nhật mật khẩu." });
            }
        }

        // Hàm chuyển đổi mật khẩu thành dạng hash (cần thực hiện mã hóa thực sự)
        private string ConvertToHash(string password)
        {
            // Sử dụng thư viện mã hóa để đảm bảo tính bảo mật
            return password; // Đây là placeholder, cần thay bằng hàm mã hóa thực sự
        }

        // Lớp hỗ trợ cho yêu cầu đặt lại mật khẩu
        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string NewPassword { get; set; }
        }
    }

    // Lớp hỗ trợ cho yêu cầu xác thực OTP
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }

}
