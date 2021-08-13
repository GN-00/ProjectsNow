using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Events;
using System.Windows.Data;
using ProjectsNow.Printing;
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

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Invoicing_Windows
{
    public partial class InvoicingWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }

        ObservableCollection<Invoice> invoices;
        ObservableCollection<JPanel> tempPanelsData;
        ObservableCollection<TPanel> panelsTransaction;
        public InvoicingWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Panels(Invoiced)] Where JobOrderID  = {JobOrderData.ID} ";
                panelsTransaction = new ObservableCollection<TPanel>(connection.Query<TPanel>(query));

                if (PanelsData == null)
                    PanelsData = JPanelController.GetJobOrderPanels(connection, JobOrderData.ID);
            }

            invoices = new ObservableCollection<Invoice>(panelsTransaction.GroupBy(i => i.Reference, ii => ii.Date).Select(m => new Invoice { Number = m.Key, Date = m.ToList()[0] }).OrderByDescending(m => m.Number));

            viewDataInvoices = new CollectionViewSource() { Source = invoices };
            viewDataPanels = new CollectionViewSource() { Source = panelsTransaction };

            viewDataPanels.Filter += DataFilter;
            InvoicesList.ItemsSource = viewDataInvoices.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataInvoices.View.CollectionChanged += new NotifyCollectionChangedEventHandler(InvoicesCollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PanelsCollectionChanged);

            if (viewDataInvoices.View.Cast<object>().Count() == 0)
                InvoicesCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewDataPanels.View.Cast<object>().Count() == 0)
                PanelsCollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            tempPanelsData = new ObservableCollection<JPanel>();
            foreach (JPanel panel in PanelsData)
            {
                JPanel newPanel = new JPanel();
                newPanel.Update(panel);
                tempPanelsData.Add(newPanel);
            }

            DataContext = new { JobOrderData, UserData };
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
        private void PanelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewDataPanels.View.SortDescriptions.Add(new SortDescription("PanelSN", ListSortDirection.Ascending));
            }
        }
        private void InvoicesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = InvoicesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationInvoices.Text = $"Invoices: {viewDataInvoices.View.Cast<object>().Count()}";
            else
                NavigationInvoices.Text = $"Invoice: {selectedIndex + 1} / {viewDataInvoices.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
        }

        #region Filters
        CollectionViewSource viewDataInvoices;
        CollectionViewSource viewDataPanels;
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is TPanel panel)
                {
                    if (InvoicesList.SelectedItem is Invoice invoice)
                        if (invoice.Number != panel.Reference)
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
            ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Visible;
            ControlLock2.Visibility = Visibility.Collapsed;

            int invoiceNumber;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select InvoiceNumber From [JobOrder].[InvoiceNumber] Where Year = {DateTime.Now.Year}";
                invoiceNumber = connection.QueryFirstOrDefault<int>(query) + 1;
            }

            var newInvoice = new Invoice() { Number = $"{DateTime.Now.Year}{DateTime.Now.Month:00}{invoiceNumber:000}", Date = DateTime.Now };
            invoices.Insert(0, newInvoice);
            InvoicesList.SelectedItem = newInvoice;

            LoadingControl.Visibility = Visibility.Visible;

            PanelsList.ContextMenu = (ContextMenu)this.Resources["PanelsListContextMenu"];
            PanelsList.RowStyle = (Style)this.Resources["NewPanels"];
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Invoice invoice)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();
                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    foreach (TPanel panel in panelsTransaction.Where(i => i.Reference == invoice.Number))
                    {
                        query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                                $"(PanelID, Reference, Qty, Date, Action, JobOrderID) " +
                                $"Values " +
                                $"(@PanelID, @Reference, @Qty, @Date, 'Invoiced', @JobOrderID) Select @@IDENTITY";
                        panel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, panel);

                        JPanel panelData = PanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                        panelData.InvoicedQty += panel.Qty;
                    }
                }

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Invoice invoice)
            {
                LoadingControl.Visibility = Visibility.Visible;
                DoingEvent.DoEvents();

                ControlLock1.Visibility = PrintingLock.Visibility = Visibility.Collapsed;
                ControlLock2.Visibility = Visibility.Visible;

                List<TPanel> list = new List<TPanel>();
                foreach (TPanel panel in panelsTransaction.Where(i => i.Reference == invoice.Number))
                {
                    JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                    panelData.InvoicedQty += panel.Qty * -1;
                    list.Add(panel);
                }

                foreach (TPanel panel in list)
                {
                    panelsTransaction.Remove(panel);
                }

                invoices.Remove(invoice);

                PanelsList.ContextMenu = null;
                PanelsList.RowStyle = (Style)this.Resources["Panels"];

                LoadingControl.Visibility = Visibility.Collapsed;
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if(InvoicesList.SelectedItem is Invoice invoiceData)
            {
                var result = CMessageBox.Show("Printing", "Print with watermark?", CMessageBoxButton.YesNo, CMessageBoxImage.Question);
                InvoiceInformation invoiceInformation;
                List<IPanel> panels;
                List<string> POs;
                InvoiceForm invoiceForm;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query;
                    query = $"Select * From [JobOrder].[InvoicesInformations] Where InvoiceNumber  = {invoiceData.Number}";
                    invoiceInformation = connection.QueryFirstOrDefault<InvoiceInformation>(query);

                    query = $"Select * From [JobOrder].[Panels(InvoiceDetails)] Where InvoiceNumber = {invoiceData.Number}";
                    panels = connection.Query<IPanel>(query).ToList();
                }
                POs = panels.GroupBy(p => p.PurchaseOrdersNumber).Select(p => p.Key).ToList();


                for (int i = 1; i <= panels.Count; i++)
                    panels[i - 1].PanelSN = i; 

                foreach(string po in POs)
                    invoiceInformation.POs += $"{po}, ";

                invoiceInformation.POs = invoiceInformation.POs.Substring(0, invoiceInformation.POs.Length - 2);

                double pagesNumber = (panels.Count) / 8d;
                if (pagesNumber - Convert.ToInt32(pagesNumber) != 0)
                    pagesNumber = Convert.ToInt32(pagesNumber) + 1;

                if (pagesNumber != 0)
                {
                    List<FrameworkElement> elements = new List<FrameworkElement>();
                    for (int i = 1; i <= pagesNumber; i++)
                    {
                        if (i == pagesNumber)
                        {
                            invoiceForm = new InvoiceForm()
                            {
                                VATPercentage = panels.Max(p => p.VAT) * 100,
                                TotalCost = panels.Sum(p => p.PanelsEstimatedPrice),
                                TotalVAT = panels.Sum(p => p.VATValue),
                                TotalPrice = panels.Sum(p => p.FinalPrice),
                                Page = i,
                                Pages = Convert.ToInt32(pagesNumber),
                                InvoiceInformationData = invoiceInformation,
                                PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 8) && p.PanelSN <= ((i) * 8)).ToList()
                            };
                            if (result == MessageBoxResult.Yes) invoiceForm.Background.Visibility = Visibility.Visible;

                        }
                        else
                        {
                            invoiceForm = new InvoiceForm()
                            {
                                Page = i,
                                Pages = Convert.ToInt32(pagesNumber),
                                InvoiceInformationData = invoiceInformation,
                                PanelsData = panels.Where(p => p.PanelSN > ((i - 1) * 8) && p.PanelSN <= ((i) * 8)).ToList()
                            };
                            if (result == MessageBoxResult.Yes) invoiceForm.Background.Visibility = Visibility.Visible;
                        }
                        elements.Add(invoiceForm);
                    }
                    Print.PrintPreview(elements, $"Invoice-{invoiceData.Number}");
                }
                else
                {
                    CMessageBox.Show("Items", "There is no panels!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void AddPanels_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicesList.SelectedItem is Invoice invoice)
            {
                var window = new PanelsWindow()
                {
                    InvoiceData = invoice,
                    PanelsData = tempPanelsData,
                    PanelsTransaction = panelsTransaction,
                };
                window.ShowDialog();
            }
        }
        private void DeletePanels_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is TPanel panel)
            {
                JPanel panelData = tempPanelsData.FirstOrDefault(i => i.PanelID == panel.PanelID);
                panelData.InvoicedQty += panel.Qty * -1;
                panelsTransaction.Remove(panel);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UserData.JobOrderID = null;
            using(SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserController.UpdateJobOrderID(connection, UserData);
            }
        }
    }
}
