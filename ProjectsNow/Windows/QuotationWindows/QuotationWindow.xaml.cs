using System;
using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using ProjectsNow.Windows.CustomerWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationWindow : Window
    {
        public User UserData { get; set; }
        public bool OpenPanelsWindow { get; set; }
        public Quotation QuotationData { get; set; }


        CollectionViewSource viewProjectContacts;
        ObservableCollection<Contact> projectContacts;

        Customer customerData;
        Consultant consultantData;
        List<int> contactsIDs = new List<int>();
        Quotation newQuotationData = new Quotation();

        public QuotationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newQuotationData.Update(QuotationData);

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                projectContacts = ContactController.GetProjectContacts(connection, newQuotationData.InquiryID);
                viewProjectContacts = new CollectionViewSource { Source = projectContacts };
                ProjectContactsList.ItemsSource = viewProjectContacts.View;

                foreach (Contact contact in ProjectContactsList.ItemsSource)
                {
                    contactsIDs.Add(contact.ContactID);
                }

                CustomerList.ItemsSource = CustomerController.GetCustomers(connection);

                SalesList.ItemsSource = SalesmanController.GetSalesmen(connection);

                EstimationList.ItemsSource = EstimationController.GetEstimation(connection);

                ConsultantList.ItemsSource = ConsultantController.GetConsultants(connection);
            }

            if (OpenPanelsWindow) Cancel.Visibility = Visibility.Collapsed;

            viewProjectContacts.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            DataContext = new { newQuotationData, customerData, consultantData };
        }
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
        private void CustomerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customerData = (((ComboBox)sender).SelectedItem as Customer);
            newQuotationData.CustomerName = customerData.CustomerName;
            DataContext = new { newQuotationData, customerData, consultantData };
        }
        private void Consultant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            consultantData = (((ComboBox)sender).SelectedItem as Consultant);
            DataContext = new { newQuotationData, customerData, consultantData };
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isNull = false;
            var message = "Please Enter:";
            if (newQuotationData.CustomerID == 0) { message += $"\n  Customer Name."; isNull = true; }
            if (newQuotationData.ProjectName == null || newQuotationData.ProjectName == "") { message += $"\n  Project Name."; isNull = true; }
            if (newQuotationData.PowerVoltage == null || newQuotationData.PowerVoltage == "") { message += $"\n  Power Voltage."; isNull = true; }
            if (newQuotationData.Phase == null || newQuotationData.Phase == "") { message += $"\n  Phase."; isNull = true; }
            if (newQuotationData.Frequency == null || newQuotationData.Frequency == "") { message += $"\n  Frequency."; isNull = true; }
            if (newQuotationData.NetworkSystem == null || newQuotationData.NetworkSystem == "") { message += $"\n  Network System."; isNull = true; }
            if (newQuotationData.ControlVoltage == null || newQuotationData.ControlVoltage == "") { message += $"\n  Control Voltage."; isNull = true; }
            if (newQuotationData.TinPlating == null || newQuotationData.TinPlating == "") { message += $"\n  Tin Plating."; isNull = true; }
            if (newQuotationData.NeutralSize == null || newQuotationData.NeutralSize == "") { message += $"\n  Neutral Size."; isNull = true; }
            if (newQuotationData.EarthSize == null || newQuotationData.EarthSize == "") { message += $"\n  Earth Size."; isNull = true; }
            if (newQuotationData.EarthingSystem == null || newQuotationData.EarthingSystem == "") { message += $"\n  Earthing System."; isNull = true; }

            if (!isNull)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    string query;
                    query = DatabaseAI.UpdateRecord<Inquiry>();
                    connection.Execute(query, newQuotationData);

                    query = DatabaseAI.UpdateRecord<Quotation>();
                    connection.Execute(query, newQuotationData);
                }

                QuotationData.Update(newQuotationData);
                if (OpenPanelsWindow)
                {
                    var quotationPanelsWindow = new QuotationPanelsWindow()
                    {
                        UserData = this.UserData,
                        QuotationData = this.QuotationData
                    };
                    this.Close();
                    quotationPanelsWindow.ShowDialog();
                }
                else
                {
                    this.CloseWindow_Click(sender, e);
                }

            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow_Click(sender, e);
        }

        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (projectContacts.Count != 0)
            {
                var selectedIndex = ProjectContactsList.SelectedIndex;
                NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewProjectContacts.View.Cast<object>().Count()}";
            }
            else
            {
                NavigationPanel.Text = $"Contact: 0";
            }
        }
        private void ProjectContactsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ProjectContactsList.SelectedIndex;
            NavigationPanel.Text = $"Contact: {selectedIndex + 1} / {viewProjectContacts.View.Cast<object>().Count()}";
        }
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            PowerVoltage.Text = "220-380V";
            Phase.Text = "3P + N";
            Frequency.Text = "60Hz";
            NetworkSystem.Text = "AC";
            ControlVoltage.Text = "230V AC";
            TinPlating.Text = "Bare Copper";
            NeutralSize.Text = "Full of Phase";
            EarthSize.Text = "Half of Neutral";
            EarthingSystem.Text = "TNS";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.QuotationID = null;
                UserController.UpdateQuotationID(connection, UserData);
            }
        }
    }
}
