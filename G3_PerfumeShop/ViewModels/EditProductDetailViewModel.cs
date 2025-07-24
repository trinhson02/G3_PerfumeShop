using System.ComponentModel.DataAnnotations;

namespace G3_PerfumeShop.Models
{
    public class EditProductDetailViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Năm phát hành không hợp lệ.")]
        [Required(ErrorMessage = "Năm phát hành không được để trống.")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Độ bền không được để trống.")]
        public int Longevity { get; set; }

        [Required(ErrorMessage = "Nồng độ không được để trống.")]
        public int Concentration { get; set; }

        [Required(ErrorMessage = "Điểm không được để trống.")]
        public int Point { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Thương hiệu không được để trống.")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Nơi xuất xứ không được để trống.")]
        public int OriginId { get; set; }

        public List<Gender> Genders { get; set; } = new List<Gender>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Origin> Origins { get; set; } = new List<Origin>();
    }
}

