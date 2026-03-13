using System.Windows;
using System.Windows.Controls;

namespace ExpressCleaner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadView("Dashboard");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string viewName)
            {
                LoadView(viewName);
            }
        }

        private void LoadView(string viewName)
        {
            UserControl? view = viewName switch
            {
                "Dashboard" => new Views.DashboardView(),
                "Analysis" => new Views.AnalysisView(),
                "Cleaning" => new Views.CleaningView(),
                "Browsers" => new Views.BrowsersView(),
                "System" => new Views.SystemView(),
                "Tools" => new Views.ToolsView(),
                "Settings" => new Views.SettingsView(),
                _ => null
            };

            if (view != null)
            {
                ContentArea.Content = view;
                StatusText.Text = $"Открыт раздел: {viewName}";
            }
        }
    }
}
