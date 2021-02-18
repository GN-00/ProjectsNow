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
                double percentage = Math.Round((_Paid / InvoiceTotal) * 100);
                if (percentage >= 100) Status = "Paid in Full";
                else Status = $"Paid {percentage:N2}%";
            }
        }

        private double _Balance;
        public double Balance
        {
            get 
            {
                return _Balance;
            }
            set
            {
                if (value < 0) _Balance = 0;
                if (value > InvoiceTotal) _Balance = InvoiceTotal;
            }
        }
        public string Status { get; set; }

    }
}
