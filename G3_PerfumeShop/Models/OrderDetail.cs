using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int ProductSizePricingId { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Title { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ProductSizePricing ProductSizePricing { get; set; } = null!;
    }
}
