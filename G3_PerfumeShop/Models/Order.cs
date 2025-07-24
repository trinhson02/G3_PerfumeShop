using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Order
    {
        public Order()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingEmail { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Phone { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
