using System;
using ProjectsNow.Database;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjectsNow.Printing
{
    public partial class QuotationPage : UserControl
    {
        public bool? BackgroundData { get; set; } 
        public string QuotationCode { get; set; }
        public Quotation QuotationData{ get; set; }

        public QuotationPage()
        {
            InitializeComponent();
        }

        public QuotationPage(Quotation quotation)
        {
            InitializeComponent();
            QuotationCode = quotation.QuotationCode;
            QuotationData= quotation;
            if (BackgroundData == null) BackgroundData = false;
            if (BackgroundData.Value) BackgroundImage.Visibility = System.Windows.Visibility.Visible;
            DataContext = new { QuotationCode };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BackgroundData == null) BackgroundData = false;
            if (BackgroundData.Value) BackgroundImage.Visibility = System.Windows.Visibility.Visible;

            if (QuotationData.EstimationName == "Eng. Abdul Rahim Alastal")
                BackgroundImage.Source = new BitmapImage(new Uri(@"/Images/Arahim.png", UriKind.Relative));
            else if (QuotationData.EstimationName == "Eng. Qasim Alshehri")
                BackgroundImage.Source = new BitmapImage(new Uri(@"/Images/Qasim.png", UriKind.Relative));
            else if (QuotationData.EstimationName == "Eng. Waheeb Akram")
                BackgroundImage.Source = new BitmapImage(new Uri(@"/Images/Waheeb.png", UriKind.Relative));
            else
                BackgroundImage.Source = new BitmapImage(new Uri(@"/Images/WATERMARK.png", UriKind.Relative));
        }
    }
}
