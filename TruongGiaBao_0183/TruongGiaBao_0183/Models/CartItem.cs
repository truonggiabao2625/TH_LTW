namespace TruongGiaBao_0183.Models
{
    // Lớp đại diện cho một sản phẩm trong giỏ hàng
    public class CartItem
    {
        // Thông tin sản phẩm được thêm vào giỏ hàng
        public Product Product { get; set; }

        // Số lượng của sản phẩm này trong giỏ hàng
        public int Quantity { get; set; }
    }
}
