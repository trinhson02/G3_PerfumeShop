using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class FeedbackResponse
    {
        public FeedbackResponse() 
        {
            SourcesFeedbackRespones = new HashSet<SourcesFeedbackRespone>();
        }
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string ResponseContent { get; set; } = null!;
        public DateTime ResponseDate { get; set; }
        public int UserId { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<SourcesFeedbackRespone> SourcesFeedbackRespones { get; set; }
    }
}
