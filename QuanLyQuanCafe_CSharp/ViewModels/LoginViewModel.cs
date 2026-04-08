using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Services;
using QuanLyQuanCafe.Utils;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService = new();

        private string _username = "";
        private string _password = "";
        private string _errorMessage = "";

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username);
        }

        private void ExecuteLogin(object? parameter)
        {
            ErrorMessage = "";

            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = "⚠ Vui lòng nhập tên đăng nhập!";
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "⚠ Vui lòng nhập mật khẩu!";
                return;
            }

            User? user = _authService.Login(Username, Password);

            if (user == null)
            {
                ErrorMessage = "❌ Tên đăng nhập hoặc mật khẩu không đúng!";
                Password = "";
                return;
            }

            // Đăng nhập thành công
            UserSession.SetCurrentUser(user);
            OnLoginSuccess?.Invoke();
        }

        /// <summary>
        /// Event khi đăng nhập thành công, View sẽ subscribe để mở MainWindow
        /// </summary>
        public event Action? OnLoginSuccess;
    }
}
