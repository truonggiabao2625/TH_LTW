using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TruongGiaBao_0183.Models
{
    // Lớp định nghĩa bảng Chi tiết đơn hàng (OrderDetail)
    public class OrderDetail
    {
        // Khóa chính của chi tiết đơn hàng
        public int Id { get; set; }

        // Khóa ngoại liên kết tới bảng Orders
        public int OrderId { get; set; }

        // Thuộc tính điều hướng liên kết tới thực thể Order tương ứng
        [ValidateNever]
        public Order Order { get; set; }

        // Khóa ngoại liên kết tới bảng Products
        public int ProductId { get; set; }

        // Thuộc tính điều hướng liên kết tới thực thể Product tương ứng
        [ValidateNever]
        public Product Product { get; set; }

        // Số lượng sản phẩm mua trong chi tiết đơn hàng này
        public int Quantity { get; set; }

        // Giá bán của sản phẩm tại thời điểm mua (lưu cố định tránh giá sản phẩm thay đổi sau này)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
