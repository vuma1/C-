using QuanLyQuanCafe.Models;

namespace QuanLyQuanCafe.Utils
{
    public static class UserSession
    {
        private static User? _currentUser; // Biến tĩnh (static) để lưu giữ liệu toàn cục

        // Lấy user hiện tại
        public static User? GetCurrentUser()
        {
            return _currentUser;
        }

        // Lưu user khi đăng nhập
        public static void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        // Xóa user khi đăng xuất
        public static void Clear()
        {
            _currentUser = null;
        }
    }
}
