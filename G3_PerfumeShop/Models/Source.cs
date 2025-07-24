using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Source
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int FeedbackId { get; set; }
        public string? Caption { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
    }
}
