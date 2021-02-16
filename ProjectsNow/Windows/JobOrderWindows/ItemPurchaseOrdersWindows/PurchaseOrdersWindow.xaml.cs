using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows.ItemPurchaseOrdersWindows
{
    public partial class PurchaseOrdersWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        CollectionViewSource viewDataPO;
        CollectionViewSource viewDataItems;
        ObservableCollection<CompanyPO> orders;
        ObservableCollection<CompanyPOTransaction> items;
        public PurchaseOrdersWindow()
        {
            InitializeComponent();
            UserData = new User();
            JobOrderData = new JobOrder() { ID = 20 };
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

            DataContext = new { JobOrderData, UserData };
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

        private void NewPO_Click(object sender, RoutedEventArgs e)
        {
            CompanyPO order = new CompanyPO() { Date = DateTime.Now, JobOrderID = JobOrderData.ID, VAT = App.VAT };
            OrderWindow purchaseOrderWindow = new OrderWindow()
            {
                ActionData = Actions.New,
                OrdersData = this.orders,
                OrderData = order
            };
            purchaseOrderWindow.ShowDialog();
        }
        private void EditPO_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is CompanyPO order)
            {
                bool canEdit = true;
                string query;
                ObservableCollection<CompanyPOTransaction> checkItems;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Purchase].[TransactionsView] Where JobOrderID = {JobOrderData.ID}";
                    checkItems = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));

                    foreach (CompanyPOTransaction transaction in checkItems)
                    {
                        if (transaction.Reference != null) canEdit = false;
                    }

                    if (canEdit)
                    {
                        OrderWindow purchaseOrderWindow = new OrderWindow()
                        {
                            ActionData = Actions.Edit,
                            OrdersData = null,
                            OrderData = order
                        };
                        purchaseOrderWindow.ShowDialog();
                    }

                }

                if (!canEdit)
                {
                    CMessageBox.Show("Error", "You can't edit this PO!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }

            }
        }
        private void DeletePO_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is CompanyPO companyPO)
            {
                bool canDelete = true;
                string query;
                ObservableCollection<CompanyPOTransaction> checkItems;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Purchase].[TransactionsView] Where JobOrderID = {JobOrderData.ID}";
                    checkItems = new ObservableCollection<CompanyPOTransaction>(connection.Query<CompanyPOTransaction>(query));

                    foreach (CompanyPOTransaction transaction in checkItems)
                    {
                        if (transaction.Reference != null) canDelete = false;
                    }

                    if (canDelete)
                    {
                        query += $"Delete From [Purchase].[Orders] Where ID = {companyPO.ID}; ";
                        query += $"Delete From [Purchase].[Transactions] Where PurchaseOrderID = {companyPO.ID}; ";
                        connection.Execute(query);

                        foreach (CompanyPOTransaction transaction in checkItems)
                        {
                            items.Remove(items.FirstOrDefault(item => item.ID == transaction.ID));
                        }

                        orders.Remove(companyPO);
                    }

                }

                if (!canDelete)
                {
                    CMessageBox.Show("Error", "You can't delete this PO!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }
            }
        }
        private void AddItems_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is CompanyPO companyPO)
            {
                ItemsWindow itemsWindow = new ItemsWindow()
                {
                    CompanyPOData = companyPO,
                    ItemsData = items,
                };
                itemsWindow.ShowDialog();
            }
        }

        private void EditItems_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is CompanyPO companyPO)
            {
                if (ItemsList.SelectedItem is CompanyPOTransaction transaction)
                {
                    string query;
                    CompanyPOTransaction checkTransaction;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        query = $"Select * From [Purchase].[TransactionsView] Where ID = {transaction.ID}";
                        checkTransaction = connection.QueryFirstOrDefault<CompanyPOTransaction>(query);

                        if (checkTransaction == null)
                            return;

                        if (checkTransaction.Reference == null)
                        {
                            ItemPurchased item;
                            query = $"Select * From [Store].[JobOrdersItems(PurchaseDetails)] Where JobOrderID = {companyPO.JobOrderID} And Code = '{transaction.Code}'";
                            item = connection.QueryFirstOrDefault<ItemPurchased>(query);

                            QtyWindow qtyWindow = new QtyWindow()
                            {
                                ActionData = Actions.Edit,
                                ItemData = item,
                                CompanyPOData = companyPO,
                                ItemsData = null,
                                CompanyPOTransactionData = transaction,
                            };
                            qtyWindow.ShowDialog();
                        }
                    }

                    if (checkTransaction.Reference != null)
                    {
                        CMessageBox.Show("Error", "You can't delete this Item!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                        return;
                    }
                }
            }
        }

        private void DeleteItems_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is CompanyPOTransaction transaction)
            {
                string query;
                CompanyPOTransaction checkTransaction;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Purchase].[TransactionsView] Where ID = {transaction.ID}";
                    checkTransaction = connection.QueryFirstOrDefault<CompanyPOTransaction>(query);

                    if (checkTransaction == null)
                        return;

                    if (checkTransaction.Reference == null)
                    {
                        query = $"Delete From [Purchase].[Transactions] Where ID = {transaction.ID}; ";
                        connection.Execute(query);
                        items.Remove(transaction);
                    }
                }

                if (checkTransaction.Reference != null)
                {
                    CMessageBox.Show("Error", "You can't delete this Item!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void Suppliers_Click(object sender, RoutedEventArgs e)
        {
            var suppliersWindow = new StoreWindows.SuppliersWindows.SuppliersWindow()
            {
                UserData = this.UserData,
            };
            suppliersWindow.ShowDialog();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if(POList.SelectedItem is CompanyPO companyPO)
            {
                const double rows = 10;
                double pages = items.Count / rows;
                if (pages != Convert.ToInt32(pages)) pages = Convert.ToInt32(pages) + 1;

                companyPO.JobOrderCode = JobOrderData.Code;
                List<CompanyPOTransaction> transactions = items.Where(item => item.PurchaseOrderID == companyPO.ID).ToList();
                foreach(CompanyPOTransaction transaction in transactions)
                {
                    transaction.SN = transactions.IndexOf(transaction) + 1;
                }
                List<FrameworkElement> elements = new List<FrameworkElement>();
                if (pages != 0)
                {
                    for (int i = 1; i <= pages; i++)
                    {
                        Printing.Purchase.PurchaseOrderForm purchaseOrderForm = new Printing.Purchase.PurchaseOrderForm()
                        {
                            Page = i,
                            Pages = Convert.ToInt32(pages),
                            CompanyPOData = companyPO,
                            Transactions = transactions.Where(s => s.SN > ((i - 1) * rows) && s.SN <= ((i) * rows)).ToList(),
                        };
                        elements.Add(purchaseOrderForm);
                    }

                    Printing.Print.PrintPreview(elements, $"Purchase Order {companyPO.Number}");
                }
                else
                {
                    CMessageBox.Show("Statement", "There is no items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.JobOrderID = null;
                UserController.UpdateJobOrderID(connection, UserData);
            }
        }
    }
}
