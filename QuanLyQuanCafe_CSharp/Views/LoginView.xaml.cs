using QuanLyQuanCafe.ViewModels;
using System.Windows;

namespace QuanLyQuanCafe.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            // Subscribe to login success event
            _viewModel.OnLoginSuccess += () =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };
        }

        private void HandleLogin(object sender, RoutedEventArgs e)
        {
            // Get password from PasswordBox (can't bind directly)
            _viewModel.Username = txtUser.Text.Trim();
            _viewModel.Password = txtPass.Password;

            _viewModel.LoginCommand.Execute(null);
        }
    }
}
