using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private string _productName = "";
        private double _productPrice;
        private Product? _selectedProduct;

        public ObservableCollection<Product> Products { get; } = new();

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public double ProductPrice
        {
            get => _productPrice;
            set => SetProperty(ref _productPrice, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value) && value != null)
                {
                    ProductName = value.Name;
                    ProductPrice = value.Price;
                }
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        public ProductViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadProducts());
            AddCommand = new RelayCommand(_ => AddProduct());
            UpdateCommand = new RelayCommand(_ => UpdateProduct(), _ => SelectedProduct != null);
            DeleteCommand = new RelayCommand(_ => DeleteProduct(), _ => SelectedProduct != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadProducts();
        }

        private void LoadProducts()
        {
            Products.Clear();
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "SELECT * FROM products";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Products.Add(new Product(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetDouble("price")));
            }
        }

        private void AddProduct()
        {
            if (string.IsNullOrEmpty(ProductName))
            {
                MessageBox.Show("Vui lòng nhập tên món!", "Thông báo");
                return;
            }

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "INSERT INTO products (name, price) VALUES (@name, @price)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", ProductName);
            cmd.Parameters.AddWithValue("@price", ProductPrice);
            cmd.ExecuteNonQuery();

            LoadProducts();
            ClearForm();
            MessageBox.Show("Thêm thành công!", "Thông báo");
        }

        private void UpdateProduct()
        {
            if (SelectedProduct == null) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "UPDATE products SET name = @name, price = @price WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", ProductName);
            cmd.Parameters.AddWithValue("@price", ProductPrice);
            cmd.Parameters.AddWithValue("@id", SelectedProduct.Id);
            cmd.ExecuteNonQuery();

            LoadProducts();
            ClearForm();
            MessageBox.Show("Cập nhật thành công!", "Thông báo");
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "DELETE FROM products WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", SelectedProduct.Id);
            cmd.ExecuteNonQuery();

            LoadProducts();
            ClearForm();
        }

        private void ClearForm()
        {
            ProductName = "";
            ProductPrice = 0;
            SelectedProduct = null;
        }
    }
}
