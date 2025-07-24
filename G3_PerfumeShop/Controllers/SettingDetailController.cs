using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace G3_PerfumeShop.Controllers
{
    public class SettingDetailController : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;
        // Đối tượng DbContext để truy cập và thao tác với cơ sở dữ liệu

        public SettingDetailController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
            // Khởi tạo đối tượng _context bằng cách truyền vào DbContext để có thể dùng cho các thao tác cơ sở dữ liệu
        }

        public IActionResult Index(int userId)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userIdcheck = HttpContext.Session.GetInt32("UserId");

            if (userIdcheck == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (roleId != 1 || userIdcheck == null) // Kiểm tra quyền truy cập
            {
                return RedirectToAction("UnauthorizedAccess", "Home"); // Chuyển hướng tới UnauthorizedAccess view
            }

            // Lấy thông tin người dùng theo userId, bao gồm vai trò hiện tại và lịch sử thay đổi vai trò
            var user = _context.Users
                .Include(u => u.Role) // Bao gồm vai trò hiện tại của người dùng
                .Include(u => u.UserRolesAuditUsers) // Bao gồm lịch sử thay đổi vai trò của người dùng
                    .ThenInclude(audit => audit.OldRole) // Bao gồm vai trò cũ của người dùng trong lịch sử thay đổi
                .Include(u => u.UserRolesAuditUsers)
                    .ThenInclude(audit => audit.NewRole) // Bao gồm vai trò mới trong lịch sử thay đổi
                .Include(u => u.UserRolesAuditUsers)
                    .ThenInclude(audit => audit.ChangedByNavigation) // Bao gồm thông tin người thực hiện thay đổi
                .FirstOrDefault(u => u.Id == userId); // Lấy người dùng đầu tiên có Id khớp với userId

            if (user == null)
            {
                return NotFound(); // Trả về NotFound nếu người dùng không tồn tại
            }

            // Lấy các vai trò có sẵn để gán cho người dùng, loại trừ vai trò Admin và Customer
            var availableRoles = _context.Roles
                .Where(r => r.Id != 1 && r.Id != 4)
                .ToList();

            // Lấy bản ghi thay đổi vai trò mới nhất trong lịch sử thay đổi vai trò của người dùng
            var latestAuditRecord = user.UserRolesAuditUsers
                .OrderByDescending(audit => audit.Id) // Sắp xếp theo Id giảm dần
                .FirstOrDefault(); // Lấy bản ghi đầu tiên

            ViewBag.AvailableRoles = availableRoles; // Truyền danh sách vai trò có sẵn vào View
            ViewBag.UserId = user.Id; // Truyền Id của người dùng vào View
            ViewBag.LatestAuditRecord = latestAuditRecord; // Truyền bản ghi thay đổi vai trò mới nhất vào View

            return View(user.UserRolesAuditUsers); // Trả về view với danh sách lịch sử thay đổi vai trò của người dùng
        }

        [HttpPost]
        public IActionResult UpdateRole(int userId, int newRoleId, string note)
        {
            // Tìm người dùng theo userId
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound(); // Nếu không tìm thấy người dùng, trả về NotFound

            // Kiểm tra nếu vai trò mới là Admin hoặc Customer thì không cho phép thay đổi
            if (newRoleId == 1 || newRoleId == 4)
            {
                ModelState.AddModelError("", "Không thể thay đổi thành vai trò Admin hoặc Customer.");
                return RedirectToAction("Index", new { userId });
            }

            // Lấy bản ghi mới nhất của UserRolesAudit cho người dùng để kiểm tra và cập nhật
            var auditRecord = _context.UserRolesAudits
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id) // Sắp xếp theo Id giảm dần
                .FirstOrDefault(); // Lấy bản ghi đầu tiên (mới nhất)

            if (auditRecord != null)
            {
                // Cập nhật bản ghi hiện có với vai trò mới và thông tin thay đổi
                auditRecord.NewRoleId = newRoleId; // Vai trò mới
                auditRecord.ChangedBy = 1; // Id của người thực hiện thay đổi
                auditRecord.ChangeDate = DateTime.Now; // Ngày thay đổi
                auditRecord.Note = note; // Ghi chú về lý do thay đổi
            }
            else
            {
                // Nếu chưa có bản ghi nào, tạo một bản ghi mới cho lịch sử thay đổi vai trò
                auditRecord = new UserRolesAudit
                {
                    UserId = userId, // Id của người dùng
                    OldRoleId = user.RoleId, // Vai trò cũ của người dùng
                    NewRoleId = newRoleId, // Vai trò mới
                    ChangedBy = 1, // Id của người thực hiện thay đổi
                    ChangeDate = DateTime.Now, // Ngày thay đổi
                    Note = note // Ghi chú về lý do thay đổi
                };
                _context.UserRolesAudits.Add(auditRecord); // Thêm bản ghi mới vào cơ sở dữ liệu
            }

            // Cập nhật RoleId của người dùng thành vai trò mới
            user.RoleId = newRoleId;
            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Quay lại trang Index để tải lại dữ liệu mới sau khi cập nhật
            return RedirectToAction("Index", new { userId });
        }

    }
}
