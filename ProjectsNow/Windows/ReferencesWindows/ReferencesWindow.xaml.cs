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
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class ReferencesWindow : Window
    {
        public User UserData { get; set; }

        ObservableCollection<Reference> referencesData;
        public ReferencesWindow()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.ReferencesDiscount)
                DiscountButton.Visibility = Visibility.Visible;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                referencesData = new ObservableCollection<Reference>(ReferenceController.GetReferences(connection));
            }

            viewData = new CollectionViewSource() { Source = referencesData };
            viewData.Filter += DataFilter;

            ReferencesList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            DataContext = new { UserData };
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
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = ReferencesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"References: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Reference: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void ReferencesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ReferencesList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"References: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Reference: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(Reference)).GetProperty("PartNumber"),
            (typeof(Reference)).GetProperty("Description"),
            (typeof(Reference)).GetProperty("Unit"),
            (typeof(Reference)).GetProperty("Cost"),
            (typeof(Reference)).GetProperty("Discount"),
            (typeof(Reference)).GetProperty("Brand"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is Reference record)
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

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ReferenceWindow referenceWindow = new ReferenceWindow()
            {
                UserData = UserData,
                ActionData = Actions.New,
                ReferenceData = new Reference(),
                ReferencesData = referencesData,
            };
            referenceWindow.ShowDialog();

        }

        private void Edit_ClicK(object sender, RoutedEventArgs e)
        {
            if (ReferencesList.SelectedItem is Reference referenceData)
            {
                ReferenceWindow referenceWindow = new ReferenceWindow()
                {
                    UserData = UserData,
                    ActionData = Actions.Edit,
                    ReferenceData = referenceData,
                };
                referenceWindow.ShowDialog();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReferencesList.SelectedItem is Reference referenceData)
            {
                if(referenceData.Type == "Smart")
                {
                    CMessageBox.Show("Delete", "Can't delete this reference!!");
                    return;
                }

                MessageBoxResult result = CMessageBox.Show("Deleting", $"Are you sure to delete:\n{referenceData.PartNumber}\n{referenceData.Description} ?!", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Delete From [Reference].[References] Where ReferenceID = {referenceData.ReferenceID}";
                        connection.Execute(query);
                    }

                    referencesData.Remove(referenceData);
                }
            }
        }

        private void DiscountRecalculate_Click(object sender, RoutedEventArgs e)
        {
            CategoriesDiscountsWindow categoriesDiscountsWindow = new CategoriesDiscountsWindow()
            {
                ReferencesWindowData = this,
            };
            categoriesDiscountsWindow.ShowDialog();
        }
    }
}
