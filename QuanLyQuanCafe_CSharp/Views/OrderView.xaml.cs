using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuanLyQuanCafe.Views
{
    public partial class OrderView : UserControl
    {
        private readonly OrderViewModel _viewModel;
        private Border? selectedProductCard = null;
        private Border? selectedTableButton = null;

        public OrderView()
        {
            InitializeComponent();
            _viewModel = new OrderViewModel();
            DataContext = _viewModel;

            // Bind DataGrid
            orderTable.ItemsSource = _viewModel.OrderItems;

            // Subscribe events từ ViewModel
            _viewModel.OnProductsReloaded += BuildProductCards;
            _viewModel.OnTablesReloaded += BuildTableButtons;
            _viewModel.OnCheckoutSuccess += ResetTableSelection;
            _viewModel.OnOrderCleared += () => { };

            // Vẽ giao diện động lần đầu
            BuildProductCards();
            BuildTableButtons();
        }

        // ===== TẠO GIAO DIỆN ĐỘNG (UI-ONLY) =====

        private void BuildTableButtons()
        {
            tableContainer.Children.Clear();
            selectedTableButton = null;

            foreach (var table in _viewModel.Tables)
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
                if (_viewModel.OrderItems.Count > 0 && _viewModel.SelectedTable != null && _viewModel.SelectedTable.Id != table.Id)
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

                // Select new → gán vào ViewModel
                _viewModel.SelectedTable = table;
                selectedTableButton = btn;
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4E342E"));
                text.Foreground = Brushes.White;
                lblSelectedTableName.Text = table.Name;
            };

            return btn;
        }

        private void BuildProductCards()
        {
            productContainer.Children.Clear();
            selectedProductCard = null;

            foreach (var product in _viewModel.Products)
            {
                productContainer.Children.Add(CreateProductCard(product));
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

                // Select new → gán vào ViewModel
                _viewModel.SelectedProduct = product;
                selectedProductCard = card;
                card.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFEBE9"));
                card.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5D4037"));
                card.BorderThickness = new Thickness(2);
            };

            return card;
        }

        // ===== EVENT HANDLERS → GỌI VIEWMODEL =====

        private void HandleAddToCart(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn trước khi thêm món!");
                return;
            }

            if (_viewModel.SelectedProduct == null)
            {
                ShowAlert("Vui lòng chọn món ăn!");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity) || quantity <= 0)
            {
                ShowAlert("Số lượng không hợp lệ!");
                return;
            }

            _viewModel.Quantity = quantity;
            _viewModel.AddToOrderCommand.Execute(null);
            orderTable.Items.Refresh();
            UpdateTotal();
            txtQuantity.Text = "1";
        }

        private void HandleRemoveItem(object sender, RoutedEventArgs e)
        {
            var selected = orderTable.SelectedItem as OrderDetail;
            if (selected == null)
            {
                ShowAlert("Vui lòng chọn món cần xóa!");
                return;
            }
            _viewModel.RemoveFromOrderCommand.Execute(selected);
            UpdateTotal();
        }

        private void HandleClearOrder(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OrderItems.Count == 0)
            {
                ShowAlert("Đơn hàng đang trống!");
                return;
            }
            _viewModel.ClearOrderCommand.Execute(null);
            UpdateTotal();
        }

        private void HandleFreeTable(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn cần trả!");
                return;
            }
            _viewModel.FreeTableCommand.Execute(null);
            ResetTableSelection();
        }

        private void HandleCheckout(object sender, RoutedEventArgs e)
        {
            if (_viewModel.OrderItems.Count == 0)
            {
                ShowAlert("Giỏ hàng đang trống!");
                return;
            }

            if (_viewModel.SelectedTable == null)
            {
                ShowAlert("Vui lòng chọn bàn phục vụ trước khi thanh toán!");
                return;
            }

            _viewModel.CheckoutCommand.Execute(null);
            UpdateTotal();
        }

        // ===== UI HELPERS =====

        private void UpdateTotal()
        {
            lblTotalAmount.Text = $"{_viewModel.Total:N0} VNĐ";
        }

        private void ResetTableSelection()
        {
            selectedTableButton = null;
            lblSelectedTableName.Text = "Chưa chọn bàn";
            UpdateTotal();
        }

        private void ShowAlert(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
