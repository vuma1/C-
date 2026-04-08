using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Utils;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuanLyQuanCafe.Views
{
    public partial class OrderView : UserControl
    {
        private ObservableCollection<Product> productList;
        private ObservableCollection<OrderDetail> orderList;
        private Table? currentSelectedTable = null;
        private Product? selectedProduct = null;
        private Border? selectedProductCard = null;
        private Border? selectedTableButton = null;

        public OrderView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            productList = new ObservableCollection<Product>();
            orderList = new ObservableCollection<OrderDetail>();
            orderTable.ItemsSource = orderList;

            LoadProducts();
            LoadTablesFromDatabase();
        }

        private void LoadTablesFromDatabase()
        {
            tableContainer.Children.Clear();
            var tables = new List<Table>();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    string sql = "SELECT * FROM tables";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new Table(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetString("status")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải bàn: {ex.Message}");
            }

            foreach (var table in tables)
            {
                var btn = CreateTableButton(table);
                tableContainer.Children.Add(btn);
            }
        }

        private Border CreateTableButton(Table table)
        {
            var btn = new Border
            {
                Width = 80,
                Height = 50,
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(5),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = table
            };

            var text = new TextBlock
            {
                Text = table.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            btn.Child = text;

            // Set style based on status
            if (table.Status?.ToLower() == "có khách")
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDBDBD"));
                text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575"));
            }
            else
            {
                btn.Background = Brushes.White;
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E342E"));
                btn.BorderThickness = new Thickness(2);
                text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E342E"));
            }

            btn.MouseLeftButtonUp += (s, e) =>
            {
                // Prevent switching tables when bill already has items
                if (orderList.Count > 0 && currentSelectedTable != null && currentSelectedTable.Id != table.Id)
                {
                    ShowAlert("Không thể đổi bàn khi đã có món trong bill!\nVui lòng thanh toán hoặc xóa tất cả món trước.");
                    return;
                }

                // Deselect previous
                if (selectedTableButton != null)
                {
                    var prevTable = selectedTableButton.Tag as Table;
                    if (prevTable?.Status?.ToLower() != "có khách")
                    {
                        selectedTableButton.Background = Brushes.White;
                        ((TextBlock)selectedTableButton.Child).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E342E"));
                    }
                }

                // Select new
                currentSelectedTable = table;
                selectedTableButton = btn;
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E342E"));
                text.Foreground = Brushes.White;
                lblSelectedTableName.Text = table.Name;
            };

            return btn;
        }

        private void LoadProducts()
        {
            productList.Clear();
            productContainer.Children.Clear();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    string sql = "SELECT * FROM products";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product(
                                reader.GetInt32("id"),
                                reader.GetString("name"),
                                reader.GetDouble("price"));
                            productList.Add(product);
                            productContainer.Children.Add(CreateProductCard(product));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải sản phẩm: {ex.Message}");
            }
        }

        private Border CreateProductCard(Product product)
        {
            var card = new Border
            {
                Width = 120,
                Height = 75,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E1")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7CCC8")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = product
            };

            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

            var nameLabel = new TextBlock
            {
                Text = product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5D4037")),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };

            var priceLabel = new TextBlock
            {
                Text = $"{product.Price:N0} đ",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8D6E63")),
                TextAlignment = TextAlignment.Center
            };

            stack.Children.Add(nameLabel);
            stack.Children.Add(priceLabel);
            card.Child = stack;

            card.MouseLeftButtonUp += (s, e) =>
            {
                // Deselect previous
                if (selectedProductCard != null)
                {
                    selectedProductCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E1"));
                    selectedProductCard.BorderThickness = new Thickness(1);
                }

                // Select new
                selectedProduct = product;
                selectedProductCard = card;
                card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFEBE9"));
                card.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5D4037"));
                card.BorderThickness = new Thickness(2);
            };

            return card;
        }

        private void HandleAddToCart(object sender, RoutedEventArgs e)
        {
            if (currentSelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn trước khi thêm món!");
                return;
            }

            if (selectedProduct == null)
            {
                ShowAlert("Vui lòng chọn món ăn!");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity <= 0)
            {
                ShowAlert("Số lượng không hợp lệ!");
                return;
            }

            // Check if product already in order
            var existing = orderList.FirstOrDefault(x => x.ProductName == selectedProduct.Name);
            if (existing != null)
            {
                existing.UpdateQuantity(existing.Quantity + quantity);
                orderTable.Items.Refresh();
            }
            else
            {
                orderList.Add(new OrderDetail(selectedProduct.Name, quantity, selectedProduct.Price));
            }

            UpdateTotal();
            txtQuantity.Text = "1";
        }

        private void UpdateTotal()
        {
            double total = orderList.Sum(x => x.Total);
            lblTotalAmount.Text = $"{total:N0} VNĐ";
        }

        private void HandleRemoveItem(object sender, RoutedEventArgs e)
        {
            var selected = orderTable.SelectedItem as OrderDetail;
            if (selected == null)
            {
                ShowAlert("Vui lòng chọn món cần xóa!");
                return;
            }
            orderList.Remove(selected);
            UpdateTotal();
        }

        private void HandleClearOrder(object sender, RoutedEventArgs e)
        {
            if (orderList.Count == 0)
            {
                ShowAlert("Đơn hàng đang trống!");
                return;
            }
            orderList.Clear();
            UpdateTotal();
        }

        private void HandleFreeTable(object sender, RoutedEventArgs e)
        {
            if (currentSelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn cần trả!");
                return;
            }

            if (currentSelectedTable.Status?.ToLower() != "có khách")
            {
                ShowAlert("Bàn này đang trống, không cần trả!");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    string sql = "UPDATE tables SET status = 'Trống' WHERE id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", currentSelectedTable.Id);
                        cmd.ExecuteNonQuery();
                    }
                }

                currentSelectedTable = null;
                selectedTableButton = null;
                lblSelectedTableName.Text = "Chưa chọn bàn";
                LoadTablesFromDatabase();
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi khi trả bàn: {ex.Message}");
            }
        }

        private void HandleCheckout(object sender, RoutedEventArgs e)
        {
            if (orderList.Count == 0)
            {
                ShowAlert("Giỏ hàng đang trống!");
                return;
            }

            if (currentSelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn phục vụ trước khi thanh toán!");
                return;
            }

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    double total = orderList.Sum(x => x.Total);

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
                        cmd.Parameters.AddWithValue("@id", currentSelectedTable.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Print invoice
                    InvoicePrinter.PrintOrderInvoice(currentSelectedTable.Name, orderList.ToList(), total);

                    // Clear
                    orderList.Clear();
                    UpdateTotal();
                    currentSelectedTable = null;
                    selectedTableButton = null;
                    lblSelectedTableName.Text = "Chưa chọn bàn";
                    LoadTablesFromDatabase();
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi thanh toán: {ex.Message}");
            }
        }

        private void ShowAlert(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
