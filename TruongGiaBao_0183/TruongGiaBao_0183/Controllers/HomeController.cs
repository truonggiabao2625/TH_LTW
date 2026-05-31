using Microsoft.AspNetCore.Mvc;
using TruongGiaBao_0183.Repositories;

namespace TruongGiaBao_0183.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            
            ViewBag.Categories = categories;

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
                
                var selectedCat = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                ViewBag.SelectedCategoryName = selectedCat?.Name;
            }

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
