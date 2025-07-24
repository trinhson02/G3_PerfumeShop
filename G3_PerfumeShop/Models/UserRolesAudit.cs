using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class UserRolesAudit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OldRoleId { get; set; }
        public int NewRoleId { get; set; }
        public int ChangedBy { get; set; }
        public DateTime ChangeDate { get; set; }
        public string? Note { get; set; }

        public virtual User ChangedByNavigation { get; set; } = null!;
        public virtual Role NewRole { get; set; } = null!;
        public virtual Role OldRole { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
