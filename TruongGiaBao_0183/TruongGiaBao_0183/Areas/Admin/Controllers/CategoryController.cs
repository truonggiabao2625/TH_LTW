using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TruongGiaBao_0183.Models;
using TruongGiaBao_0183.Repositories;

namespace TruongGiaBao_0183.Areas.Admin.Controllers
{
    // Xác định Controller thuộc phân vùng Admin
    [Area("Admin")]
    // Yêu cầu xác thực tài khoản có vai trò quản trị viên
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        // Constructor tiêm repository xử lý danh mục
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Trang danh sách danh mục
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // Form thêm mới danh mục (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm mới danh mục (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Form cập nhật danh mục (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }
            return View(category);
        }

        // Xử lý cập nhật danh mục (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Xác nhận xóa danh mục (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Danh mục không tồn tại.");
            }
            return View(category);
        }

        // Xử lý xóa danh mục thực tế (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
