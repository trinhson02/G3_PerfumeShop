using G3_PerfumeShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G3_PerfumeShop.Controllers
{
    public class UserListController : Controller
    {
        public readonly G3_PerfumeShopDB_Iter3Context _context;

        public UserListController(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (roleId != 1)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            //Lấy danh sách người dùng 
            var genders = _context.Genders.ToList();
            var statuses = _context.Statuses.ToList();
            var roles = _context.Roles.ToList();
            var users = _context.Users.Include(u => u.Status).Include(u => u.Role).ToList();

            //Tạo 1 UserListViewModel chứa thông tin người dùng , số trang hiện tại , kích thước trang và tổng số bản ghi 
            var model = new UserListViewModel
            {
                Users = _context.Users.Include(u => u.Status).Include(u => u.Role).ToList(),
                Page = 1,
                PageSize = 5,
                TotalItems = _context.Users.Count()
            };
            //Gán vào ViewBag để truyền dữ liệu đến View
            ViewBag.Genders = genders;
            ViewBag.Roles = roles;
            ViewBag.Statuses = statuses;


            return View(model);
        }
        [HttpPost]
        public IActionResult ManageUsers(int? Gender, int? RoleId, int? StatusId, int pageNumber = 1, int rowsPerPage = 5)
        {
            //Lọc danh sách người dùng theo giới tính , vai trò và trạng thái
            var usersQuery = _context.Users.Include(u => u.Role).Include(u => u.Status).AsQueryable();
            if (Gender.HasValue) usersQuery = usersQuery.Where(u => u.Gender == Gender.Value);
            if (RoleId.HasValue) usersQuery = usersQuery.Where(u => u.RoleId == RoleId.Value);
            if (StatusId.HasValue) usersQuery = usersQuery.Where(u => u.StatusId == StatusId.Value);

            //Xử lý phân trang
            int totalCount = usersQuery.Count();
            var users = usersQuery.Skip((pageNumber - 1) * rowsPerPage).Take(rowsPerPage).ToList();

            //Tạo và truyền UserListViewModel chứa dữ liệu lọc tới view Index
            var model = new UserListViewModel
            {
                Users = users,
                StatusId = StatusId,
                Page = pageNumber,
                PageSize = rowsPerPage,
                TotalItems = totalCount,
            };
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.Genders = _context.Genders.ToList();
            ViewBag.Roles = _context.Roles.ToList();

            return View("Index", model);
        }
        public IActionResult EditUser(int id)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 1 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            Console.WriteLine(HttpContext.Session.GetInt32("RoleId"));
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy thông tin người dùng từ database, tạo một EditUserViewModel và truyền nó đến view.
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.Phone,
                Birthdate = user.Birthdate,
                Gender = user.Gender,
                StatusId = user.StatusId,
                Statuses = _context.Statuses.ToList()
            };

            return View(model);
        }

        // Xử lý lưu thông tin chỉnh sửa
        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 1 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.Id);
                if (user != null)
                {
                    user.Username = model.Username;
                    user.Password = model.Password;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.Birthdate = model.Birthdate;
                    user.Gender = model.Gender;
                    user.StatusId = model.StatusId;

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }

            // Nếu Model không hợp lệ, ghi lại các lỗi
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }


            // Nếu Model không hợp lệ, làm lại thông tin trạng thái
            model.Statuses = _context.Statuses.ToList();
            return View(model);
        }
        public IActionResult AddNewUser()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (roleId != 1 || userId == null)
            {
                return RedirectToAction("UnauthorizedAccess", "Home");
            }
            //Tạo AddUserViewModel chứa danh sách trạng thái và vai trò để hiển thị trong form.
            var model = new AddUserViewModel
            {
                Statuses = _context.Statuses.ToList(),
                Roles = _context.Roles.ToList()
            };

            return View(model);
        }

        // Xử lý lưu người dùng mới
        [HttpPost]
        public IActionResult AddNewUser(AddUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Address = model.Address,
                    Username = model.Username,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Birthdate = model.Birthdate,
                    Gender = model.Gender,
                    StatusId = model.StatusId,
                    RoleId = model.RoleId
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu Model không hợp lệ, hiển thị lại danh sách trạng thái
            model.Statuses = _context.Statuses.ToList();
            return View(model);
        }
    }
}
