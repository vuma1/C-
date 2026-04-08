using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyQuanCafe.Views
{
    public partial class ProductView : UserControl
    {
        private readonly ProductViewModel _viewModel;

        public ProductView()
        {
            InitializeComponent();
            _viewModel = new ProductViewModel();
            DataContext = _viewModel;
        }

        private void OnProductSelected(object sender, SelectionChangedEventArgs e)
        {
            if (tableProducts.SelectedItem is Product product)
            {
                _viewModel.SelectedProduct = product;
                txtName.Text = product.Name;
                txtPrice.Text = product.Price.ToString("N0");
            }
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void HandleAddProduct(object sender, RoutedEventArgs e)
        {
            _viewModel.ProductName = txtName.Text.Trim();
            if (double.TryParse(txtPrice.Text, out double price))
                _viewModel.ProductPrice = price;
            if (_viewModel.AddCommand.CanExecute(null))
            {
                _viewModel.AddCommand.Execute(null);
                ClearForm();
            }
        }

        private void HandleEditProduct(object sender, RoutedEventArgs e)
        {
            _viewModel.ProductName = txtName.Text.Trim();
            if (double.TryParse(txtPrice.Text, out double price))
                _viewModel.ProductPrice = price;
            if (_viewModel.UpdateCommand.CanExecute(null))
            {
                _viewModel.UpdateCommand.Execute(null);
                ClearForm();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món cần sửa!", "Thông báo");
            }
        }

        private void HandleDeleteProduct(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DeleteCommand.CanExecute(null))
            {
                _viewModel.DeleteCommand.Execute(null);
                ClearForm();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món cần xóa!", "Thông báo");
            }
        }

        private void HandleClear(object? sender, RoutedEventArgs? e)
        {
            ClearForm();
            _viewModel.ClearCommand.Execute(null);
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtPrice.Clear();
            tableProducts.SelectedItem = null;
        }
    }
}

