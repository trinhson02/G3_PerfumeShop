using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class CustomDashboard
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FilterParamId { get; set; }
        public int Order { get; set; }
        public bool IsDisplay { get; set; }

        public virtual FilterDashboardParameter FilterParam { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
