using System.ComponentModel.DataAnnotations;

namespace TruongGiaBao_0183.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 1000000000)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string ImageUrl { get; set; } // Đường dẫn đến hình ảnh đại diện

        public List<string> ImageUrls { get; set; } // Danh sách các hình ảnh khác
    }
}
