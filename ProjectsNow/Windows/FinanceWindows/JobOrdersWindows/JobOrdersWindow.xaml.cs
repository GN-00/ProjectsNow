using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.FinanceWindows.JobOrdersWindows
{
    public partial class JobOrdersWindow : Window
    {
        public User UserData { get; set; }

        List<int> years;
        bool isLoading = true;
        ObservableCollection<JobOrderFinance> jobOrders;
        public JobOrdersWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                query = $"Select * From [Finance].[JobOrdersDetails] Where Year = {DateTime.Now.Year}";
                jobOrders = new ObservableCollection<JobOrderFinance>(connection.Query<JobOrderFinance>(query));
                
                query = $"Select * From [JobOrder].[JobOrdersYears]";
                years = connection.Query<int>(query).ToList();
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = jobOrders };
            viewData.Filter += DataFilter;

            YearsList.ItemsSource = years;
            JobOrdersList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (jobOrders.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            isLoading = false;
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

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = JobOrdersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Job Orders: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Job Order: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void JobOrdersList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = JobOrdersList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Job Orders: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Job Order: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (YearsList.SelectedItem is int year)
            {
                string query;
                DeleteFilter_Click(sender, e);
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Finance].[JobOrderDetails] Where Year = {year}";
                    jobOrders = new ObservableCollection<JobOrderFinance>(connection.Query<JobOrderFinance>(query));
                }
                JobOrdersList.ItemsSource = viewData.View;
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(JobOrderFinance).GetProperty("Code"),
            typeof(JobOrderFinance).GetProperty("QuotationCode"),
            typeof(JobOrderFinance).GetProperty("CustomerName"),
            typeof(JobOrderFinance).GetProperty("ProjectPrice"),
            typeof(JobOrderFinance).GetProperty("Paid"),
            typeof(JobOrderFinance).GetProperty("Balance"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JobOrderFinance record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        else
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}".ToUpper();

                        if (!value.Contains(((TextBox)FindName(property.Name)).Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                e.Accepted = true;
            }
        }
        private void ApplyFilter(object sender, KeyEventArgs e)
        {
            viewData.View.Refresh();
        }
        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            foreach (PropertyInfo property in filterProperties)
            {
                ((TextBox)FindName(property.Name)).Text = null;
            }
            viewData.View.Refresh();
        }

        #endregion

        private void PostingItems_ClicK(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                string query;
                JobOrder jobOrderData;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[JobOrdersInformation] Where ID = {jobOrder.ID}";
                    jobOrderData = connection.QueryFirstOrDefault<JobOrder>(query);
                }
                StoreWindows.InvoicesWindows.InvoicesWindow invoicesWindow = new StoreWindows.InvoicesWindows.InvoicesWindow()
                {
                    UserData = UserData,
                    JobOrderData = jobOrderData,
                };
                invoicesWindow.ShowDialog();
            }
        }

        private void Invoicing_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                User usedBy;
                string query;
                JobOrder jobOrderData = new JobOrder();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, jobOrder.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderFinanceID = jobOrder.ID;
                        UserController.UpdateJobOrderID(connection, UserData);

                        query = $"Select * From [JobOrder].[JobOrdersInformation] Where ID = {jobOrder.ID}";
                        jobOrderData = connection.QueryFirstOrDefault<JobOrder>(query);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var window = new JobOrderWindows.Panels_Invoicing_Windows.InvoicingWindow()
                    {
                        UserData = UserData,
                        PanelsData = null,
                        JobOrderData = jobOrderData,
                    };
                    window.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Transactions_ClicK(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderFinanceID(connection, jobOrder.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderFinanceID = jobOrder.ID;
                        UserController.UpdateJobOrderFinanceID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    JobOrderTransactionsWindow jobOrderTransactionsWindow = new JobOrderTransactionsWindow()
                    {
                        UserData = UserData,
                        JobOrderData = jobOrder,
                    };
                    jobOrderTransactionsWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void SuppliersInvoices_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                SupplierInvoicesWindow supplierInvoicesWindow = new SupplierInvoicesWindow()
                {
                    UserData = UserData,
                    JobOrderData = jobOrder,
                };
                supplierInvoicesWindow.ShowDialog();
            }
        }

        private void JobAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if(JobOrdersList.SelectedItem is JobOrderFinance jobOrder)
            {
                string query;
                List<string> POs;
                
                JobAnalysisWindows.Profit profit;
                JobAnalysisWindows.Overhead overhead;
                JobAnalysisWindows.SalesOrder salesOrder;
                JobAnalysisWindows.Transportation transportation;
                List<JobAnalysisWindows.CustomerInvoice> customerInvoices;
                List<JobAnalysisWindows.SupplierInvoice> supplierInvoices;
                List<JobAnalysisWindows.OriginalInvoice> originalInvoices;

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [Finance].[JobAnalysis(ProjectsInformation)] Where ID = {jobOrder.ID}";
                    salesOrder = connection.QueryFirstOrDefault<JobAnalysisWindows.SalesOrder>(query);

                    query = $"Select PurchaseOrdersNumber From [JobOrder].[Panels] Where JobOrderID = {jobOrder.ID} Group By PurchaseOrdersNumber";
                    POs = connection.Query<string>(query).ToList();

                    query = $"Select * From [Finance].[JobAnalysis(CustomersInvoices)] Where JobOrderID = {jobOrder.ID}";
                    customerInvoices = connection.Query<JobAnalysisWindows.CustomerInvoice>(query).ToList();
                    
                    query = $"Select* From[Finance].[JobAnalysis(JobOrdersInvoices)] Where JobOrderID = { jobOrder.ID }";
                    supplierInvoices = connection.Query<JobAnalysisWindows.SupplierInvoice>(query).ToList();

                    query = $"Select * From[Finance].[JobAnalysis(JobOrdersInvoicesPaid)] Where JobOrderID = { jobOrder.ID }";
                    originalInvoices = connection.Query<JobAnalysisWindows.OriginalInvoice>(query).ToList();

                    overhead = new JobAnalysisWindows.Overhead(); // need work
                    transportation = new JobAnalysisWindows.Transportation(); // need work
                }

                foreach (string po in POs)
                    salesOrder.POs += $"{po}, ";

                salesOrder.POs = salesOrder.POs.Substring(0, salesOrder.POs.Length - 2);

                double paid = customerInvoices[0].Paid;
                foreach (JobAnalysisWindows.CustomerInvoice invoice in customerInvoices)
                {
                    if(paid > 0)
                    {
                        if (paid > invoice.InvoiceTotal) invoice.Paid = invoice.InvoiceTotal;
                        else invoice.Paid = paid;
                    }
                    else
                    {
                        invoice.Paid = 0;
                    }
                    paid = Math.Round(paid - invoice.InvoiceTotal, 6);
                }

                foreach (JobAnalysisWindows.SupplierInvoice invoice in supplierInvoices)
                {
                    invoice.Balance = originalInvoices.Where(i => i.InvoiceID == invoice.ID).Sum(i => i.Balance);
                    invoice.Paid = invoice.InvoiceTotal - invoice.Balance;
                }

                profit = new JobAnalysisWindows.Profit();
                profit.CostAmount = supplierInvoices.Sum(i => i.Amount);
                profit.CostVAT = supplierInvoices.Sum(i => i.VAT);
                profit.CostInvoiceTotal = supplierInvoices.Sum(i => i.InvoiceTotal);

                profit.NetMarginAmount = salesOrder.QuotationAmount - profit.CostAmount;
                profit.NetMarginVAT = salesOrder.QuotationVAT - profit.CostVAT;
                profit.NetMarginInvoiceTotal = salesOrder.QuotationInvoiceTotal - profit.CostInvoiceTotal;

                profit.NetMarginAmountPercentage = (profit.NetMarginAmount / salesOrder.QuotationAmount) * 100;
                profit.NetMarginVATPercentage = (profit.NetMarginVAT / salesOrder.QuotationVAT) * 100;
                profit.NetMarginInvoiceTotalPercentage = (profit.NetMarginInvoiceTotal / salesOrder.QuotationInvoiceTotal) * 100;

                //int page = 1;
                //double rows;
                //double cm = App.cm;
                //double BodyHeight = 670;
                Printing.Finance.JobAnalysis.JobAnalysisForm jobAnalysis = new Printing.Finance.JobAnalysis.JobAnalysisForm(jobOrder.Code, jobOrder.CustomerName);
                
                Printing.Finance.JobAnalysis.SalesOrderTable salesOrderTable = new Printing.Finance.JobAnalysis.SalesOrderTable(salesOrder);
                jobAnalysis.Body.Children.Add(salesOrderTable);

                Printing.Finance.JobAnalysis.CustomerTable customerTable = new Printing.Finance.JobAnalysis.CustomerTable(customerInvoices)
                { Margin = new Thickness(0, 5, 0, 0) };
                jobAnalysis.Body.Children.Add(customerTable);

                Printing.Finance.JobAnalysis.SupplierTable supplierTable = new Printing.Finance.JobAnalysis.SupplierTable(supplierInvoices)
                { Margin = new Thickness(0, 5, 0, 0) };
                jobAnalysis.Body.Children.Add(supplierTable);

                Printing.Finance.JobAnalysis.OverheadTable overheadTable = new Printing.Finance.JobAnalysis.OverheadTable(overhead)
                { Margin = new Thickness(0, 5, 0, 0) };
                jobAnalysis.Body.Children.Add(overheadTable);

                Printing.Finance.JobAnalysis.TransportationTable transportationTable = new Printing.Finance.JobAnalysis.TransportationTable(transportation)
                { Margin = new Thickness(0, 5, 0, 0) };
                jobAnalysis.Body.Children.Add(transportationTable);


                Printing.Finance.JobAnalysis.ProfitTable profitTable = new Printing.Finance.JobAnalysis.ProfitTable(profit)
                { Margin = new Thickness(0, 5, 0, 0) };
                jobAnalysis.Body.Children.Add(profitTable);


                //rows = (BodyHeight - (1 * cm)) / (0.5 * cm);
                //if(rows > customerInvoices.Count)
                //{
                //    Printing.Finance.JobAnalysis.CustomerTable customerTable = new Printing.Finance.JobAnalysis.CustomerTable(customerInvoices)
                //    { Margin = new Thickness(0, 5, 0, 0) }; 
                //    jobAnalysis.Body.Children.Add(customerTable);
                //    BodyHeight -= (1 + customerInvoices.Count * 0.5) * cm;
                //}
                //else
                //{
                //    //Printing.Finance.JobAnalysis.CustomerTable customerTable = 
                //    //    new Printing.Finance.JobAnalysis.CustomerTable(customerInvoices.Where(i => customerInvoices.IndexOf(i) < (Convert.ToInt32(rows) - 1)).ToList());
                //    //jobAnalysis.Body.Children.Add(customerTable);
                //    //BodyHeight -= 2 * cm;
                //}

                //rows = (BodyHeight - (1 * cm)) / (0.5 * cm);
                //if (rows > supplierInvoices.Count)
                //{
                //    Printing.Finance.JobAnalysis.SupplierTable supplierTable = new Printing.Finance.JobAnalysis.SupplierTable(supplierInvoices) 
                //    { Margin = new Thickness(0, 5, 0, 0) };
                //    jobAnalysis.Body.Children.Add(supplierTable);
                //    BodyHeight -= 2 * cm;
                //}
                //else
                //{
                //    //Printing.Finance.JobAnalysis.CustomerTable customerTable = 
                //    //    new Printing.Finance.JobAnalysis.CustomerTable(customerInvoices.Where(i => customerInvoices.IndexOf(i) < (Convert.ToInt32(rows) - 1)).ToList());
                //    //jobAnalysis.Body.Children.Add(customerTable);
                //    //BodyHeight -= 2 * cm;
                //}

                Printing.Print.PrintPreview(jobAnalysis, $"Job Analysis {jobOrder.Code}", System.Printing.PageOrientation.Landscape);
            }
        }
    }
}
