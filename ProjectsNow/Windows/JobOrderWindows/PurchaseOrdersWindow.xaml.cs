using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Reflection;
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

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PurchaseOrdersWindow : Window
    {
        public User UserData { get; set; }
        public int JobOrderID { get; set; }
        public JobOrder JobOrderData { get; set; }

        ObservableCollection<JPanelDetails> panels;
        ObservableCollection<PurchaseOrder> purchaseOrders;

        public PurchaseOrdersWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (JobOrderData == null)
                    JobOrderData = JobOrderController.JobOrder(connection, JobOrderID);
                
                purchaseOrders = PurchaseOrderController.GetPurchaseOrders(connection, JobOrderData.ID);
                panels = JPanelDetailsController.GetJobOrderPanels(connection, JobOrderData.ID);
            }

            viewDataPO = new CollectionViewSource() { Source = purchaseOrders };
            viewDataPanels = new CollectionViewSource() { Source = panels };
            
            viewDataPanels.Filter += DataFilter;

            POList.ItemsSource = viewDataPO.View;
            PanelsList.ItemsSource = viewDataPanels.View;

            viewDataPO.View.CollectionChanged += new NotifyCollectionChangedEventHandler(PO_CollectionChanged);
            viewDataPanels.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Panels_CollectionChanged);

            if (purchaseOrders.Count == 0)
                PO_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (panels.Count == 0)
                Panels_CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            DataContext = new { JobOrderData, UserData };
        }
        private void PO_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            else
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
        }
        private void Panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        #region Filters

        CollectionViewSource viewDataPO;
        CollectionViewSource viewDataPanels;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(JPanel)).GetProperty("PurchaseOrdersNumber"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanelDetails record)
                {
                    string columnName;
                    foreach (PropertyInfo property in filterProperties)
                    {
                        columnName = property.Name;
                        string value;
                        if (property.PropertyType == typeof(DateTime))
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record):dd/MM/yyyy}";
                        else
                            value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";

                        if (POList.SelectedItem is PurchaseOrder purchaseOrder)
                            if (value != purchaseOrder.Number)
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
            PurchaseOrderWindow purchaseOrderWindow = new PurchaseOrderWindow()
            {
                ActionData = Actions.New,
                PurchaseOrdersData = this.purchaseOrders,
                JobOrderID = JobOrderData.ID
            };
            purchaseOrderWindow.ShowDialog();
        }
        private void EditPO_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is PurchaseOrder purchaseOrdersData)
            {
                PurchaseOrderWindow purchaseOrderWindow = new PurchaseOrderWindow()
                {
                    ActionData = Actions.Edit,
                    PurchaseOrdersData = this.purchaseOrders,
                    PurchaseOrderData = purchaseOrdersData
                };
                purchaseOrderWindow.ShowDialog();
            }
        }
        private void DeletePO_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is PurchaseOrder purchaseOrderData)
            {
                if (panels.Where(panel => panel.Status != Statuses.New.ToString()).Count() != 0)
                {
                    CMessageBox.Show("Error", "You Can't delete this P.O!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }

                string query;
                string panelsIDs = "";
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Delete From [JobOrder].[PurchaseOrders] Where ID = {purchaseOrderData.ID}; ";

                    int panelIndex = 0;
                    if (panels.Where(panel => panel.PurchaseOrdersNumber == purchaseOrderData.Number).Count() != 0)
                    {
                        foreach (JPanelDetails panelData in panels.Where(panel => panel.PurchaseOrdersNumber == purchaseOrderData.Number))
                        {
                            if (panelIndex == 0)
                            { panelsIDs += $"{panelData.PanelID}"; panelIndex++; }
                            else
                            { panelsIDs += $", {panelData.PanelID}"; }
                        }
                        query += $"Delete From [JobOrder].[Panels] Where PanelID In ({panelsIDs}); ";
                        query += $"Delete From [JobOrder].[PanelsItems] Where PanelID In ({panelsIDs}); ";
                        query += $"Update [Quotation].[QuotationsPanels] Set PurchaseOrdersNumber = NULL Where PanelID In ({panelsIDs}); ";
                    }

                    connection.Execute(query);
                }

                purchaseOrders.Remove(purchaseOrderData);
                if (purchaseOrders.Count == 0)
                    panels.Clear();
            }
        }
        private void AddPanels_Click(object sender, RoutedEventArgs e)
        {
            if (POList.SelectedItem is PurchaseOrder purchaseOrderData)
            {
                QuotationPanelsWindow quotationPanelsWindow = new QuotationPanelsWindow()
                {
                    JobOrderData = JobOrderData,
                    PurchaseOrderNumber = purchaseOrderData.Number,
                    JobOrderPanels = panels,
                };
                quotationPanelsWindow.ShowDialog();
            }
        }

        private void DeletePanels_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is JPanelDetails panelData)
            {
                if (panelData.Status != Statuses.New.ToString())
                {
                    CMessageBox.Show("Error", "You Can't delete this Panel!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }

                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = $"Delete From [JobOrder].[Panels] Where PanelID = {panelData.PanelID}; ";
                    query += $"Delete From [JobOrder].[PanelsItems] Where PanelID = {panelData.PanelID}; ";
                    query += $"Update [Quotation].[QuotationsPanels] Set PurchaseOrdersNumber = NULL Where PanelID = {panelData.PanelID}; ";

                    connection.Execute(query);
                }

                panels.Remove(panelData);
            }
        }

        private void POList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            viewDataPanels.View.Refresh();
            var selectedIndex = POList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPO.Text = $"Purchase Orders: {viewDataPO.View.Cast<object>().Count()}";
            else
                NavigationPO.Text = $"Purchase Order: {selectedIndex + 1} / {viewDataPO.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewDataPanels.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewDataPanels.View.Cast<object>().Count()}";
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
