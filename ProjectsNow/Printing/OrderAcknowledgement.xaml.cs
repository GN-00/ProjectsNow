using ProjectsNow.Database;
using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class OrderAcknowledgement : UserControl
    {
        public AcknowledgmentInformation AcknowledgementInformationData { get; set; }
        public OrderAcknowledgement()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;

            DataContext = AcknowledgementInformationData;
        }
    }
}
