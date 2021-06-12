using System;
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

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class PanelsWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }

        ObservableCollection<JPanel> panelsData;
        public PanelsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.JobOrderPanelsToolTab == false) ToolTab.Visibility = Visibility.Collapsed;
            if (UserData.JobOrderPanelsMaterialsTab == false) MaterialsTab.Visibility = Visibility.Collapsed;

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                panelsData = JPanelController.GetJobOrderPanels(connection, JobOrderData.ID);
            }
            viewData = new CollectionViewSource() { Source = panelsData };
            viewData.Filter += DataFilter;
            PanelsList.ItemsSource = viewData.View;
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
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void EditPanel_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is JPanel panelData)
            {
                PanelWindow panelWindow = new PanelWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                    PanelsData = this.panelsData,
                };
                panelWindow.ShowDialog();
            }
        }
        private void Progression_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is JPanel panelData)
            {
                PanelProgressionWindow panelProgressionWindow = new PanelProgressionWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                };
                panelProgressionWindow.ShowDialog();
            }
        }
        private void Design_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Design_Window()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Approval_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Approval_Windows.RequestsWindow()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Production_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Production_Window()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Closing_Windows.PanelsWindow()
            {
                UserData = UserData,
                PanelsData = panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Invoicing_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Invoicing_Windows.InvoicingWindow()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Delivery_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Delivery_Windows.DeliveryWindow()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Hold_Windows.PanelsWindow()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = new Panels_Cancelling_Windows.PanelsWindow()
            {
                UserData = UserData,
                PanelsData = this.panelsData,
                JobOrderData = this.JobOrderData,
            };
            window.ShowDialog();
            this.Window_Loaded(sender, e);
        }

        private void Material_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is JPanel panelData)
            {
                var panelItemsWindow = new PanelItemsWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                    JobOrderData = this.JobOrderData,
                };
                panelItemsWindow.ShowDialog();
            }
        }
        private void Modification_Click(object sender, RoutedEventArgs e)
        {
            ItemsModificationWindow itemsModificationWindow = new ItemsModificationWindow()
            {
                UserData = this.UserData,
                JobOrderData = this.JobOrderData,
            };

            itemsModificationWindow.ShowDialog();
            Window_Loaded(sender, e);
        }
        private void ModificationsHistory_Click(object sender, RoutedEventArgs e)
        {
            ModificationsHistoryWindow modificationsHistoryWindow = new ModificationsHistoryWindow()
            {
                UserData = this.UserData,
                JobOrderData = this.JobOrderData,
            };
            modificationsHistoryWindow.ShowDialog();
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(JPanel)).GetProperty("PanelSN"),
            (typeof(JPanel)).GetProperty("PanelName"),
            (typeof(JPanel)).GetProperty("PanelQty"),
            (typeof(JPanel)).GetProperty("Status"),
            (typeof(JPanel)).GetProperty("EnclosureType"),
            (typeof(JPanel)).GetProperty("EnclosureHeight"),
            (typeof(JPanel)).GetProperty("EnclosureWidth"),
            (typeof(JPanel)).GetProperty("EnclosureDepth"),
            (typeof(JPanel)).GetProperty("EnclosureIP"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is JPanel record)
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.JobOrderID = null;
                UserController.UpdateJobOrderID(connection, UserData);
            }
        }
    }
}
