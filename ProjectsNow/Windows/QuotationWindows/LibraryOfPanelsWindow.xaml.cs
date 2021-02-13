using System;
using Dapper;
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

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class LibraryOfPanelsWindow : Window
    {
        public ObservableCollection<QPanel> panelsData;

        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }
        public Quotation QuotationToUpdate { get; set; }
        public ObservableCollection<QPanel> PanelsData { get; set; }

        public LibraryOfPanelsWindow()
        {
            InitializeComponent();
            QuotationData = new Quotation()
            {
                QuotationID = 0,
                QuotationCode = "Library",
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                panelsData = QPanelController.QuotationPanels(connection, QuotationData.QuotationID);
            }

            viewData = new CollectionViewSource() { Source = panelsData };
            viewData.Filter += DataFilter;

            PanelsList.ItemsSource = viewData.View;
            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            DataContext = new { UserData, QuotationData };
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

        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void PanelsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = PanelsList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }

        private void AddPanel_Click(object sender, RoutedEventArgs e)
        {
            var qPanelWindow = new QPanelWindow()
            {
                ActionData = Actions.New,
                QPanelsData = this.panelsData,
                SelectedIndex = this.panelsData.Count,
                QuotationData = this.QuotationData,
            };
            qPanelWindow.ShowDialog();
        }
        private void InsertUp_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panel)
            {
                var qPanelWindow = new QPanelWindow()
                {
                    ActionData = Actions.InsertUp,
                    QPanelsData = this.panelsData,
                    SelectedIndex = this.panelsData.IndexOf(panel),
                    QuotationData = this.QuotationData,
                };
                qPanelWindow.ShowDialog();
            }
        }
        private void InsertDown_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panel)
            {
                var qPanelWindow = new QPanelWindow()
                {
                    ActionData = Actions.InsertDown,
                    QPanelsData = this.panelsData,
                    SelectedIndex = this.panelsData.IndexOf(panel) + 1,
                    QuotationData = this.QuotationData,
                };
                qPanelWindow.ShowDialog();
            }
        }
        private void EditPanel_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panel)
            {
                var qPanelWindow = new QPanelWindow()
                {
                    ActionData = Actions.Edit,
                    QPanelData = panel,
                    QPanelsData = this.panelsData,
                    QuotationData = this.QuotationData,
                };
                qPanelWindow.ShowDialog();
            }
        }
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                if (panelData.PanelSN > 1)
                {
                    panelData.PanelSN -= 1;
                    panelsData.Move(panelsData.IndexOf(panelData), panelsData.IndexOf(panelData) - 1);
                    foreach (QPanel panel in panelsData.Where(p => p.PanelSN == panelData.PanelSN && p.PanelID != panelData.PanelID))
                    {
                        ++panel.PanelSN;
                    }
                    string query = $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN + 1 Where PanelSN = {panelData.PanelSN} And QuotationID = {panelData.QuotationID} And PanelID != {panelData.PanelID}; " +
                                   $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN - 1 Where PanelID = {panelData.PanelID}; ";
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute(query);
                    }
                }
            }
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                if (panelData.PanelSN < panelsData.Count && panelsData.Count != 1)
                {
                    panelData.PanelSN += 1;
                    panelsData.Move(panelsData.IndexOf(panelData), panelsData.IndexOf(panelData) + 1);
                    foreach (QPanel panel in panelsData.Where(p => p.PanelSN == panelData.PanelSN && p.PanelID != panelData.PanelID))
                    {
                        --panel.PanelSN;
                    }
                    string query = $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN - 1 Where PanelSN = {panelData.PanelSN} And QuotationID = {panelData.QuotationID} And PanelID != {panelData.PanelID}; " +
                                   $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN + 1 Where PanelID = {panelData.PanelID}; ";
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.Execute(query);
                    }
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                MessageBoxResult result = CMessageBox.Show("Deleting", $"Are you sure you want to \ndelete {panelData.PanelName} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Delete From [Quotation].[QuotationsPanels] Where PanelID = {panelData.PanelID}; " +
                                       $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {panelData.PanelID}; " +
                                       $"Delete From [Quotation].[QuotationsOptionsPanels] Where PanelID = {panelData.PanelID}; " +
                                       $"Update [Quotation].[QuotationsPanels] Set PanelSN = PanelSN - 1 Where PanelSN > {panelData.PanelSN} And QuotationID = {panelData.QuotationID}; ";
                        connection.Execute(query);
                    }

                    foreach (QPanel panel in panelsData.Where(p => p.PanelSN > panelData.PanelSN))
                    {
                        --panel.PanelSN;
                    }

                    panelsData.Remove(panelData);
                    QuotationData.QuotationCost = panelsData.Sum(p => p.PanelsPrice);
                }
            }
        }
        private void Material_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                QuotationPanelItemsWindow panelItemsWindow = new QuotationPanelItemsWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                    QuotationData = this.QuotationData,
                    QuotationPanelsWindowData = null,
                };
                panelItemsWindow.ShowDialog();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                QuotationPanelNameWindow panelNameWindow = new QuotationPanelNameWindow()
                {
                    PanelData = panelData,
                    PanelsData = panelsData,
                };
                panelNameWindow.ShowDialog();
            }
        }

        #region Filters

        CollectionViewSource viewData;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(QPanel)).GetProperty("PanelSN"),
            (typeof(QPanel)).GetProperty("PanelName"),
            (typeof(QPanel)).GetProperty("PanelQty"),
            (typeof(QPanel)).GetProperty("EnclosureType"),
            (typeof(QPanel)).GetProperty("EnclosureHeight"),
            (typeof(QPanel)).GetProperty("EnclosureWidth"),
            (typeof(QPanel)).GetProperty("EnclosureDepth"),
            (typeof(QPanel)).GetProperty("EnclosureIP"),
            (typeof(QPanel)).GetProperty("PanelProfit"),
            (typeof(QPanel)).GetProperty("PanelCost"),
            (typeof(QPanel)).GetProperty("PanelPrice"),
            (typeof(QPanel)).GetProperty("PanelsCost"),
            (typeof(QPanel)).GetProperty("PanelsPrice"),
        };
        private void DataFilter(object sender, FilterEventArgs e)
        {
            try
            {
                e.Accepted = true;
                if (e.Item is QPanel record)
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

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                DownloadPanelWindow downLoadPanelWindow = new DownloadPanelWindow()
                {
                    PanelData = panelData,
                    PanelsData = PanelsData,
                    QuotationToUpdate = QuotationToUpdate,
                };
                downLoadPanelWindow.ShowDialog();
            }
        }

        private void Recalculate_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Visibility.Visible;
            Events.DoingEvent.DoEvents();

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                var items = QItemController.QuotationRecalculateItems(connection, QuotationData.QuotationID);

                if (items.Count == 0)
                {
                    LoadingControl.Visibility = Visibility.Collapsed;
                    return;
                }

                string query = "";
                foreach (QItem item in items)
                {
                    query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemCost = {item.ReferenceCost}, ItemDiscount = {item.ReferenceDiscount} where ItemID = {item.ItemID} ;";
                }

                connection.Execute(query);

                this.Window_Loaded(sender, e);
            }

            LoadingControl.Visibility = Visibility.Collapsed;
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
