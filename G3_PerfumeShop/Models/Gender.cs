using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
