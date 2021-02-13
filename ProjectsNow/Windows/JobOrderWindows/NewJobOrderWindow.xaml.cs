using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class NewJobOrderWindow : Window
    {
        public User UserData { get; set; }

        bool isLoading = true;
        string listData = "Quotation";

        ObservableCollection<JobOrder> JobOrdersData;
        ObservableCollection<Quotation> QuotationsData;

        ObservableCollection<JobOrdersYear> JobOrdersYearsData;
        ObservableCollection<QuotationsYear> QuotationsYearsData;

        public NewJobOrderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                QuotationsYearsData = JobOrderController.QuotationsWaitingPOYears(connection);

                QuotationsData = JobOrderController.QuotationsWaitPO(connection, DateTime.Today.Year);
            }
            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = QuotationsData };
            viewData.Filter += DataFilter;
            

            QuotationsList.ItemsSource = viewData.View;
            YearsList.ItemsSource = QuotationsYearsData;
            YearsList.Text = DateTime.Today.Year.ToString();
            YearValue.Text = DateTime.Today.Year.ToString();

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (QuotationsData.Count == 0)
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
            var selectedIndex = QuotationsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"{listData}s: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"{listData}: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void QuotationsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = QuotationsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"{listData}s: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"{listData}: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void Years_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoading)
            {
                if (listData == "Quotation")
                {
                    if (YearsList.SelectedItem is QuotationsYear yearData)
                    {
                        YearValue.Text = yearData.Year.ToString();
                        DeleteFilter_Click(sender, e);
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            viewData.Source = QuotationsData = JobOrderController.QuotationsWaitPO(connection, yearData.Year);
                        }
                        QuotationsList.ItemsSource = viewData.View;
                        CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
                    }
                }
                else
                {
                    if (YearsList.SelectedItem is JobOrdersYear yearData)
                    {
                        YearValue.Text = yearData.Year.ToString();
                        DeleteFilter_Click(sender, e);
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            viewData.Source = JobOrdersData = JobOrderController.JobOrders(connection, yearData.Year);
                        }
                        QuotationsList.ItemsSource = viewData.View;
                        CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
                    }
                }
            }
        }

        private void Quotations_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            listData = "Quotation";
            ListName.Text = $"{listData}s";
            ListName.Foreground = Brushes.Blue;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                QuotationsYearsData = JobOrderController.QuotationsWaitingPOYears(connection);
                viewData.Source = QuotationsData = JobOrderController.QuotationsWaitPO(connection, DateTime.Today.Year);
            }
            QuotationsList.ItemsSource = viewData.View;

            YearsList.ItemsSource = QuotationsYearsData;

            YearsList.SelectedItem = QuotationsYearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            StartJobOrderButton.Visibility = Visibility.Visible;
            QuotationsList.RowStyle = (Style)Resources["QuotationsRowStyle"];

            isLoading = false;
        }
        private void JobOrders_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter_Click(sender, e);
            isLoading = true;
            listData = "Job Order";
            ListName.Text = $"{listData}s";
            ListName.Foreground = Brushes.Gray;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                JobOrdersYearsData = JobOrderController.JobOrdersYears(connection);
                viewData.Source = JobOrdersData = JobOrderController.JobOrders(connection, DateTime.Today.Year);
            }
            QuotationsList.ItemsSource = viewData.View;

            YearsList.ItemsSource = JobOrdersYearsData;
            YearsList.SelectedItem = JobOrdersYearsData.FirstOrDefault(i => i.Year == DateTime.Today.Year);
            YearValue.Text = DateTime.Today.Year.ToString();

            CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            StartJobOrderButton.Visibility = Visibility.Collapsed;
            QuotationsList.RowStyle = (Style)Resources["JobOrdersRowStyle"];

            isLoading = false;
        }

        private void StartJobOrder_Click(object sender, RoutedEventArgs e)
        {
            if(QuotationsList.SelectedItem is Quotation quotationData)
            {
                MessageBoxResult result = CMessageBox.Show("New Order", $"Are you sure you want to register\nQ.Code: {quotationData.RegisterCode} \nas job order?", CMessageBoxButton.YesNo, CMessageBoxImage.Information);
                
                if (result == MessageBoxResult.No)
                    return;
                JobOrder jobOrder;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    jobOrder = new JobOrder()
                    {
                        QuotationID = quotationData.QuotationID,
                        Date = DateTime.Today,
                        CodeNumber = JobOrderController.GetCodeNumber(connection),
                        CodeMonth = DateTime.Today.Month,
                        CodeYear = DateTime.Today.Year,
                    };

                    if (jobOrder.CodeNumber != 0)
                    {
                        if (jobOrder.CodeNumber > 99)
                            jobOrder.Code = $"{(jobOrder.CodeYear - DatabaseAI.CompanyCreationYear) * 1000 + jobOrder.CodeNumber}/{jobOrder.CodeMonth:00}/{jobOrder.CodeYear.ToString().Substring(2, 2)}";

                        else
                            jobOrder.Code = $"{(jobOrder.CodeYear - DatabaseAI.CompanyCreationYear) * 100 + jobOrder.CodeNumber}/{jobOrder.CodeMonth:00}/{jobOrder.CodeYear.ToString().Substring(2, 2)}";
                    }

                    string query = DatabaseAI.InsertRecord<JobOrder>();
                    jobOrder.ID = (int)(decimal)connection.ExecuteScalar(query, jobOrder);
                }

                this.Close();
                PurchaseOrdersWindow purchaseOrdersWindow = new PurchaseOrdersWindow()
                {
                    UserData = UserData,
                    JobOrderID = jobOrder.ID,
                };
                purchaseOrdersWindow.ShowDialog();
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(Quotation)).GetProperty("QuotationCode"),
            (typeof(Quotation)).GetProperty("CustomerName"),
            (typeof(Quotation)).GetProperty("ProjectName"),
            (typeof(Quotation)).GetProperty("EstimationName"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Quotation  || e.Item is JobOrder)
                {
                    var record = e.Item;
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

    }
}
