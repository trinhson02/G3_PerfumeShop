using System;
using System.Collections.Generic;

namespace G3_PerfumeShop.Models
{
    public partial class Product
    {
        public Product()
        {
            Feedbacks = new HashSet<Feedback>();
            ProductSizePricings = new HashSet<ProductSizePricing>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int GenderId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime ReleaseYear { get; set; }
        public int Longevity { get; set; }
        public int OriginId { get; set; }
        public int Concentration { get; set; }
        public int Point { get; set; }
        public int ProductCategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsHidden { get; set; }
        public string? Title { get; set; }

        public virtual Brand Brand { get; set; } = null!;
        public virtual Gender Gender { get; set; } = null!;
        public virtual Origin Origin { get; set; } = null!;
        public virtual ProductCategory ProductCategory { get; set; } = null!;
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ProductSizePricing> ProductSizePricings { get; set; }
    }
}
