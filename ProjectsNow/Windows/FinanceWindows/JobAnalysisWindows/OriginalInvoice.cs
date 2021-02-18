using System;

namespace ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows
{
    public class OriginalInvoice
    {
        public int InvoiceID { get; set; }
        public int OriginalInvoiceID { get; set; }
        public double Paid { get; set; }
        public double InvoiceTotal { get; set; }
        public double Balance
        {
            get { return Math.Round(InvoiceTotal - Paid); }
        }
    }
}
