using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyQuanCafe.Views
{
    public partial class UserView : UserControl
    {
        private readonly UserViewModel _viewModel;

        public UserView()
        {
            InitializeComponent();
            _viewModel = new UserViewModel();
            DataContext = _viewModel;
            cbRole.SelectedIndex = 0;
        }

        private void OnUserSelected(object sender, SelectionChangedEventArgs e)
        {
            if (tableUsers.SelectedItem is Models.User user)
            {
                _viewModel.SelectedUser = user;
                txtUsername.Text = user.Username;
                txtPassword.Text = user.Password;
                cbRole.SelectedIndex = user.Role == "MANAGER" || user.Role == "ADMIN" ? 0 : 1;
            }
        }

        private void HandleAdd(object sender, RoutedEventArgs e)
        {
            _viewModel.Username = txtUsername.Text.Trim();
            _viewModel.Password = txtPassword.Text.Trim();
            _viewModel.Role = ((ComboBoxItem)cbRole.SelectedItem)?.Content?.ToString() ?? "STAFF";
            if (_viewModel.AddCommand.CanExecute(null))
            {
                _viewModel.AddCommand.Execute(null);
                HandleClear(null, null);
            }
        }

        private void HandleEdit(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo");
                return;
            }

            _viewModel.Username = txtUsername.Text.Trim();
            _viewModel.Password = txtPassword.Text.Trim();
            _viewModel.Role = ((ComboBoxItem)cbRole.SelectedItem)?.Content?.ToString() ?? "STAFF";
            _viewModel.UpdateCommand.Execute(null);
            HandleClear(null, null);
        }

        private void HandleDelete(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DeleteCommand.CanExecute(null))
            {
                _viewModel.DeleteCommand.Execute(null);
                HandleClear(null, null);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo");
            }
        }

        private void HandleClear(object? sender, RoutedEventArgs? e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            cbRole.SelectedIndex = 0;
            txtUsername.IsEnabled = true;
            tableUsers.SelectedItem = null;
            _viewModel.ClearCommand.Execute(null);
        }
    }
}
