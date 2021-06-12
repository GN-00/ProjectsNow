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

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class JobOrdersWindow : Window
    {
        public User UserData { get; set; }

        bool isLoading = true;
        Statuses status = Statuses.Running;

        ObservableCollection<JobOrder> JobOrdersData;
        ObservableCollection<JobOrdersYear> YearsData;
        public JobOrdersWindow()
        {
            InitializeComponent();
            UserData = new User() { JobOrderAcknowledgement = true };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.JobOrderInformation == false) Information.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderAcknowledgement == false) AcknowledgementButton.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderPurchaseOrders == false) PurchaseOrders.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderPanels == false) Panels.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderPanels == false) Panels.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderPosting == false) Posting.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderInvoicing == false) Invoicing.Visibility = Visibility.Collapsed;

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                JobOrdersData = JobOrderController.GetRunningJobOrders(connection, DateTime.Now.Year);
                YearsData = JobOrderController.GetRunningJobOrdersYears(connection);
            }
            
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = JobOrdersData };
            viewData.Filter += DataFilter;

            JobOrdersList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (JobOrdersData.Count == 0)
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

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                var quotationWindow = new QuotationWindows.QuotationsInformationWindows.QuotationWindow()
                {
                    UserData = this.UserData,
                    QuotationID = jobOrderData.QuotationID,
                };
                quotationWindow.ShowDialog();
            }
        }
        private void Acknowledgement_Click(object sender, RoutedEventArgs e)
        {
            AcknowledgementPopup.IsOpen = true;
        }
        private void EditAcknowledgement_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                User usedBy;
                string query;
                Acknowledgment acknowledgement;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[Acknowledgment] Where JobOrderID = {jobOrderData.ID}";
                    acknowledgement = connection.QueryFirstOrDefault<Acknowledgment>(query);

                    if(acknowledgement == null)
                    {
                        acknowledgement = new Acknowledgment() { JobOrderID = jobOrderData.ID};
                        query = $"{DatabaseAI.InsertRecord<Acknowledgment>()}";
                        acknowledgement.ID = (int)(decimal)connection.ExecuteScalar(query, acknowledgement);
                    }
                    usedBy = UserController.CheckUserAcknowledgementID(connection, acknowledgement.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.AcknowledgementID = acknowledgement.ID;
                        UserController.UpdateAcknowledgementID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    AcknowledgementWindow acknowledgementWindow = new AcknowledgementWindow()
                    {
                        UserData = this.UserData,
                        AcknowledgementData = acknowledgement,
                    };
                    acknowledgementWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void PrintAcknowledgement_Click(object sender, RoutedEventArgs e)
        {
            if(JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                string query;
                List<string> POs;
                Acknowledgment acknowledgementData;
                AcknowledgmentInformation acknowledgementInformationData;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Select * From [JobOrder].[Acknowledgment] Where JobOrderID = {jobOrderData.ID}";
                    acknowledgementData = connection.QueryFirstOrDefault<Acknowledgment>(query);

                    if (acknowledgementData == null)
                    {
                        acknowledgementData = new Acknowledgment() { JobOrderID = jobOrderData.ID };
                        query = $"{DatabaseAI.InsertRecord<Acknowledgment>()}";
                        acknowledgementData.ID = (int)(decimal)connection.ExecuteScalar(query, acknowledgementData);
                    }

                    query = $"Select * From [JobOrder].[AcknowledgmentsInformations] Where JobOrderID = {jobOrderData.ID}";
                    acknowledgementInformationData = connection.QueryFirstOrDefault<AcknowledgmentInformation>(query);

                    query = $"Select Number From [JobOrder].[PurchaseOrders] Where JobOrderID ={jobOrderData.ID}";
                    POs = connection.Query<string>(query).ToList();
                }

                if(POs.Count != 0)
                {
                    foreach (string po in POs)
                        acknowledgementInformationData.POs += $"{po}, ";

                    acknowledgementInformationData.POs = acknowledgementInformationData.POs.Substring(0, acknowledgementInformationData.POs.Length - 2);
                }

                OrderAcknowledgement acknowledgementForm = new OrderAcknowledgement() { AcknowledgementInformationData = acknowledgementInformationData };
                FrameworkElement element = acknowledgementForm;
                Print.PrintPreview(element, $"Order Acknowledgement-{jobOrderData.Code}");
            }
        }
        private void PO_ClicK(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, jobOrderData.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderID = jobOrderData.ID;
                        UserController.UpdateJobOrderID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    PurchaseOrdersWindow purchaseOrdersWindow = new PurchaseOrdersWindow()
                    {
                        UserData = this.UserData,
                        JobOrderData = jobOrderData,
                    };
                    purchaseOrdersWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Panels_ClicK(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, jobOrderData.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderID = jobOrderData.ID;
                        UserController.UpdateJobOrderID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var panelsWindow = new PanelsWindow()
                    {
                        UserData = this.UserData,
                        JobOrderData = jobOrderData,
                    };
                    panelsWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This job order underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void AllJobOrders_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.All;
            StatusName.Text = $"All Job Orders";
            StatusName.Foreground = YearValue.Foreground = Brushes.Black;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                YearsData = JobOrderController.JobOrdersYears(connection);
                viewData.Source = JobOrdersData = JobOrderController.JobOrders(connection, DateTime.Now.Year);
            }
            YearsList.ItemsSource = YearsData;
            JobOrdersList.ItemsSource = viewData.View;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            isLoading = false;
        }
        private void RunningJobOrders_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Running;
            StatusName.Text = $"Running Job Orders";
            StatusName.Foreground = YearValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9211E8"));
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                YearsData = JobOrderController.GetRunningJobOrdersYears(connection);
                viewData.Source = JobOrdersData = JobOrderController.GetRunningJobOrders(connection, DateTime.Now.Year);
            }
            YearsList.ItemsSource = YearsData;
            JobOrdersList.ItemsSource = viewData.View;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            isLoading = false;
        }
        private void ClosedJobOrders_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            status = Statuses.Closed;
            StatusName.Text = $"Closed Job Orders";
            StatusName.Foreground = YearValue.Foreground = Brushes.Gray;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                YearsData = JobOrderController.GetClosedJobOrdersYears(connection);
                viewData.Source = JobOrdersData = JobOrderController.GetClosedJobOrders(connection, DateTime.Now.Year);
            }
            YearsList.ItemsSource = YearsData;
            JobOrdersList.ItemsSource = viewData.View;

            YearsList.SelectedItem = YearsData.FirstOrDefault(i => i.Year == DateTime.Now.Year);
            YearValue.Text = DateTime.Now.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            isLoading = false;
        }
        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading)
                return;

            if (YearsList.SelectedItem is JobOrdersYear yearData)
            {
                DeleteFilter_Click(sender, e);
                YearValue.Text = yearData.Year.ToString();
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    if (status == Statuses.All)
                        viewData.Source = JobOrdersData = JobOrderController.JobOrders(connection, yearData.Year);
                    else if (status == Statuses.Running)
                        viewData.Source = JobOrdersData = JobOrderController.GetRunningJobOrders(connection, yearData.Year);
                    else if (status == Statuses.Closed)
                        viewData.Source = JobOrdersData = JobOrderController.GetClosedJobOrders(connection, yearData.Year);

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
            typeof(JobOrder).GetProperty("Code"),
            typeof(JobOrder).GetProperty("QuotationCode"),
            typeof(JobOrder).GetProperty("CustomerName"),
            typeof(JobOrder).GetProperty("ProjectName"),
            typeof(JobOrder).GetProperty("EstimationName"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JobOrder record)
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

        private void DiscountSheets_ClicK(object sender, RoutedEventArgs e)
        {
            QuotationWindows.DiscountSheetsWindow discountSheetsWindow = new QuotationWindows.DiscountSheetsWindow();
            discountSheetsWindow.ShowDialog();
        }

        private void PostingItems_ClicK(object sender, RoutedEventArgs e)
        {
            if(JobOrdersList.SelectedItem is JobOrder jobOrder)
            {
                StoreWindows.InvoicesWindows.InvoicesWindow invoicesWindow = new StoreWindows.InvoicesWindows.InvoicesWindow()
                {
                    UserData = UserData,
                    JobOrderData = jobOrder,
                };
                invoicesWindow.ShowDialog();
            }
        }

        private void Invoicing_Click(object sender, RoutedEventArgs e)
        {
            if (JobOrdersList.SelectedItem is JobOrder jobOrderData)
            {
                User usedBy;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserJobOrderID(connection, jobOrderData.ID);

                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.JobOrderID = jobOrderData.ID;
                        UserController.UpdateJobOrderID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    var window = new Panels_Invoicing_Windows.InvoicingWindow()
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

        private void PurchaseItems_ClicK(object sender, RoutedEventArgs e)
        {
            if(JobOrdersList.SelectedItem is JobOrder jobOrder)
            {
                ItemPurchaseOrdersWindows.PurchaseOrdersWindow purchaseOrdersWindow = new ItemPurchaseOrdersWindows.PurchaseOrdersWindow()
                {
                    UserData = UserData,
                    JobOrderData = jobOrder,
                };
                purchaseOrdersWindow.ShowDialog();
            }
        }
    }
}
