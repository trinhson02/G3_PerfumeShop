using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class ProductSizePricing
    {
        public ProductSizePricing()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
