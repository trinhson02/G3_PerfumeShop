using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
