using System.Windows.Controls;
using ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class OverheadTable : UserControl
    {
        public OverheadTable(Overhead overhead)
        {
            InitializeComponent();
            DataContext = overhead;
        }
    }
}
