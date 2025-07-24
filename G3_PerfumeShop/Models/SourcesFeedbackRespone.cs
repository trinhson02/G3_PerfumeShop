using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class SourcesFeedbackRespone
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int FeedbackResponseId { get; set; }
        public string? Caption { get; set; }

        public virtual FeedbackResponse FeedbackResponse { get; set; } = null!;
    }
}
