using System.Windows.Controls;
using ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class TransportationTable : UserControl
    {
        public TransportationTable(Transportation transportation)
        {
            InitializeComponent();
            DataContext = transportation;
        }
    }
}
