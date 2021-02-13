using System.Linq;
using System.Windows;
using ProjectsNow.Database;
using System.Windows.Controls;
using System.Collections.Generic;

namespace ProjectsNow.Printing.Purchase
{
    public partial class PurchaseOrderForm : UserControl
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public CompanyPO CompanyPOData { get; set; }
        public List<CompanyPOTransaction> Transactions { get; set; }


        double subtotal;
        double VAT;
        double grandTotal;
        string grandTotalText;
        public PurchaseOrderForm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;

            if (Page != Pages)
                TotalTable.Visibility = Visibility.Collapsed;
            else
                grandTotalText = DataInput.Input.NumberToWords(System.Convert.ToInt32(grandTotal));

            for (int i = 0; i < Transactions.Count; i++)
            {
                ItemsTable.RowDefinitions[i+1].Height = new GridLength(0.6 * App.cm);
            }

            subtotal = Transactions.Sum(t => t.Qty * t.Cost);
            VAT = subtotal * (CompanyPOData.VAT / 100);
            grandTotal = subtotal * (1 + (CompanyPOData.VAT / 100));

            DataContext = new { Page, Pages, Transactions, CompanyPOData, subtotal, VAT, grandTotal, grandTotalText };
        }
    }
}
