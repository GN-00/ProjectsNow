using ProjectsNow.Database;
using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class OrderAcknowledgement : UserControl
    {
        public bool BackgroundData { get; set; }
        public AcknowledgmentInformation AcknowledgementInformationData { get; set; }
        public OrderAcknowledgement()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;

            if (AcknowledgementInformationData.CancelationToggle)
                Cancelation.Visibility = System.Windows.Visibility.Visible;

            if (BackgroundData) BackgroundImage.Visibility = System.Windows.Visibility.Visible;
            DataContext = AcknowledgementInformationData;
        }
    }
}
