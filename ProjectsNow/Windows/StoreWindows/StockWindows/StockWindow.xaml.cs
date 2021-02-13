using Dapper;
using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using ProjectsNow.Database;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.StoreWindows.StockWindows
{
    public partial class StockWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }

        ObservableCollection<ItemStock> itemsData;
        public StockWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (JobOrderData == null)
                {
                    JobOrderData = new JobOrder();
                    JobOrderData.Update(DatabaseAI.store);
                    UserData.JobOrderID = 0;
                    UserController.UpdateJobOrderID(connection, UserData);
                }

                string query = $"Select * From [Store].[JobOrderStock(AvgCost)] Where JobOrderID = {JobOrderData.ID}";
                itemsData = new ObservableCollection<ItemStock>(connection.Query<ItemStock>(query));
            }

            viewData = new CollectionViewSource() { Source = itemsData };
            viewData.Filter += DataFilter;
            ItemsList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (viewData.View.Cast<object>().Count() == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            DataContext = new { UserData, JobOrderData };
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
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Items: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Item: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            typeof(ItemStock).GetProperty("PartNumber"),
            typeof(ItemStock).GetProperty("Description"),
            typeof(ItemStock).GetProperty("Unit"),
            typeof(ItemStock).GetProperty("Qty"),
            typeof(ItemStock).GetProperty("Brand"),
            typeof(ItemStock).GetProperty("AvgCost"),
            typeof(ItemStock).GetProperty("TotalAvgCost"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is ItemStock record)
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
                        else if (e.Item is ItemStock item)
                        {
                            if (item.Qty == 0)
                            {
                                e.Accepted = false;
                                return;
                            }
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

        private void SelectJobOrder_Click(object sender, RoutedEventArgs e)
        {
            var oldJobOrderID = JobOrderData.ID;
            JobOrderSelectorWindow jobOrderSelectorWindow = new JobOrderSelectorWindow()
            {
                UserData = UserData,
                JobOrderData = JobOrderData,
                StockWindowData = this,
            };
            jobOrderSelectorWindow.ShowDialog();

            if (JobOrderData.ID != oldJobOrderID)
                this.Window_Loaded(sender, e);
        }
        private void PostItems_Click(object sender, RoutedEventArgs e)
        {
            InvoicesWindows.InvoicesWindow invoicesWindow = new InvoicesWindows.InvoicesWindow()
            {
                UserData = UserData,
                JobOrderData = JobOrderData,
            };
            invoicesWindow.ShowDialog();
            this.Window_Loaded(sender, e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.JobOrderID = null;
                UserController.UpdateJobOrderID(connection, UserData);
            }
        }

        private void ReturnToStock_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is ItemStock item)
            {
                ReturnItemsWindows.ReturnItemsWindow returnItemsWindow = new ReturnItemsWindows.ReturnItemsWindow()
                {
                    UserData = UserData,
                    ItemData = item,
                    JobOrderData = JobOrderData,
                };
                returnItemsWindow.ShowDialog();
                this.Window_Loaded(sender, e);
            }
        }
    }
}
