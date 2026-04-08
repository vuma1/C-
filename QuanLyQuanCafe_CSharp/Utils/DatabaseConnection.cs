using MySql.Data.MySqlClient;
using System;

namespace QuanLyQuanCafe.Utils
{
    public static class DatabaseConnection
    {
        private static readonly string ConnectionString =
            "Server=localhost;Database=coffee_shop_db;Uid=root;Pwd=;SslMode=none;AllowPublicKeyRetrieval=true;";

        public static MySqlConnection? GetConnection()
        {
            try
            {
                var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show(
                    $"Không thể kết nối đến MySQL!\n\nVui lòng kiểm tra:\n• XAMPP MySQL đã chạy chưa?\n• Database 'coffee_shop_db' đã tồn tại chưa?\n\nLỗi: {ex.Message}",
                    "Lỗi kết nối CSDL",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Lỗi không xác định: {ex.Message}",
                    "Lỗi",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return null;
            }
        }
    }
}
