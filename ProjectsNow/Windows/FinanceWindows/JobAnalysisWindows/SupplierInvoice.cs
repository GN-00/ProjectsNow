using System;

namespace ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows
{
    public class SupplierInvoice
    {
        public int ID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string SupplierName { get; set; }
        public double Amount { get; set; }
        public double VAT { get; set; }
        public double InvoiceTotal { get; set; }
        public double Paid { get; set; }
        public double Balance { get; set; }
        public string Status { get; set; }

    }
}
