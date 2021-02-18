using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows
{

    public class Profit //JobAnalysis(ProjectsInformation)
    {
        public double CostAmount { get; set; }
        public double CostVAT { get; set; }
        public double CostInvoiceTotal { get; set; }

        public double NetMarginAmount { get; set; }
        public double NetMarginVAT { get; set; }
        public double NetMarginInvoiceTotal { get; set; }

        public double NetMarginAmountPercentage { get; set; }
        public double NetMarginVATPercentage { get; set; }
        public double NetMarginInvoiceTotalPercentage { get; set; }

    }
}
