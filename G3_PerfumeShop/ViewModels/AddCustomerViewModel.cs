using System.ComponentModel.DataAnnotations;

namespace G3_PerfumeShop.Models
{
    public class AddCustomerViewModel
    {
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = null!;
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?""':{}|<>]).*$", ErrorMessage = "Password must contain at least one special character.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; } = null!;
        [Required(ErrorMessage = "Birthdate is required.")]
        public DateTime Birthdate { get; set; }
        public int? Gender { get; set; } // Số nguyên 1 hoặc 2
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int StatusId { get; set; }
        public int RoleId { get; set; }


        public List<Status> Statuses { get; set; } = new List<Status>();
    }
}
