using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Blog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BlogCategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string BlogContent { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Author { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int BlogStatusId { get; set; }

        public virtual BlogCategory BlogCategory { get; set; } = null!;
        public virtual BlogStatus BlogStatus { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
