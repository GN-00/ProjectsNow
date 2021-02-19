using System.Windows.Controls;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class JobAnalysisForm : UserControl
    {
        public JobAnalysisForm(string code, string customerName)
        {
            InitializeComponent();
            DataContext = new { code, customerName };
        }
    }
}
