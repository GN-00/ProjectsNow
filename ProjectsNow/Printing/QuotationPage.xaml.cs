using System.Windows.Controls;

namespace ProjectsNow.Printing
{
    public partial class QuotationPage : UserControl
    {
        public string QuotationCode { get; set; }

        public QuotationPage()
        {
            InitializeComponent();
        }

        public QuotationPage(string quotationCode)
        {
            InitializeComponent();
            QuotationCode = quotationCode;
            DataContext = new { QuotationCode };
        }
    }
}
