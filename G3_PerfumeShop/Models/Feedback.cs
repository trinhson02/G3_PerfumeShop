using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            FeedbackResponses = new HashSet<FeedbackResponse>();
            Sources = new HashSet<Source>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
        public string Content { get; set; } = null!;
        public int Rating { get; set; }
        public bool Status { get; set; }
        public int ProductId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<FeedbackResponse> FeedbackResponses { get; set; }
        public virtual ICollection<Source> Sources { get; set; }
    }
}
