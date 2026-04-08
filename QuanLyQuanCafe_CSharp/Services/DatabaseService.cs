using MySql.Data.MySqlClient;
using System.Data;

namespace QuanLyQuanCafe.Services
{
    /// <summary>
    /// Service xử lý kết nối và truy vấn database
    /// </summary>
    public class DatabaseService
    {
        private static readonly string ConnectionString =
            "Server=localhost;Database=coffee_shop_db;Uid=root;Pwd=;SslMode=none;AllowPublicKeyRetrieval=true;";

        /// <summary>
        /// Tạo và mở kết nối database
        /// </summary>
        public MySqlConnection? GetConnection()
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
                    $"Không thể kết nối đến MySQL!\n\nLỗi: {ex.Message}",
                    "Lỗi kết nối CSDL",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Thực thi query và trả về DataTable
        /// </summary>
        public DataTable? ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            using var conn = GetConnection();
            if (conn == null) return null;

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);

            var dt = new DataTable();
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Thực thi INSERT/UPDATE/DELETE
        /// </summary>
        public int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using var conn = GetConnection();
            if (conn == null) return -1;

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Thực thi và lấy giá trị đơn
        /// </summary>
        public object? ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using var conn = GetConnection();
            if (conn == null) return null;

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }
    }
}
