using QuanLyQuanCafe.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyQuanCafe.Views
{
    public partial class FeedbackView : UserControl
    {
        private readonly FeedbackViewModel _viewModel;

        public FeedbackView()
        {
            InitializeComponent();
            _viewModel = new FeedbackViewModel();
            DataContext = _viewModel;
        }

        private void HandleSubmit(object sender, RoutedEventArgs e)
        {
            // Get star rating from RadioButtons
            int stars = 5;
            foreach (var child in ratingPanel.Children)
            {
                if (child is RadioButton rb && rb.IsChecked == true)
                {
                    stars = int.Parse(rb.Tag.ToString() ?? "5");
                    break;
                }
            }

            // Đọc checkbox và gán vào ViewModel
            _viewModel.StarRating = stars;
            _viewModel.ProductRating = GetCheckboxString(cbProdTasty, cbProdBeautiful, cbProdTemperature, cbProdPrice);
            _viewModel.ServiceRating = GetCheckboxString(cbServFast, cbServFriendly, cbServClean, cbServWifi);
            _viewModel.OtherComment = txtComment.Text.Trim();

            // Gọi ViewModel xử lý
            if (_viewModel.SubmitCommand.CanExecute(null))
            {
                _viewModel.SubmitCommand.Execute(null);
                ResetForm();
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
