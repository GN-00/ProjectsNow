using System;
using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;

namespace ProjectsNow.Windows.StoreWindows.ReturnItemsWindows
{
    public partial class QtyWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public ItemTransaction ItemData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemsToPostInput.Text = ItemData.FinalQty.ToString();
            ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            var reciverJobOrderData = DatabaseAI.store;
            string query;
            SupplierInvoice senderInvoice;
            SupplierInvoice receiverInvoice;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Store].[SuppliersInvoices] Where ID = {ItemData.InvoiceID}";
                senderInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);

                query = $"Select * From [Store].[SuppliersInvoices] Where JobOrderID = {reciverJobOrderData.ID} And Number = '{senderInvoice.Number}'";
                receiverInvoice = connection.QueryFirstOrDefault<SupplierInvoice>(query);

                if (receiverInvoice == null)
                {
                    receiverInvoice = new SupplierInvoice();
                    receiverInvoice.Update(senderInvoice);
                    receiverInvoice.JobOrderID = reciverJobOrderData.ID;
                    query = $"{DatabaseAI.InsertRecord<SupplierInvoice>()}";
                    receiverInvoice.ID = (int)(decimal)connection.ExecuteScalar(query, receiverInvoice);
                }

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
                        VAT = ItemData.VAT,
                    };
                    query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                    connection.Execute(query, transferData);

                    ItemTransaction newItem = new ItemTransaction()
                    {
                        JobOrderID = reciverJobOrderData.ID,
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
                        VAT = ItemData.VAT,
                        TransferInvoiceID = senderInvoice.ID
                    };
                    query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                    connection.Execute(query, newItem);

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
