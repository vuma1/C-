using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyQuanCafe.Views
{
    public partial class TableView : UserControl
    {
        private readonly TableViewModel _viewModel;

        public TableView()
        {
            InitializeComponent();
            _viewModel = new TableViewModel();
            DataContext = _viewModel;
        }

        private void OnTableSelected(object sender, SelectionChangedEventArgs e)
        {
            if (tableList.SelectedItem is Table table)
            {
                _viewModel.SelectedTable = table;
                txtName.Text = table.Name;
            }
        }

        private void HandleAdd(object sender, RoutedEventArgs e)
        {
            _viewModel.TableName = txtName.Text.Trim();
            if (_viewModel.AddCommand.CanExecute(null))
            {
                _viewModel.AddCommand.Execute(null);
                txtName.Clear();
                tableList.SelectedItem = null;
            }
        }

        private void HandleEdit(object sender, RoutedEventArgs e)
        {
            _viewModel.TableName = txtName.Text.Trim();
            if (_viewModel.UpdateCommand.CanExecute(null))
            {
                _viewModel.UpdateCommand.Execute(null);
                txtName.Clear();
                tableList.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn cần sửa!", "Thông báo");
            }
        }

        private void HandleDelete(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DeleteCommand.CanExecute(null))
            {
                _viewModel.DeleteCommand.Execute(null);
                txtName.Clear();
                tableList.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa!", "Thông báo");
            }
        }

        private void HandleClear(object? sender, RoutedEventArgs? e)
        {
            txtName.Clear();
            tableList.SelectedItem = null;
            _viewModel.ClearCommand.Execute(null);
        }
    }
}
