using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace TruongGiaBao_0183.Extensions
{
    // Lớp chứa các phương thức mở rộng (Extension Methods) cho Session
    public static class SessionExtensions
    {
        // Tuần tự hóa một đối tượng thành chuỗi JSON và lưu vào Session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // Chuyển đối tượng sang định dạng chuỗi JSON
            var jsonString = JsonSerializer.Serialize(value);
            // Lưu chuỗi JSON vào session theo Key tương ứng
            session.SetString(key, jsonString);
        }

        // Đọc chuỗi JSON từ Session và giải tuần tự hóa ngược lại thành đối tượng
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            // Lấy chuỗi JSON từ session theo Key
            var value = session.GetString(key);
            // Nếu không tìm thấy, trả về giá trị mặc định của kiểu T (thường là null)
            // Ngược lại, giải tuần tự hóa chuỗi JSON về kiểu dữ liệu T ban đầu
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
