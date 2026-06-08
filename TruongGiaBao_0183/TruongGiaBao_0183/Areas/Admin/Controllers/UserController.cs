using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TruongGiaBao_0183.Models;

namespace TruongGiaBao_0183.Areas.Admin.Controllers
{
    // Xác định Controller thuộc phân vùng Admin
    [Area("Admin")]
    // Yêu cầu xác thực tài khoản có vai trò quản trị viên
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor tiêm dịch vụ quản lý người dùng và vai trò
        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Trang danh sách thành viên
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRolesViewModelList = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = await _userManager.GetRolesAsync(user)
                };
                userRolesViewModelList.Add(thisViewModel);
            }

            // Truyền tất cả các roles qua ViewBag để hiển thị dropdown thay đổi role
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(userRolesViewModelList);
        }

        // Cập nhật vai trò (Role) của thành viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Thành viên không tồn tại.");
            }

            // Ngăn quản trị viên tự thay đổi vai trò của chính mình để tránh mất quyền Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự thay đổi vai trò quản trị của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra role mới có hợp lệ và tồn tại trong hệ thống không
            var roleExists = await _roleManager.RoleExistsAsync(newRole);
            if (!roleExists)
            {
                TempData["ErrorMessage"] = "Vai trò mới không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách các vai trò hiện tại của user
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Gỡ bỏ các vai trò cũ nếu có
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Không thể gỡ bỏ vai trò hiện tại.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Gán vai trò mới chọn
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Không thể gán vai trò mới cho thành viên.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Đã cập nhật vai trò của thành viên thành công sang '{newRole}'.";
            return RedirectToAction(nameof(Index));
        }

        // Xóa thành viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Thành viên không tồn tại.");
            }

            // Ngăn quản trị viên tự xóa tài khoản của chính mình
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự xóa tài khoản của chính mình khi đang đăng nhập!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Không thể xóa thành viên này.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
