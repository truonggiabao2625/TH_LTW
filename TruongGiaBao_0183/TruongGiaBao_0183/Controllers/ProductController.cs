namespace TruongGiaBao_0183.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TruongGiaBao_0183.Models;
    using TruongGiaBao_0183.Repositories;

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            // Loại bỏ kiểm tra ModelState cho các trường ảnh vì chúng ta xử lý file riêng biệt
            ModelState.Remove("imageUrl");
            ModelState.Remove("imageUrls");

            if (ModelState.IsValid)
            {
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    try
                    {
                        product.ImageUrl = await SaveImage(imageUrl);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("ImageUrl", ex.Message);
                    }
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in imageUrls.Where(file => file.Length > 0))
                    {
                        try
                        {
                            product.ImageUrls.Add(await SaveImage(file));
                        }
                        catch (InvalidOperationException ex)
                        {
                            ModelState.AddModelError("ImageUrls", ex.Message);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    _productRepository.Add(product);
                    return RedirectToAction("Index");
                }
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageUrl)
        {
            ModelState.Remove("imageUrl");

            if (ModelState.IsValid)
            {
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    try
                    {
                        product.ImageUrl = await SaveImage(imageUrl);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("ImageUrl", ex.Message);
                    }
                }
                else
                {
                    // Nếu không chọn ảnh mới, giữ lại đường dẫn ảnh cũ từ database
                    var existingProduct = _productRepository.GetById(product.Id);
                    product.ImageUrl = existingProduct?.ImageUrl;
                }

                if (ModelState.IsValid)
                {
                    _productRepository.Update(product);
                    return RedirectToAction("Index");
                }
            }

            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Chỉ được upload ảnh định dạng .jpg, .jpeg, .png, .gif hoặc .webp.");
            }

            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var folderPath = Path.Combine(webRootPath, "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }
    }
}
