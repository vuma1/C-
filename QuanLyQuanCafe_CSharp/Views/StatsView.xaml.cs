using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuanLyQuanCafe.Views
{
    public partial class StatsView : UserControl
    {
        private readonly StatsViewModel _viewModel;
        private double niceMaxValue = 0;

        public StatsView()
        {
            InitializeComponent();
            _viewModel = new StatsViewModel();
            DataContext = _viewModel;

            // Subscribe event khi ViewModel load xong data → vẽ lại biểu đồ
            _viewModel.OnDataLoaded += RefreshChart;

            Loaded += StatsView_Loaded;
        }

        private void StatsView_Loaded(object sender, RoutedEventArgs e)
        {
            // ViewModel đã load data trong constructor, chỉ cần vẽ chart
            RefreshChart();
        }

        /// <summary>
        /// Đọc data từ ViewModel, tính BarHeight, rồi vẽ biểu đồ
        /// </summary>
        private void RefreshChart()
        {
            var chartData = _viewModel.ChartItems;

            // Tìm giá trị max
            double maxValue = 0;
            foreach (var item in chartData)
            {
                if (item.Value > maxValue) maxValue = item.Value;
            }

            // Làm tròn max để trục Y đẹp
            niceMaxValue = RoundUpToNice(maxValue);

            // Lấy chiều cao thực tế của vùng biểu đồ
            double chartHeight = yAxisLabels.ActualHeight > 0 ? yAxisLabels.ActualHeight : 400;

            // Tính chiều cao thanh cho từng mục
            foreach (var item in chartData)
            {
                item.BarHeight = niceMaxValue > 0 ? (item.Value / niceMaxValue) * chartHeight : 0;
            }

            // Gán dữ liệu cho ItemsControl
            chartDaily.ItemsSource = null;
            chartDaily.ItemsSource = chartData;
            chartLabels.ItemsSource = null;
            chartLabels.ItemsSource = chartData;

            DrawYAxisAndGridLines();
        }

        private void DrawYAxisAndGridLines()
        {
            yAxisLabels.Children.Clear();
            yAxisLabels.RowDefinitions.Clear();
            gridLinesCanvas.Children.Clear();

            if (niceMaxValue <= 0) return;

            int numLabels = 6;
            double step = niceMaxValue / (numLabels - 1);

            // Create row definitions for Y-axis grid
            for (int i = 0; i < numLabels; i++)
            {
                yAxisLabels.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Create Y-axis labels - từ max (row 0) đến 0 (row cuối)
            for (int i = 0; i < numLabels; i++)
            {
                double value = niceMaxValue - (step * i);
                var label = new TextBlock
                {
                    Text = $"{value:N0}",
                    FontSize = 11,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666")),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0, 0, 8, 0)
                };

                // Nhãn cuối cùng (0) căn dưới
                if (i == numLabels - 1)
                {
                    label.VerticalAlignment = VerticalAlignment.Bottom;
                }

                Grid.SetRow(label, i);
                yAxisLabels.Children.Add(label);
            }

            // Draw horizontal grid lines
            double canvasWidth = chartArea.ActualWidth > 0 ? chartArea.ActualWidth : 1000;
            double chartHeight = yAxisLabels.ActualHeight > 0 ? yAxisLabels.ActualHeight : 400;
            double lineSpacing = chartHeight / (numLabels - 1);

            for (int i = 1; i < numLabels; i++)
            {
                double y = i * lineSpacing;
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                    StrokeThickness = 1
                };
                gridLinesCanvas.Children.Add(line);
            }
        }

        private double RoundUpToNice(double value)
        {
            if (value <= 0) return 100000;

            double magnitude = Math.Pow(10, Math.Floor(Math.Log10(value)));
            double normalized = value / magnitude;

            if (normalized <= 1) return magnitude;
            if (normalized <= 2) return 2 * magnitude;
            if (normalized <= 5) return 5 * magnitude;
            return 10 * magnitude;
        }
    }
}
