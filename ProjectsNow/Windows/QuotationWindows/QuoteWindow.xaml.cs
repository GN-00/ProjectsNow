using System;
using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuoteWindow : Window
    {
        public User UserData { get; set; }

        ObservableCollection<Inquiry> InquiriesData;
        public QuoteWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                InquiriesData = InquiryController.GetNewInquiries(connection, UserData);
            }

            DataContext = new { UserData };

            viewData = new CollectionViewSource() { Source = InquiriesData };
            viewData.Filter += DataFilter;

            InquiriesList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

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
        private void Quote_Click(object sender, RoutedEventArgs e)
        {
            if (InquiriesList.SelectedItem is Inquiry inquiryData)
            {
                User usedBy;
                Quotation quotationData = new Quotation(inquiryData);

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserInquiryID(connection, quotationData.InquiryID);

                    if (usedBy == null)
                    {
                        quotationData.QuotationNumber = QuotationController.NewQuotationNumber(connection, DateTime.Now.Year);
                        quotationData.QuotationYear = DateTime.Now.Year;
                        quotationData.QuotationMonth = DateTime.Now.Month;
                        quotationData.QuotationCode =
                            $"ER-{quotationData.QuotationNumber:000}/{UserData.UserCode}/{quotationData.QuotationMonth}/{quotationData.QuotationYear}/R00";
                        quotationData.QuotationReviseDate = DateTime.Now;

                        var query = DatabaseAI.InsertRecord<Quotation>();
                        quotationData.QuotationID = (int)(decimal)connection.ExecuteScalar(query, quotationData);
                        TermController.DefaultTerms(connection, quotationData.QuotationID);

                        UserData.InquiryID = inquiryData.InquiryID;
                        UserController.UpdateInquiryID(connection, UserData);

                        UserData.QuotationID = quotationData.QuotationID;
                        UserController.UpdateQuotationID(connection, UserData);
                    }
                }

                if (usedBy == null)
                {
                    var quotationWindow = new QuotationWindow()
                    {
                        UserData = this.UserData,
                        OpenPanelsWindow = true,
                        QuotationData = quotationData
                    };
                    this.Close();
                    quotationWindow.ShowDialog();
                }
                else
                {
                    CMessageBox.Show($"Access", $"This inquiry underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }


        private void InquiriesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = InquiriesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Inquiries: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Inquiry: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = InquiriesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Inquiries: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Inquiry: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }


        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(Inquiry)).GetProperty("RegisterCode"),
            (typeof(Inquiry)).GetProperty("CustomerName"),
            (typeof(Inquiry)).GetProperty("ProjectName"),
            (typeof(Inquiry)).GetProperty("EstimationName"),
            (typeof(Inquiry)).GetProperty("RegisterDate"),
            (typeof(Inquiry)).GetProperty("DuoDate"),
            (typeof(Inquiry)).GetProperty("Priority"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Inquiry record)
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

    }
}
