using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class InvoicesWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }

        int SupplierID;
        SupplierInvoice invoiceData;
        List<Reference> referencesData;
        ObservableCollection<SupplierInvoice> invoices;
        ObservableCollection<ItemTransaction> itemsData;
        public InvoicesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [Store].[SuppliersInvoices] Where JobOrderID  = {JobOrderData.ID} ";
                invoices = new ObservableCollection<SupplierInvoice>(connection.Query<SupplierInvoice>(query));

                query = $"Select * From [Store].[JobOrdersInvoicesItems] Where JobOrderID  = {JobOrderData.ID}";
                itemsData = new ObservableCollection<ItemTransaction>(connection.Query<ItemTransaction>(query));

                referencesData = ReferenceController.GetReferences(connection);
            }

            viewDataInvoices = new CollectionViewSource() { Source = invoices };
            viewDataItems = new CollectionViewSource() { Source = itemsData };

            viewDataItems.Filter += DataFilter;
            InvoicesList.ItemsSource = viewDataInvoices.View;
            ItemsList.ItemsSource = viewDataItems.View;

            viewDataInvoices.View.CollectionChanged += new NotifyCollectionChangedEventHandler(InvoicesCollectionChanged);
            viewDataItems.View.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsCollectionChanged);

            if (viewDataInvoices.View.Cast<object>().Count() == 0)
                InvoicesCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewDataItems.View.Cast<object>().Count() == 0)
                ItemsCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                DataContext = new { JobOrderData, UserData, SupplierID };

            if (JobOrderData.ID == 0)
                InvoicesList.RowStyle = (Style)this.Resources["RowStyle2"];
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InvoicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationInvoices.Text = $"Invoices: {viewDataInvoices.View.Cast<object>().Count()}";
            else
                NavigationInvoices.Text = $"Invoice: {selectedIndex + 1} / {viewDataInvoices.View.Cast<object>().Count()}";
        }
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataItems.View.SortDescriptions.Add(new SortDescription("PartNumber", ListSortDirection.Ascending));
            }
        }
        private void InvoicesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataItems.View.Refresh();
            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationInvoices.Text = $"Invoices: {viewDataInvoices.View.Cast<object>().Count()}";
            else
            {
                NavigationInvoices.Text = $"Invoice: {selectedIndex + 1} / {viewDataInvoices.View.Cast<object>().Count()}";
                invoiceData = InvoicesList.SelectedItem as SupplierInvoice;
                SupplierID = invoiceData.SupplierID;
                DataContext = new { JobOrderData, UserData, SupplierID };
            }
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Items: {viewDataItems.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Item: {selectedIndex + 1} / {viewDataItems.View.Cast<object>().Count()}";
        }

        #region Filters
        CollectionViewSource viewDataInvoices;
        CollectionViewSource viewDataItems;
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ItemTransaction item)
                {
                    if (InvoicesList.SelectedItem is SupplierInvoice invoice)
                        if (invoice.ID != item.InvoiceID)
                        {
                            e.Accepted = false;
                            return;
                        }

                    if (item.FinalQty < 1)
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

        private void New_Click(object sender, RoutedEventArgs e)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice()
            {
                JobOrderID = JobOrderData.ID,
                Date = DateTime.Now,
            };
            InvoiceWindow invoicesWindow = new InvoiceWindow()
            {
                ActionData = Actions.New,
                SupplierInvoiceData = supplierInvoice,
                SupplierInvoicesData = invoices,
            };
            invoicesWindow.ShowDialog();
        }
        private void NewInternalInvoice_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select Number From [Store].[TransferInvoiceNumber] Where Year  = {DateTime.Now.Year} And Month = {DateTime.Now.Month} ";
                int invoiceNumber = connection.QueryFirstOrDefault<int>(query) + 1;
                SupplierInvoice supplierInvoice = new SupplierInvoice()
                {
                    JobOrderID = JobOrderData.ID,
                    Date = DateTime.Now,
                    SupplierID = 0,
                    SupplierCode = null,
                    Number = $"ER-{DateTime.Now.Year}{DateTime.Now.Month:00}{invoiceNumber:00}"
                };

                query = $"{DatabaseAI.InsertRecord<SupplierInvoice>()}";
                supplierInvoice.ID = (int)(decimal)connection.ExecuteScalar(query, supplierInvoice);

                invoices.Add(supplierInvoice);
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if(InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                InvoiceWindow invoicesWindow = new InvoiceWindow()
                {
                    ActionData = Actions.Edit,
                    SupplierInvoiceData = invoice
                };
                invoicesWindow.ShowDialog();
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                IEnumerable<ItemTransaction> checkItemsUsage;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"Select * From [Store].[Transactions] Where InvoiceID = {invoice.ID} And Type = 'Used'";
                    checkItemsUsage = connection.Query<ItemTransaction>(query);

                    if (checkItemsUsage.Count() == 0 || checkItemsUsage == null)
                    {
                        query = $"Delete From [Store].[Invoices] Where ID = {invoice.ID}; ";

                        foreach (ItemTransaction item in itemsData.Where(i => i.InvoiceID == invoice.ID).ToList())
                        {
                            query += $"Delete From [Store].[Transactions] Where ID = {item.ID}; ";

                            if (item.TransferInvoiceID != null)
                                query += $"Delete From [Store].[Transactions] Where ID = {item.TransferInvoiceID}; ";

                            itemsData.Remove(item);
                        }
                        connection.Execute(query);
                        invoices.Remove(invoice);
                    }
                    else
                    {
                        CMessageBox.Show("Deleting", $"You can't delete {invoice.Number} data. \nIts used in projects data", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    }
                }
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is SupplierInvoice invoiceData)
            {
                if (invoiceData.SupplierID != 0)
                {
                    CMessageBox.Show("Invoice", "Only internal invoice can be print it!! ", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }

                InvoiceInformation invoiceInformation;
                List<Printing.Store.Item> items;
                List<IPanel> panels;
                List<string> POs;
                string IDs = "";
                Printing.Store.InternalInvoice invoiceForm;

                foreach (ItemTransaction item in viewDataItems.View)
                    IDs += $"{item.ID}, ";

                IDs = IDs.Substring(0, IDs.Length - 2);


                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query;
                    query = $"Select * From [Store].[InvoicesInformations] Where ID  = {invoiceData.ID}";
                    invoiceInformation = connection.QueryFirstOrDefault<InvoiceInformation>(query);

                    query = $"Select PurchaseOrdersNumber From [JobOrder].[Panels] Where JobOrderID = {invoiceData.JobOrderID}";
                    panels = connection.Query<IPanel>(query).ToList();

                    query = $"Select * From [Store].[InvoicesItemsInformations] Where InvoiceID = {invoiceData.ID} And " +
                            $"ID in ({IDs})";
                    items = connection.Query<Printing.Store.Item>(query).ToList();
                }
                POs = panels.GroupBy(p => p.PurchaseOrdersNumber).Select(p => p.Key).ToList();


                for (int i = 1; i <= items.Count; i++)
                    items[i - 1].SN = i;

                foreach (string po in POs)
                    invoiceInformation.POs += $"{po}, ";

                invoiceInformation.POs = invoiceInformation.POs.Substring(0, invoiceInformation.POs.Length - 2);

                double pagesNumber = (items.Count) / 8d;
                if (pagesNumber - Convert.ToInt32(pagesNumber) != 0)
                    pagesNumber = Convert.ToInt32(pagesNumber) + 1;

                if (pagesNumber != 0)
                {
                    List<FrameworkElement> elements = new List<FrameworkElement>();
                    for (int i = 1; i <= pagesNumber; i++)
                    {
                        if (i == pagesNumber)
                        {
                            invoiceForm = new Printing.Store.InternalInvoice()
                            {
                                VATPercentage = items.Max(item => item.VAT),
                                TotalCost = items.Sum(item => item.TotalCost),
                                TotalVAT = items.Sum(item => item.VAT/100 * item.TotalCost),
                                TotalPrice = items.Sum(item => (1 + item.VAT / 100) * item.TotalCost),
                                Page = i,
                                Pages = Convert.ToInt32(pagesNumber),
                                InvoiceInformationData = invoiceInformation,
                                ItemsData = items.Where(item => item.SN > ((i - 1) * 8) && item.SN <= ((i) * 8)).ToList()
                            };
                        }
                        else
                        {
                            invoiceForm = new Printing.Store.InternalInvoice()
                            {
                                Page = i,
                                Pages = Convert.ToInt32(pagesNumber),
                                InvoiceInformationData = invoiceInformation,
                                ItemsData = items.Where(item => item.SN > ((i - 1) * 8) && item.SN <= ((i) * 8)).ToList()
                            };
                        }

                        elements.Add(invoiceForm);
                    }

                    Printing.Print.PrintPreview(elements, $"Invoice-{invoiceData.Number}");
                }
                else
                {
                    CMessageBox.Show("Items", "There is no items!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void AddItems_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                ItemTransaction itemTransaction = new ItemTransaction()
                {
                    JobOrderID = JobOrderData.ID,
                    InvoiceID = invoice.ID,
                    Date = invoice.Date,
                    OriginalInvoiceID = invoice.ID,
                };

                var window = new ItemsWindow()
                {
                    ActionData = Actions.New,
                    ItemData = itemTransaction,
                    ReferencesData = referencesData,
                    ItemsData = itemsData,
                };
                window.ShowDialog();
            }
        }
        private void EditItems_Click(object sender, RoutedEventArgs e)
        {
            //if (ItemsList.SelectedItem is ItemTransaction item)
            //{
            //    ItemTransaction checkItemUsage;
            //    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            //    {
            //        string query = $"Select Reference From [Store].[Transactions] Where Reference = {item.ID}";
            //        checkItemUsage = connection.QueryFirstOrDefault<ItemTransaction>(query);
            //    }

            //    if(checkItemUsage == null)
            //    {
            //        var window = new ItemsWindow()
            //        {
            //            ActionData = Actions.Edit,
            //            ItemData = item,
            //            ReferencesData = referencesData,
            //            ItemsData = itemsData,
            //        };
            //        window.ShowDialog();
            //    }
            //    else
            //    {
            //        CMessageBox.Show("Items Usage", "Can't edit this item!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            //    }
            //}
        }
        private void DeleteItems_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is ItemTransaction item)
            {
                ItemTransaction checkItemUsage;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query = $"Select Reference From [Store].[Transactions] Where Reference = {item.ID}";
                    checkItemUsage = connection.QueryFirstOrDefault<ItemTransaction>(query);

                    if (checkItemUsage == null)
                    {
                        query = $"Delete From [Store].[Transactions] Where ID = {item.ID}; ";

                        if (item.TransferInvoiceID != null)
                            query += $"Delete From [Store].[Transactions] Where ID = {item.TransferInvoiceID}; ";

                        connection.Execute(query);

                        itemsData.Remove(item);
                    }
                }

                if (checkItemUsage != null)
                {
                    CMessageBox.Show("Items Usage", "Can't delete this item!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Suppliers_Click(object sender, RoutedEventArgs e)
        {
            var suppliersWindow = new SuppliersWindows.SuppliersWindow()
            {
                UserData = this.UserData,
            };
            suppliersWindow.ShowDialog();
        }

        private void JobOrderItems_Click(object sender, RoutedEventArgs e)
        {
            if(InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                JobOrderItemsWindow jobOrderItemsWindow = new JobOrderItemsWindow()
                {
                    JobOrderData = JobOrderData,
                    InvoiceData = invoice,
                    ItemsData = itemsData,
                };
                jobOrderItemsWindow.ShowDialog();
            }
        }

        private void PurchaseItems_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                PurchaseOrdersItemsWindow purchaseOrdersItemsWindow = new PurchaseOrdersItemsWindow()
                {
                    JobOrderData = JobOrderData,
                    InvoiceData = invoice,
                    ItemsData = itemsData,
                };
                purchaseOrdersItemsWindow.ShowDialog();
            }
        }

        private void StockItems_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is SupplierInvoice invoice)
            {
                TransferWindows.TransferWindow transferWindow = new TransferWindows.TransferWindow()
                {
                    UserData = UserData,
                    JobOrderData = JobOrderData,
                    InvoiceData = invoice,
                    ItemsData = itemsData,
                };
                transferWindow.ShowDialog();
            }
        }
    }
}
