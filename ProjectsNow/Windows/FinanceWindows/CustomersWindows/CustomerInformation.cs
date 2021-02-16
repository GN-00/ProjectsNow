using System;

namespace ProjectsNow.Windows.FinanceWindows.CustomersWindows
{
    public class CustomerInformation
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public string VATNumber { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Balance 
        { 
            get
            {
                return (Credit - Debit);
            }
        }
        public string Sign
        { 
            get
            {
                if (Math.Round(Balance) > 0) return "Cr";
                else if (Math.Round(Balance) < 0) return "Dr";
                else return null;
            }
        }

        public string BalanceView
        {
            get { return $"{Math.Abs(Balance):N2} {Sign}"; }
        }
    }
}
