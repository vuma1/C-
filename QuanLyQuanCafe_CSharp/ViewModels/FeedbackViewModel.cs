using MySql.Data.MySqlClient;
using QuanLyQuanCafe.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyQuanCafe.ViewModels
{
    public class FeedbackData
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

    public class FeedbackViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private int _starRating = 5;
        private string _productRating = "";
        private string _serviceRating = "";
        private string _otherComment = "";

        public ObservableCollection<FeedbackData> Feedbacks { get; } = new();

        public int StarRating
        {
            get => _starRating;
            set => SetProperty(ref _starRating, value);
        }

        public string ProductRating
        {
            get => _productRating;
            set => SetProperty(ref _productRating, value);
        }

        public string ServiceRating
        {
            get => _serviceRating;
            set => SetProperty(ref _serviceRating, value);
        }

        public string OtherComment
        {
            get => _otherComment;
            set => SetProperty(ref _otherComment, value);
        }

        public ICommand LoadCommand { get; }
        public ICommand SubmitCommand { get; }

        public FeedbackViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadFeedbacks());
            SubmitCommand = new RelayCommand(_ => SubmitFeedback());

            LoadFeedbacks();
        }

        private void LoadFeedbacks()
        {
            Feedbacks.Clear();
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "SELECT * FROM feedback ORDER BY id DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Feedbacks.Add(new FeedbackData
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

        private void SubmitFeedback()
        {
            using var conn = _db.GetConnection();
            if (conn == null) return;

            string sql = "INSERT INTO feedback (star_rating, product_rating, service_rating, other_comment) VALUES (@star, @prod, @serv, @comment)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@star", StarRating);
            cmd.Parameters.AddWithValue("@prod", ProductRating);
            cmd.Parameters.AddWithValue("@serv", ServiceRating);
            cmd.Parameters.AddWithValue("@comment", OtherComment);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Cảm ơn bạn đã gửi đánh giá!", "Thông báo");
            LoadFeedbacks();

            // Reset form
            StarRating = 5;
            ProductRating = "";
            ServiceRating = "";
            OtherComment = "";
        }
    }
}
