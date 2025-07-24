using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class User
    {
        public User()
        {
            Blogs = new HashSet<Blog>();
            CustomDashboards = new HashSet<CustomDashboard>();
            DashBoardUserOptions = new HashSet<DashBoardUserOption>();
            FeedbackResponses = new HashSet<FeedbackResponse>();
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
            UserRolesAuditChangedByNavigations = new HashSet<UserRolesAudit>();
            UserRolesAuditUsers = new HashSet<UserRolesAudit>();
        }

        public int Id { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public int StatusId { get; set; }
        public int? Gender { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpExpiration { get; set; }
        public string GenderDisplay
        {
            get
            {
                return Gender switch
                {
                    1 => "Nam",
                    2 => "Nữ",
                    _ => "Chưa cập nhật"
                };
            }
        }
        public virtual Role Role { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<CustomDashboard> CustomDashboards { get; set; }
        public virtual ICollection<DashBoardUserOption> DashBoardUserOptions { get; set; }
        public virtual ICollection<FeedbackResponse> FeedbackResponses { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserRolesAudit> UserRolesAuditChangedByNavigations { get; set; }
        public virtual ICollection<UserRolesAudit> UserRolesAuditUsers { get; set; }
    }
}
