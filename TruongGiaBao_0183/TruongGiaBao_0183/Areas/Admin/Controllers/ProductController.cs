using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruongGiaBao_0183.Repositories;
using TruongGiaBao_0183.Models;

namespace TruongGiaBao_0183.Areas.Admin.Controllers
{
    // Xác định Controller này thuộc phân vùng (Area) Admin
    [Area("Admin")]
    // Chỉ cho phép người dùng đăng nhập có vai trò "Admin" truy cập
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        // Khởi tạo Controller và inject Product Repository
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Action hiển thị danh sách sản phẩm dành riêng cho quản trị viên
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu bất đồng bộ
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
    }
}
