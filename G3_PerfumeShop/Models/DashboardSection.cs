using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class DashboardSection
    {
        public DashboardSection()
        {
            FilterDashboardParameters = new HashSet<FilterDashboardParameter>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<FilterDashboardParameter> FilterDashboardParameters { get; set; }
    }
}
