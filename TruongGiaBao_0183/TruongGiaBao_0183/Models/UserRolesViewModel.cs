using System.Collections.Generic;

namespace TruongGiaBao_0183.Models
{
    // Model hiển thị danh sách người dùng kèm các vai trò của họ
    public class UserRolesViewModel
    {
        // Mã định danh người dùng
        public string Id { get; set; }

        // Email người dùng
        public string Email { get; set; }

        // Tên đầy đủ
        public string FullName { get; set; }

        // Danh sách các vai trò (Roles) được gán
        public IList<string> Roles { get; set; }
    }
}
