using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class DashBoardUserOption
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsDefault { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
