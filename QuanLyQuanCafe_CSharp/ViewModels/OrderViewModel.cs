using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Services;
using QuanLyQuanCafe.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private Table? _selectedTable;
        private Product? _selectedProduct;
        private int _quantity = 1;

        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Table> Tables { get; } = new();
        public ObservableCollection<OrderDetail> OrderItems { get; } = new();

        public Table? SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public double Total => OrderItems.Sum(x => x.Total);

        public ICommand LoadCommand { get; }
        public ICommand AddToOrderCommand { get; }
        public ICommand RemoveFromOrderCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand FreeTableCommand { get; }

        public OrderViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadData());
            AddToOrderCommand = new RelayCommand(_ => AddToOrder(), _ => SelectedProduct != null && Quantity > 0);
            RemoveFromOrderCommand = new RelayCommand(item => RemoveFromOrder(item as OrderDetail));
            ClearOrderCommand = new RelayCommand(_ => ClearOrder(), _ => OrderItems.Count > 0);
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => OrderItems.Count > 0 && SelectedTable != null);
            FreeTableCommand = new RelayCommand(_ => FreeTable(), _ => SelectedTable != null);

            LoadData();
        }

        private void LoadData()
        {
            LoadProducts();
            LoadTables();
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

        private void LoadTables()
        {
            Tables.Clear();
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "SELECT * FROM tables";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Tables.Add(new Table(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("status")));
            }
        }

        private void AddToOrder()
        {
            if (SelectedProduct == null || Quantity <= 0) return;

            var existing = OrderItems.FirstOrDefault(x => x.ProductName == SelectedProduct.Name);
            if (existing != null)
            {
                existing.UpdateQuantity(existing.Quantity + Quantity);
            }
            else
            {
                OrderItems.Add(new OrderDetail(SelectedProduct.Name, Quantity, SelectedProduct.Price));
            }

            OnPropertyChanged(nameof(Total));
            Quantity = 1;
        }

        private void RemoveFromOrder(OrderDetail? item)
        {
            if (item == null) return;
            OrderItems.Remove(item);
            OnPropertyChanged(nameof(Total));
        }

        private void ClearOrder()
        {
            OrderItems.Clear();
            OnPropertyChanged(nameof(Total));
        }

        private void Checkout()
        {
            if (OrderItems.Count == 0 || SelectedTable == null) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            double total = Total;

            // Insert order
            string sql = "INSERT INTO orders (total_amount, created_at) VALUES (@total, NOW())";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@total", total);
                cmd.ExecuteNonQuery();
            }

            // Update table status
            string sqlTable = "UPDATE tables SET status = 'Có khách' WHERE id = @id";
            using (var cmd = new MySqlCommand(sqlTable, conn))
            {
                cmd.Parameters.AddWithValue("@id", SelectedTable.Id);
                cmd.ExecuteNonQuery();
            }

            // Print invoice
            InvoicePrinter.PrintOrderInvoice(SelectedTable.Name, OrderItems.ToList(), total);

            MessageBox.Show($"Thanh toán thành công!\nTổng: {total:N0}đ", "Thông báo");
            ClearOrder();
            LoadTables();
        }

        private void FreeTable()
        {
            if (SelectedTable == null) return;

            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "UPDATE tables SET status = 'Trống' WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", SelectedTable.Id);
            cmd.ExecuteNonQuery();

            LoadTables();
            MessageBox.Show("Đã trả bàn!", "Thông báo");
        }
    }
}
