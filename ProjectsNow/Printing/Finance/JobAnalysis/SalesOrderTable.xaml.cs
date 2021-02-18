using System.Windows.Controls;
using ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows;

namespace ProjectsNow.Printing.Finance.JobAnalysis
{
    public partial class SalesOrderTable : UserControl
    {
        public SalesOrderTable(SalesOrder salesOrder)
        {
            InitializeComponent();
            DataContext = salesOrder;
        }
    }
}
