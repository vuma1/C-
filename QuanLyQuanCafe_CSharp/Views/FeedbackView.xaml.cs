using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyQuanCafe.Views
{
    public class FeedbackDisplay
    {
        public int Id { get; set; }
        public int StarRating { get; set; }
        public string ProductRating { get; set; } = "";
        public string ServiceRating { get; set; } = "";
        public string OtherComment { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public string DateStr => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string StarStr => $"{StarRating} ⭐";
    }

    public partial class FeedbackView : UserControl
    {
        private ObservableCollection<FeedbackDisplay> feedbackList;

        public FeedbackView()
        {
            InitializeComponent();
            feedbackList = new ObservableCollection<FeedbackDisplay>();
            tableFeedback.ItemsSource = feedbackList;
            LoadData();
        }

        private void LoadData()
        {
            feedbackList.Clear();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    string sql = "SELECT * FROM feedback ORDER BY id DESC";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            feedbackList.Add(new FeedbackDisplay
                            {
                                Id = reader.GetInt32("id"),
                                StarRating = reader.GetInt32("star_rating"),
                                ProductRating = reader.GetString("product_rating"),
                                ServiceRating = reader.GetString("service_rating"),
                                OtherComment = reader.GetString("other_comment"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void HandleSubmit(object sender, RoutedEventArgs e)
        {
            // Get star rating
            int stars = 5;
            foreach (var child in ratingPanel.Children)
            {
                if (child is RadioButton rb && rb.IsChecked == true)
                {
                    stars = int.Parse(rb.Tag.ToString() ?? "5");
                    break;
                }
            }

            string prod = GetCheckboxString(cbProdTasty, cbProdBeautiful, cbProdTemperature, cbProdPrice);
            string serv = GetCheckboxString(cbServFast, cbServFriendly, cbServClean, cbServWifi);
            string comment = txtComment.Text.Trim();

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    if (conn == null) return;
                    string sql = "INSERT INTO feedback (star_rating, product_rating, service_rating, other_comment) VALUES (@star, @prod, @serv, @comment)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@star", stars);
                        cmd.Parameters.AddWithValue("@prod", prod);
                        cmd.Parameters.AddWithValue("@serv", serv);
                        cmd.Parameters.AddWithValue("@comment", comment);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Cảm ơn đánh giá {stars} sao của bạn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private string GetCheckboxString(params CheckBox[] boxes)
        {
            var selected = new List<string>();
            foreach (var box in boxes)
            {
                if (box.IsChecked == true)
                {
                    selected.Add(box.Content?.ToString() ?? "");
                }
            }
            return string.Join(", ", selected);
        }

        private void ResetForm()
        {
            rb5.IsChecked = true;
            cbProdTasty.IsChecked = false;
            cbProdBeautiful.IsChecked = false;
            cbProdTemperature.IsChecked = false;
            cbProdPrice.IsChecked = false;
            cbServFast.IsChecked = false;
            cbServFriendly.IsChecked = false;
            cbServClean.IsChecked = false;
            cbServWifi.IsChecked = false;
            txtComment.Clear();
        }
    }
}
