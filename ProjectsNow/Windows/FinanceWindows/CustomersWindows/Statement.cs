using System;

namespace ProjectsNow.Windows.FinanceWindows.CustomersWindows
{
    public class Statement
    {
        public int SN { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }
        public double Balance { get; set; }
        public string Sign
        {
            get
            {
                if ((double)Balance > 0) return "Cr";
                else if ((double)Balance < 0) return "Dr";
                else return null;
            }
        }

        public string BalanceView
        {
            get { return $"{Math.Abs(Balance)} {Sign}"; }
        }
    }
}
