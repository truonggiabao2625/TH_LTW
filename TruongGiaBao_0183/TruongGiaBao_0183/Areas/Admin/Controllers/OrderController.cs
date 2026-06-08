using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TruongGiaBao_0183.DataAccess;

namespace TruongGiaBao_0183.Areas.Admin.Controllers
{
    // Xác định Controller thuộc phân vùng Admin
    [Area("Admin")]
    // Yêu cầu xác thực tài khoản có vai trò quản trị viên
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor tiêm db context xử lý dữ liệu Đơn hàng
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            // Sắp xếp đơn hàng mới nhất lên đầu
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // Xem chi tiết một đơn hàng cụ thể
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            return View(order);
        }

        // Cập nhật trạng thái đơn hàng (ví dụ: Processing, Shipped, Completed, Cancelled)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = status;
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{id} thành công!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // Xóa đơn hàng khỏi hệ thống
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa đơn hàng #{id} khỏi hệ thống!";
            return RedirectToAction(nameof(Index));
        }
    }
}
