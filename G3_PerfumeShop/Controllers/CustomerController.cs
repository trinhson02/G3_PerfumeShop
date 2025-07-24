
using Microsoft.AspNetCore.Mvc;
using G3_PerfumeShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CustomerController : Controller
{
    private readonly G3_PerfumeShopDB_Iter3Context _context;

    public CustomerController(G3_PerfumeShopDB_Iter3Context context)
    {
        _context = context;
    }

    public IActionResult CustomerList(string searchTerm, int? statusId, string sortOrder, int page = 1, int pageSize = 2)
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");
        if ( roleId == 3 || roleId == 4 || userId == null)
        {
            return RedirectToAction("UnauthorizedAccess", "Home");
        }
        var query = _context.Users
            .Where(u => u.RoleId == 4)
            .Include(u => u.Status) 
            .AsQueryable();

        // Tìm kiếm theo tên, email, số điện thoại
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(searchTerm) ||
                u.LastName.Contains(searchTerm) ||
                u.Email.Contains(searchTerm) ||
                u.Phone.Contains(searchTerm));
        }

        // Lọc theo trạng thái
        if (statusId.HasValue)
        {
            query = query.Where(u => u.StatusId == statusId);
        }

        // Sắp xếp dữ liệu theo các cột
        switch (sortOrder)
        {
            case "firstname_asc":
                query = query.OrderBy(u => u.FirstName);
                break;
            case "firstname_desc":
                query = query.OrderByDescending(u => u.FirstName);
                break;
            case "lastname_asc":
                query = query.OrderBy(u => u.LastName);
                break;
            case "lastname_desc":
                query = query.OrderByDescending(u => u.LastName);
                break;
            case "email_asc":
                query = query.OrderBy(u => u.Email);
                break;
            case "email_desc":
                query = query.OrderByDescending(u => u.Email);
                break;
            case "phone_asc":
                query = query.OrderBy(u => u.Phone);
                break;
            case "phone_desc":
                query = query.OrderByDescending(u => u.Phone);
                break;
            default:
                query = query.OrderBy(u => u.Id);
                break;
        }


        // Phân trang
        var totalItems = query.Count();
        var customers = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var dynamicColumns = new List<string> { "ID", "FirstName", "LastName", "Email", "Phone", "Birthdate", "Gender", "Status" };

        // Chuẩn bị model cho view
        var model = new CustomerListViewModel
        {
            Customers = customers,
            SearchTerm = searchTerm,
            StatusId = statusId,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            Statuses = _context.Statuses.ToList(),
            DynamicColumns = dynamicColumns
        };

        return View(model);
    }
    [HttpDelete]
    public IActionResult Delete(int id)
    {

        var roleId = HttpContext.Session.GetInt32("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (roleId != 2 || userId == null)
        {
            return RedirectToAction("UnauthorizedAccess", "Home");
        }

        var customer = _context.Users.Find(id);
        if (customer != null)
        {
            _context.Users.Remove(customer);
            _context.SaveChanges();
            return Ok();
        }

        return NotFound();
    }
    // Hiển thị trang thêm khách hàng mới
    public IActionResult AddNewCustomer()
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (roleId != 2 || userId == null) 
        {
            return RedirectToAction("UnauthorizedAccess", "Home");
        }
        var model = new AddCustomerViewModel
        {
            Statuses = _context.Statuses.ToList() 
        };

        return View(model);
    }

    // Xử lý lưu khách hàng mới
    [HttpPost]
    public IActionResult AddNewCustomer(AddCustomerViewModel model)
    {

        if (ModelState.IsValid)
        {
            var customer = new User
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
                RoleId = 4 
            };

            _context.Users.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }

        // Nếu Model không hợp lệ, hiển thị lại danh sách trạng thái
        model.Statuses = _context.Statuses.ToList();
        return View(model);
    }

    public IActionResult CustomerDetail(int id)
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (roleId != 2 || userId == null)
        {
            return RedirectToAction("UnauthorizedAccess", "Home");
        }
        Console.WriteLine(HttpContext.Session.GetInt32("RoleId"));
        var customer = _context.Users.Find(id);
        if (customer == null)
        {
            return NotFound();
        }

        // Chuyển đổi model sang ViewModel nếu cần thiết
        var model = new CustomerDetailViewModel
        {
            Id = customer.Id,
            ImageUrl = customer.ImageUrl,
            Username = customer.Username,
            Password = customer.Password,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Address = customer.Address,
            Email = customer.Email,
            Phone = customer.Phone,
            Birthdate = customer.Birthdate,
            Gender = customer.Gender,
            StatusId = customer.StatusId,
            Statuses = _context.Statuses.ToList()
        };

        return View(model);
    }
    [HttpPost]
    public IActionResult CustomerDetail(CustomerDetailViewModel model, IFormFile ImageFile)
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        var userId = HttpContext.Session.GetInt32("UserId");

        var customer = _context.Users.Find(model.Id);
        if (roleId != 2 || userId == null)
        {
            return RedirectToAction("UnauthorizedAccess", "Home");
        }
        //Update ảnh
        //if (ImageFile != null && ImageFile.Length > 0)
        //{
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", ImageFile.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        ImageFile.CopyTo(stream);
        //    }
        //    customer.ImageUrl = "/images/" + ImageFile.FileName; // Cập nhật đường dẫn ảnh mới
        //}

        if (ModelState.IsValid)
        {
            if (customer != null)
            {
                customer.ImageUrl = model.ImageUrl;
                customer.Password = model.Password;
                customer.Username = model.Username;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Address = model.Address;
                customer.Email = model.Email;
                customer.Phone = model.Phone;
                customer.Birthdate = model.Birthdate;
                customer.Gender = model.Gender;
                customer.StatusId = model.StatusId;

                _context.SaveChanges();
                return RedirectToAction("CustomerList");
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
    // Action tạo, xem, sửa, xóa khách hàng sẽ viết thêm ở đây
}
