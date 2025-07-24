using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class FilterDashboardParameter
    {
        public FilterDashboardParameter()
        {
            CustomDashboards = new HashSet<CustomDashboard>();
            DefaultDashboards = new HashSet<DefaultDashboard>();
        }

        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int DashboardSectionId { get; set; }

        public virtual DashboardSection DashboardSection { get; set; } = null!;
        public virtual ICollection<CustomDashboard> CustomDashboards { get; set; }
        public virtual ICollection<DefaultDashboard> DefaultDashboards { get; set; }
    }
}
