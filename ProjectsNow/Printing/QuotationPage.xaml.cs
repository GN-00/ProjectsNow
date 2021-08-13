using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class QuotationPage : UserControl
    {
        public bool? BackgroundData { get; set; } 
        public string QuotationCode { get; set; }

        public QuotationPage()
        {
            InitializeComponent();
        }

        public QuotationPage(string quotationCode)
        {
            InitializeComponent();
            QuotationCode = quotationCode;
            if (BackgroundData == null) BackgroundData = false;
            if (BackgroundData.Value) Background.Visibility = System.Windows.Visibility.Visible;
            DataContext = new { QuotationCode };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BackgroundData == null) BackgroundData = false;
            if (BackgroundData.Value) Background.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
