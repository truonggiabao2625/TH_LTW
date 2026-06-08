using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TruongGiaBao_0183.Models
{
    // Lớp định nghĩa bảng Đơn hàng (Order)
    public class Order
    {
        // Khóa chính của đơn hàng
        public int Id { get; set; }

        // Mã định danh của người dùng đặt hàng (liên kết với IdentityUser)
        public string UserId { get; set; }

        // Thuộc tính điều hướng liên kết với thông tin tài khoản người dùng
        [ValidateNever]
        public ApplicationUser User { get; set; }

        // Ngày đặt hàng
        public DateTime OrderDate { get; set; }

        // Tổng giá trị đơn hàng
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Tên (First Name) người nhận hàng
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50, ErrorMessage = "Tên không được dài quá 50 ký tự")]
        public string FirstName { get; set; }

        // Họ (Last Name) người nhận hàng
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        [StringLength(50, ErrorMessage = "Họ không được dài quá 50 ký tự")]
        public string LastName { get; set; }

        // Địa chỉ giao nhận hàng
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(150, ErrorMessage = "Địa chỉ không được dài quá 150 ký tự")]
        public string Address { get; set; }

        // Thành phố giao nhận hàng
        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        [StringLength(50, ErrorMessage = "Thành phố không được dài quá 50 ký tự")]
        public string City { get; set; }

        // Số điện thoại người nhận hàng
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại nhận hàng")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        // Ghi chú của khách hàng khi đặt hàng
        [StringLength(500, ErrorMessage = "Ghi chú không dài quá 500 ký tự")]
        public string Notes { get; set; }

        // Trạng thái đơn hàng (Ví dụ: Chờ xử lý, Đang giao, Đã giao, Đã hủy)
        public string Status { get; set; } = "Pending";

        // Danh sách chi tiết các mặt hàng trong đơn hàng này
        [ValidateNever]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
