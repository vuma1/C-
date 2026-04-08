using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.Utils;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyQuanCafe.Views
{
    public partial class MainWindow : Window
    {
        private User? currentUser;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            currentUser = UserSession.GetCurrentUser();
            if (currentUser != null)
            {
                SetupUserInterface();
            }
        }

        private void SetupUserInterface()
        {
            if (currentUser != null)
            {
                lblWelcome.Text = $"Xin chào: {currentUser.Username}";
                
                // N?u là STAFF thì ?n m?t s? ch?c nang
                if (currentUser.Role == "STAFF")
                {
                    btnStats.IsEnabled = false;
                    btnUsers.IsEnabled = false;
                }
            }
        }

        private void LoadView(UserControl view)
        {
            contentArea.Child = view;
        }

        private void OpenOrderScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new OrderView());
        }

        private void OpenTableScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new TableView());
        }

        private void OpenProductScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new ProductView());
        }

        private void OpenStatsScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new StatsView());
        }

        private void OpenUserScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new UserView());
        }

        private void OpenFeedbackScreen(object sender, RoutedEventArgs e)
        {
            LoadView(new FeedbackView());
        }

        private void HandleExit(object sender, RoutedEventArgs e)
        {
            UserSession.Clear();
            LoginView loginView = new LoginView();
            loginView.Show();
            this.Close();
        }
    }
}
