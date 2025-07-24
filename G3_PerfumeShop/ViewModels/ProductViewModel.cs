namespace G3_PerfumeShop.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int ProductSizePricingId { get; set; }
        public string Title { get; set; }
        public string ProductName { get; set; }
        public int Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } // Image from OrderDetail, if any
        public string ProductImageUrl { get; set; } // Default image from Product table

        // Thuộc tính tính toán để hiển thị ảnh
        public string DisplayImageUrl => !string.IsNullOrEmpty(ImageUrl) ? ImageUrl : ProductImageUrl;
    }
}
