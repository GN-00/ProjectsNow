using System.Windows.Controls;
using ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class ProfitTable : UserControl
    {
        public ProfitTable(Profit profit)
        {
            InitializeComponent();
            DataContext = profit;
        }
    }
}
