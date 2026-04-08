using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private string _username = "";
        private string _password = "";
        private string _role = "STAFF";
        private User? _selectedUser;

        public ObservableCollection<User> Users { get; } = new();
        public List<string> Roles { get; } = new() { "ADMIN", "STAFF" };

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value) && value != null)
                {
                    Username = value.Username;
                    Password = value.Password;
                    Role = value.Role;
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public UserViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadUsers());
            AddCommand = new RelayCommand(_ => AddUser());
            UpdateCommand = new RelayCommand(_ => UpdateUser(), _ => SelectedUser != null);
            DeleteCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "SELECT * FROM users";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Users.Add(new User(
                    reader.GetInt32("id"),
                    reader.GetString("username"),
                    reader.GetString("password"),
                    reader.GetString("role")));
            }
        }

        private bool CheckUsernameExists(string username)
        {
            using var conn = _db.GetConnection();
            if (conn == null) return false;

            string sql = "SELECT COUNT(*) FROM users WHERE username = @username";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            long count = (long)cmd.ExecuteScalar()!;
            return count > 0;
        }

        private void AddUser()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo");
                return;
            }

            if (CheckUsernameExists(Username))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!", "Thông báo");
                return;
            }

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", Username);
            cmd.Parameters.AddWithValue("@password", Password);
            cmd.Parameters.AddWithValue("@role", Role);
            cmd.ExecuteNonQuery();

            LoadUsers();
            ClearForm();
            MessageBox.Show("Thêm thành công!", "Thông báo");
        }

        private void UpdateUser()
        {
            if (SelectedUser == null) return;

            if (string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Tên đăng nhập không được để trống!", "Thông báo");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Thông báo");
                return;
            }

            // Lưu giá trị trước khi LoadUsers() có thể clear SelectedUser
            string originalUsername = SelectedUser.Username;
            string newUsername = Username;
            string newPassword = Password;
            string newRole = Role;

            // Kiểm tra nếu đổi username thì username mới không được trùng
            if (newUsername != originalUsername && CheckUsernameExists(newUsername))
            {
                MessageBox.Show("Tên đăng nhập mới đã tồn tại!", "Thông báo");
                return;
            }

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "UPDATE users SET username = @newUsername, password = @password, role = @role WHERE username = @originalUsername";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@newUsername", newUsername);
            cmd.Parameters.AddWithValue("@password", newPassword);
            cmd.Parameters.AddWithValue("@role", newRole);
            cmd.Parameters.AddWithValue("@originalUsername", originalUsername);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                LoadUsers();
                ClearForm();
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên để cập nhật!", "Thông báo");
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "DELETE FROM users WHERE username = @username";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", SelectedUser.Username);
            cmd.ExecuteNonQuery();

            LoadUsers();
            ClearForm();
        }

        private void ClearForm()
        {
            Username = "";
            Password = "";
            Role = "STAFF";
            SelectedUser = null;
        }
    }
}
