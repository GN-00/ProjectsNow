using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Enums;
using System.Windows.Data;
using ProjectsNow.Printing;
using System.Windows.Input;
using System.Windows.Media;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using Excel = Microsoft.Office.Interop.Excel;

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

        //private void AllJobOrders_Click(object sender, RoutedEventArgs e)
        //{
        //    DeleteFilter_Click(sender, e);
        //    isLoading = true;
        //    status = Statuses.All;
        //    StatusName.Text = $"All Job Orders";
        //    StatusName.Foreground = YearValue.Foreground = Brushes.Black;
        //    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
        //    {
        //        YearsData = JobOrderController.JobOrdersYears(connection);
        //        viewData.Source = jobOrders = JobOrderController.JobOrders(connection, DateTime.Now.Year);
        //    }
        //    YearsList.ItemsSource = YearsData;
        //    JobOrdersList.ItemsSource = viewData.View;

        //    YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
        //    YearValue.Text = DateTime.Now.Year.ToString();

        //    CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //    viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

        //    isLoading = false;
        //}
        //private void RunningJobOrders_Click(object sender, RoutedEventArgs e)
        //{
        //    DeleteFilter_Click(sender, e);
        //    isLoading = true;
        //    status = Statuses.Running;
        //    StatusName.Text = $"Running Job Orders";
        //    StatusName.Foreground = YearValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9211E8"));
        //    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
        //    {
        //        YearsData = JobOrderController.GetRunningJobOrdersYears(connection);
        //        viewData.Source = jobOrders = JobOrderController.GetRunningJobOrders(connection, DateTime.Now.Year);
        //    }
        //    YearsList.ItemsSource = YearsData;
        //    JobOrdersList.ItemsSource = viewData.View;

        //    YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
        //    YearValue.Text = DateTime.Now.Year.ToString();

        //    CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //    viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

        //    isLoading = false;
        //}
        //private void ClosedJobOrders_Click(object sender, RoutedEventArgs e)
        //{
        //    DeleteFilter_Click(sender, e);
        //    isLoading = true;
        //    status = Statuses.Closed;
        //    StatusName.Text = $"Closed Job Orders";
        //    StatusName.Foreground = YearValue.Foreground = Brushes.Gray;
        //    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
        //    {
        //        YearsData = JobOrderController.GetClosedJobOrdersYears(connection);
        //        viewData.Source = jobOrders = JobOrderController.GetClosedJobOrders(connection, DateTime.Now.Year);
        //    }
        //    YearsList.ItemsSource = YearsData;
        //    JobOrdersList.ItemsSource = viewData.View;

        //    YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
        //    YearValue.Text = DateTime.Now.Year.ToString();

        //    CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //    viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

        //    isLoading = false;
        //}
        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (YearsList.SelectedItem is QuotationsYear year)
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
    }
}
