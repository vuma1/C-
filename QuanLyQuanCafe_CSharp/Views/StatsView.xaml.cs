using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuanLyQuanCafe.Views
{
    public class ChartData
    {
        public string Label { get; set; } = "";
        public double Value { get; set; }
        public double BarHeight { get; set; }
    }

    public partial class StatsView : UserControl
    {
        private double maxChartValue = 0;
        private double niceMaxValue = 0;
        private const double CHART_HEIGHT = 400;

        public StatsView()
        {
            InitializeComponent();
            Loaded += StatsView_Loaded;
        }

        private void StatsView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDailyStats();
        }

        private void LoadDailyStats()
        {
            var data = new ObservableCollection<ChartData>();
            maxChartValue = 0;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;

                    string sql = @"SELECT DATE_FORMAT(created_at, '%d/%m') as report_date, SUM(total_amount) as total 
                                   FROM orders 
                                   WHERE DATE(created_at) >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) 
                                   GROUP BY DATE(created_at) 
                                   ORDER BY DATE(created_at) ASC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string date = reader.GetString("report_date");
                            double total = reader.GetDouble("total");
                            data.Add(new ChartData { Label = date, Value = total });
                            if (total > maxChartValue) maxChartValue = total;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thống kê ngày: {ex.Message}");
            }

            // Round up max value to nice number for Y-axis
            niceMaxValue = RoundUpToNice(maxChartValue);

            // Get actual chart height from layout
            double chartHeight = yAxisLabels.ActualHeight > 0 ? yAxisLabels.ActualHeight : 400;

            // Calculate bar heights based on niceMaxValue
            foreach (var item in data)
            {
                item.BarHeight = niceMaxValue > 0 ? (item.Value / niceMaxValue) * chartHeight : 0;
            }

            chartDaily.ItemsSource = data;
            chartLabels.ItemsSource = data;
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
