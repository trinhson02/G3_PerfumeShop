using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G3_PerfumeShop.Controllers
{
    public class UserDetailController : Controller
    {
            
        public readonly G3_PerfumeShopDB_Iter3Context _context;
        public UserDetailController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var loggedInUser = _context.Users.Find(userId);
            if (loggedInUser == null || loggedInUser.RoleId != 1)
            {
                return Unauthorized();
            }

            var user = _context.Users
                .Include(u => u.Status)
                .Include(u => u.Role)
                .SingleOrDefault(u => u.Id == id);

            var genders = _context.Genders.ToList();
            var roles = _context.Roles.ToList();
            var statuses = _context.Statuses.ToList();

            //Tạo danh sách dropdown với role và status , tự động chọn vai trò và trạng thái hiện tại của người dùng 
            ViewBag.StatusList = _context.Statuses
                .Select(status => new SelectListItem
                {
                    Value = status.Id.ToString(),
                    Text = status.Name,
                    Selected = status.Id == user.StatusId
                })
                .ToList();
            ViewBag.RoleList = _context.Roles
                .Select(role => new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name,
                    Selected = role.Id == user.RoleId
                })
                .ToList();
            ViewBag.Genders = genders;
            // Check if the user exists
            if (user == null)
            {
                return NotFound();
            }
            
            return View("Index", user); // Load the Detail view with the specific user’s information
        
        }

        [HttpPost]
        public IActionResult UpdateRoleAndStatus(int id, int roleId, int statusId)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
           
            // Update user role and status without deleting the user
            user.RoleId = roleId;
            user.StatusId = statusId;

            // Save changes to the database
            _context.SaveChanges();
             return RedirectToAction("Detail", new { id = id });
        }


        //// Cập nhật vai trò và trạng thái của người dùng
        //[HttpPost]
        //public IActionResult UpdateRoleAndStatus(int id, string role, string status)
        //{
        //    // Logic cập nhật vai trò và trạng thái ở đây
        //    TempData["Message"] = "User role and status updated successfully!";
        //    return RedirectToAction("Details", new { id = id });
        //}

        //// Thêm người dùng mới
        //[HttpPost]
        //public IActionResult Add(string fullname, string gender, string email, string mobile, string address)
        //{
        //    // Giả lập tạo mật khẩu mới
        //    string generatedPassword = Guid.NewGuid().ToString().Substring(0, 8);

        //    // Logic để thêm người dùng vào cơ sở dữ liệu ở đây
        //    // Giả lập gửi email cho người dùng mới
        //    TempData["Message"] = $"User added successfully! A login password has been sent to {email}.";

        //    return RedirectToAction("Details", new { id = 1 }); // Chuyển hướng đến chi tiết người dùng mới thêm
        //} 
    }
}
