using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class PurchaseOrdersItemsWindow : Window
    {
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }
        public SupplierInvoice InvoiceData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

        CollectionViewSource viewDataPO;
        CollectionViewSource viewDataItems;
        ObservableCollection<CompanyPO> orders;
        ObservableCollection<CompanyPOTransaction> items;
        public PurchaseOrdersItemsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query;
                if (JobOrderData == null)
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);

                query = $"Select * From [Purchase].[OrdersView] Where JobOrderID = {JobOrderData.ID}";
                orders = new ObservableCollection<CompanyPO>(connection.Query<CompanyPO>(query));

                query = $"Select * From [Purchase].[TransactionsView] Where JobOrderID = {JobOrderData.ID}";
                items = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));
            }

            viewDataPO = new CollectionViewSource() { Source = orders };
            viewDataItems = new CollectionViewSource() { Source = items };

            viewDataItems.Filter += DataFilter;

            POList.ItemsSource = viewDataPO.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataPO.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PO_CollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (orders.Count == 0)
                PO_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (items.Count == 0)
                Items_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void PO_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            var selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            else
            {
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
                if (POList.SelectedItem is CompanyPO companyPO)
                    SupplierName.Text = companyPO.SupplierName;
            }
        }
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataItems.View.SortDescriptions.Add(new SortDescription("PartNumber", ListSortDirection.Ascending));
            }
        }
        private void POList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            var selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            else
            {
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
                if (POList.SelectedItem is CompanyPO companyPO)
                    SupplierName.Text = companyPO.SupplierName;
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationItems.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationItems.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
        }

        #region Filters
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is CompanyPOTransaction record)
                {
                    if (POList.SelectedItem is CompanyPO order)
                        if (record.PurchaseOrderID != order.ID)
                        {
                            e.Accepted = false;
                            return;
                        }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }

        #endregion

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Post_Click(object sender, RoutedEventArgs e)
        {
            if(POList.SelectedItem is CompanyPO companyPO)
            {
                ObservableCollection<CompanyPOTransaction> POitems;
                POitems = new ObservableCollection<CompanyPOTransaction>(items.Where(i => i.PurchaseOrderID == companyPO.ID));
                
                if (POitems.Count == 0)
                    return;

                bool canPost = true;
                foreach (CompanyPOTransaction transaction in POitems)
                {
                    if (transaction.Reference != null) canPost = false;
                }

                if (canPost)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        foreach (CompanyPOTransaction transaction in POitems)
                        {
                            ItemTransaction newItem = new ItemTransaction()
                            {
                                JobOrderID = InvoiceData.JobOrderID,
                                InvoiceID = InvoiceData.ID,
                                PanelID = null,
                                PanelTransactionID = null,
                                Category = transaction.Category,
                                Code = transaction.Code,
                                Description = transaction.Description,
                                Source = "New",
                                Type = "Stock",
                                Unit = transaction.Unit,
                                Qty = transaction.Qty,
                                Cost = transaction.Cost,
                                Date = InvoiceData.Date,
                                VAT = companyPO.VAT,
                                OriginalInvoiceID = InvoiceData.ID,
                            };

                            string query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                            newItem.ID = (int)(decimal)connection.ExecuteScalar(query, newItem);
                            ItemsData.Add(newItem);

                            query = $"Update [Purchase].[Transactions] Set Reference = {newItem.ID} Where ID = {transaction.ID}";
                            connection.Execute(query);
                        }
                    }

                    this.Close();
                }

                if (!canPost)
                {
                    CMessageBox.Show("Error", "You can't post this PO!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }
            }
        }
    }
}
