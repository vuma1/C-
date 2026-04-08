using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class StatsViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private bool _isDailyView = true;

        public ObservableCollection<(string Label, double Value)> ChartData { get; } = new();

        public bool IsDailyView
        {
            get => _isDailyView;
            set
            {
                if (SetProperty(ref _isDailyView, value))
                    LoadStats();
            }
        }

        public ICommand LoadDailyCommand { get; }
        public ICommand LoadMonthlyCommand { get; }

        public StatsViewModel()
        {
            LoadDailyCommand = new RelayCommand(_ => { IsDailyView = true; });
            LoadMonthlyCommand = new RelayCommand(_ => { IsDailyView = false; });

            LoadStats();
        }

        public void LoadStats()
        {
            ChartData.Clear();
            var data = IsDailyView ? GetDailyStats() : GetMonthlyStats();
            foreach (var item in data)
            {
                ChartData.Add(item);
            }
        }

        private List<(string Label, double Value)> GetDailyStats()
        {
            var data = new List<(string, double)>();
            using var conn = _db.GetConnection();
            if (conn == null) return data;

            string sql = @"SELECT DATE_FORMAT(created_at, '%d/%m') as report_date, SUM(total_amount) as total 
                           FROM orders 
                           WHERE DATE(created_at) >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) 
                           GROUP BY DATE(created_at) 
                           ORDER BY DATE(created_at) ASC";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                data.Add((reader.GetString("report_date"), reader.GetDouble("total")));
            }
            return data;
        }

        private List<(string Label, double Value)> GetMonthlyStats()
        {
            var data = new List<(string, double)>();
            using var conn = _db.GetConnection();
            if (conn == null) return data;

            string sql = @"SELECT DATE_FORMAT(created_at, '%m/%Y') as report_month, SUM(total_amount) as total 
                           FROM orders 
                           WHERE created_at >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH) 
                           GROUP BY DATE_FORMAT(created_at, '%Y-%m') 
                           ORDER BY MIN(created_at) ASC";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                data.Add((reader.GetString("report_month"), reader.GetDouble("total")));
            }
            return data;
        }
    }
}
