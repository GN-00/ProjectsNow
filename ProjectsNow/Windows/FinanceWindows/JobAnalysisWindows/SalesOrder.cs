using System;

namespace ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows
{
    public class SalesOrder //JobAnalysis(ProjectsInformation)
    {
        public DateTime IssuedDate { get; set; }
        public string POs { get; set; }
        public string CustomerName { get; set; }
        public double QuotationAmount { get; set; }
        public double QuotationVAT { get; set; }
        public double QuotationInvoiceTotal { get; set; }

    }
}
