using System;
using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.StoreWindows.TransferWindows
{
    public partial class QtyWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public ItemTransaction ItemData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            DataContext = JobOrderData.Code;
            ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            var receiverJobOrderData = JobOrderData;
            string query;
            SupplierInvoice senderInvoice;
            SupplierInvoice receiverInvoice;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Store].[SuppliersInvoices] Where ID = {ItemData.InvoiceID}";
                senderInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);
                receiverInvoice = InvoiceData;

                double qty = int.Parse(ItemsToPostInput.Text);
                if (qty > 0)
                {
                    ItemData.TransferredQty += qty;
                    ItemTransaction transferData = new ItemTransaction()
                    {
                        JobOrderID = JobOrderData.ID,
                        InvoiceID = senderInvoice.ID,
                        PanelID = null,
                        PanelTransactionID = null,
                        Category = ItemData.Category,
                        Code = ItemData.Code,
                        Description = ItemData.Description,
                        Reference = ItemData.ID,
                        Source = ItemData.Source,
                        Type = "Transfer",
                        Unit = ItemData.Unit,
                        Qty = qty,
                        Cost = ItemData.Cost,
                        Date = DateTime.Today,
                    };
                    query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                    connection.Execute(query, transferData);

                    ItemTransaction newItem = new ItemTransaction()
                    {
                        JobOrderID = receiverJobOrderData.ID,
                        InvoiceID = receiverInvoice.ID,
                        PanelID = null,
                        PanelTransactionID = null,
                        Category = ItemData.Category,
                        Code = ItemData.Code,
                        Description = ItemData.Description,
                        Source = JobOrderData.Code,
                        Type = "Stock",
                        Unit = ItemData.Unit,
                        Qty = qty,
                        Cost = ItemData.Cost,
                        Date = DateTime.Today,
                        TransferInvoiceID = senderInvoice.ID,
                    };
                    query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                    connection.Execute(query, newItem);

                    ItemsData.Add(newItem);
                    this.Close();
                }
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
                ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            }
            else
            {
                int qty = int.Parse(ItemsToPostInput.Text);
                if (qty > ItemData.FinalQty)
                    ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            }
        }
        private void ItemsToPostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemData.Unit == "No" || ItemData.Unit == "Set")
                DataInput.Input.IntOnly(e, 4);
            else
                DataInput.Input.DoubleOnly(e);
        }
    }
}
