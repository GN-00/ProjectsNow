using System.Windows;
using ProjectsNow.Database;
using System.Windows.Input;
using ProjectsNow.Controllers;

namespace ProjectsNow.Windows.QuotationWindows.QuotationsInformationWindows
{
    public partial class QuotationPanelWindow : Window
    {
        public QPanel PanelData { get; set; }

        QPanel newQPanelData;
        public QuotationPanelWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newQPanelData = new QPanel();
            newQPanelData.Update(PanelData);

            DataContext = new { newQPanelData };
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
