using System.ComponentModel.DataAnnotations;

namespace G3_PerfumeShop.Models
{
    public class CustomerDetailViewModel
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }

        public string Address { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public int? Gender { get; set; } // Số nguyên 1 hoặc 2
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int StatusId { get; set; }
        public List<Status> Statuses { get; set; } = new List<Status>();
    }
}
