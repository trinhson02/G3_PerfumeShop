namespace G3_PerfumeShop.Models
{
    using System;
    using System.Collections.Generic;

    public class ProductDetailViewModel
    {

        public Product Product { get; set; }
        public List<ProductSizePricing> SizePricings { get; set; }
        public decimal SelectedPrice { get; set; }
    }
}
