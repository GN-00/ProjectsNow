using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class QtyWindow : Window
    {
        public ItemPurchased ItemData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }
        public QtyWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            VATInput.Text = App.VAT.ToString();
            DataContext = ItemData;
            ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            double qty = int.Parse(ItemsToPostInput.Text);
            double cost = double.Parse(CostInput.Text);
            if (qty > 0)
            {
                ItemData.PurchasedQty += qty;

                ItemTransaction newItem = new ItemTransaction()
                {
                    JobOrderID = InvoiceData.JobOrderID,
                    InvoiceID = InvoiceData.ID,
                    PanelID = null,
                    PanelTransactionID = null,
                    Category = ItemData.Category,
                    Code = ItemData.Code,
                    Description = ItemData.Description,
                    Source = "New",
                    Type = "Stock",
                    Unit = ItemData.Unit,
                    Qty = qty,
                    Cost = cost,
                    VAT = double.Parse(VATInput.Text),
                    Date = InvoiceData.Date,
                    OriginalInvoiceID = InvoiceData.ID,
                };

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                    newItem.ID = (int)(decimal)connection.ExecuteScalar(query, newItem);
                }
                ItemsData.Add(newItem);

                this.Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ItemsToPostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemsToPostInput.Text))
            {
                ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            }
            else
            {
                int qty = int.Parse(ItemsToPostInput.Text);
                if (qty > ItemData.RemainingQty)
                    ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            }
        }
        private void ItemsToPostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemData.Unit == "No" || ItemData.Unit == "Set")
                DataInput.Input.IntOnly(e, 4);
            else
                DataInput.Input.DoubleOnly(e);
        }
        private void CostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void CostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CostInput.Text))
            {
                CostInput.Text = "0";
            }
        }

        private void VATInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void VATInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VATInput.Text) || VATInput.Text == "0")
            {
                VATInput.Text = App.VAT.ToString();
            }
        }
    }
}
