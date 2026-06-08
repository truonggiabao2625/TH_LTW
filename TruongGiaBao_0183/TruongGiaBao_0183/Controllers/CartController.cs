using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TruongGiaBao_0183.DataAccess;
using TruongGiaBao_0183.Extensions;
using TruongGiaBao_0183.Models;
using TruongGiaBao_0183.Repositories;

namespace TruongGiaBao_0183.Controllers
{
    // Controller xử lý giỏ hàng và thanh toán
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor thực hiện tiêm phụ thuộc (Dependency Injection)
        public CartController(IProductRepository productRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartItems(List<CartItem> cartItems)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);
        }

        // Hiển thị danh sách giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            // Tìm sản phẩm trong DB theo ID
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            // Lấy giỏ hàng hiện tại
            var cart = GetCartItems();
            
            // Tìm xem sản phẩm đã có trong giỏ hàng chưa
            var cartItem = cart.FirstOrDefault(c => c.Product.Id == id);

            if (cartItem == null)
            {
                // Nếu chưa có, thêm mới mục giỏ hàng
                cart.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                // Nếu đã có, cộng dồn số lượng
                cartItem.Quantity += quantity;
            }

            // Lưu lại giỏ hàng vào Session
            SaveCartItems(cart);

            // Kiểm tra nếu yêu cầu gửi lên là AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var totalQuantity = cart.Sum(item => item.Quantity);
                return Json(new { success = true, cartCount = totalQuantity });
            }

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng (hỗ trợ AJAX hoặc gọi trực tiếp)
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.Product.Id == id);
            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                SaveCartItems(cart);
            }
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.Product.Id == id);

            if (cartItem != null)
            {
                // Xóa mục ra khỏi danh sách
                cart.Remove(cartItem);
                SaveCartItems(cart);
            }

            return RedirectToAction("Index");
        }

        // Trang điền thông tin thanh toán (yêu cầu đăng nhập)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                // Giỏ hàng trống thì quay về trang giỏ hàng
                return RedirectToAction("Index");
            }

            // Lấy thông tin tài khoản đang đăng nhập
            var user = await _userManager.GetUserAsync(User);
            var order = new Order();

            if (user != null)
            {
                // Tự động điền trước các thông tin cá nhân đã có của tài khoản
                order.UserId = user.Id;
                order.FirstName = user.FullName?.Split(' ').LastOrDefault() ?? "";
                order.LastName = user.FullName?.Split(' ').FirstOrDefault() ?? "";
                order.Address = user.Address ?? "";
                order.PhoneNumber = user.PhoneNumber ?? "";
            }

            return View(order);
        }

        // Thực hiện thanh toán và lưu đơn hàng vào CSDL
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCartItems();
            if (cart == null || cart.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                return View(order);
            }

            // Lấy thông tin tài khoản đăng nhập để liên kết
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                order.UserId = user.Id;
            }

            // Thiết lập các trường hệ thống tự sinh
            order.OrderDate = DateTime.Now;
            order.TotalPrice = cart.Sum(item => item.Product.Price * item.Quantity);
            order.Status = "Pending";

            // Loại bỏ kiểm tra ModelState đối với User và OrderDetails vì chúng được thiết lập thủ công phía sau
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("OrderDetails");

            if (ModelState.IsValid)
            {
                // 1. Lưu thông tin Đơn hàng trước
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 2. Lưu chi tiết các sản phẩm trong đơn hàng
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
                await _context.SaveChangesAsync();

                // 3. Xóa giỏ hàng trong Session
                HttpContext.Session.Remove("Cart");

                // Chuyển hướng sang trang báo thành công
                return RedirectToAction("OrderCompleted");
            }

            // Nếu thông tin nhập vào không hợp lệ, tải lại form checkout
            return View(order);
        }

        // Trang thông báo đơn hàng thành công
        public IActionResult OrderCompleted()
        {
            return View();
        }
    }
}
