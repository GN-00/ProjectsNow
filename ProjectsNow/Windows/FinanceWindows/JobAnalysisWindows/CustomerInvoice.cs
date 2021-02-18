using System;

namespace ProjectsNow.Windows.FinanceWindows.JobAnalysisWindows
{
    public class CustomerInvoice
    {
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }
        public double VAT { get; set; }
        public double InvoiceTotal { get; set; }

        private double _Paid;
        public double Paid
        {
            get
            {
                return _Paid;
            }
            set
            {
                _Paid = value;
                Balance = Math.Round(InvoiceTotal - _Paid);
                if (Balance < 0) Balance = 0;
                double percentage = Math.Round((_Paid / InvoiceTotal) * 100);
                if (percentage >= 100) Status = "Paid in Full";
                else Status = $"Paid {percentage:N2}%";
            }
        }
        public double Balance { get; set; }

        public string Status { get; set; }
    }
}
