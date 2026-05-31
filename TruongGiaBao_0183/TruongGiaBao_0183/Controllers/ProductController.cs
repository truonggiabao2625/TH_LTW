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

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await LoadCategoriesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            RemoveFileAndNavigationValidation();

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
                    product.ProductImages = new List<ProductImage>();
                    foreach (var file in imageUrls.Where(file => file.Length > 0))
                    {
                        try
                        {
                            var savedPath = await SaveImage(file);
                            product.ImageUrls.Add(savedPath);
                            product.ProductImages.Add(new ProductImage { Url = savedPath });
                        }
                        catch (InvalidOperationException ex)
                        {
                            ModelState.AddModelError("ImageUrls", ex.Message);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productRepository.AddAsync(product);
                    return RedirectToAction("Index");
                }
            }

            await LoadCategoriesAsync();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            await LoadCategoriesAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            RemoveFileAndNavigationValidation();

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null) return NotFound();

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                if (imageUrl != null && imageUrl.Length > 0)
                {
                    try
                    {
                        existingProduct.ImageUrl = await SaveImage(imageUrl);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("ImageUrl", ex.Message);
                    }
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    if (existingProduct.ProductImages == null)
                    {
                        existingProduct.ProductImages = new List<ProductImage>();
                    }
                    foreach (var file in imageUrls.Where(file => file.Length > 0))
                    {
                        try
                        {
                            var savedPath = await SaveImage(file);
                            existingProduct.ProductImages.Add(new ProductImage { Url = savedPath });
                        }
                        catch (InvalidOperationException ex)
                        {
                            ModelState.AddModelError("ImageUrls", ex.Message);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    await _productRepository.UpdateAsync(existingProduct);
                    return RedirectToAction("Index");
                }
            }

            await LoadCategoriesAsync();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        private void RemoveFileAndNavigationValidation()
        {
            ModelState.Remove("imageUrl");
            ModelState.Remove("imageUrls");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageUrls");
            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
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
