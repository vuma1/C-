using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;

namespace QuanLyQuanCafe.Services
{
    /// <summary>
    /// Service xử lý xác thực người dùng
    /// </summary>
    public class AuthService
    {
        private readonly DatabaseService _db = new();

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        public User? Login(string username, string password)
        {
            using var conn = _db.GetConnection();
            if (conn == null) return null;

            string sql = "SELECT * FROM users WHERE username = @username AND password = @password";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    reader.GetInt32("id"),
                    reader.GetString("username"),
                    reader.GetString("password"),
                    reader.GetString("role"));
            }
            return null;
        }
    }
}
