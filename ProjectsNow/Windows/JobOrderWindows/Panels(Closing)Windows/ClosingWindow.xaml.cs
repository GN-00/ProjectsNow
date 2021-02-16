using Dapper;
using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Closing_Windows
{
    public partial class ClosingWindow : Window
    {
        public User UserData { get; set; }
        public JPanel PanelData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<TPanel> PanelsTransaction { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<CItem> itemsData;
        public ClosingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Items(Posting)] Where PanelID = {PanelData.PanelID}";
                itemsData = new ObservableCollection<CItem>(connection.Query<CItem>(query));
            }
            viewData = new CollectionViewSource() { Source = itemsData };
            ItemsList.ItemsSource = viewData.View;

            PanelToCloseInput.Text = (PanelData.ReadyToCloseQty).ToString();
            ItemsList_SelectedCellsChanged(sender, new SelectedCellsChangedEventArgs(new List<DataGridCellInfo>(), new List<DataGridCellInfo>()));
            DataContext = new { JobOrderData, PanelData, UserData };
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

        private void CheckQty_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToCloseInput.Text))
                return;

            var remainingPanel = PanelData.ReadyToCloseQty;
            if (remainingPanel != 0)
            {
                var panelToClose = int.Parse(PanelToCloseInput.Text);

                if (panelToClose > remainingPanel)
                {
                    panelToClose = remainingPanel;
                    PanelToCloseInput.Text = panelToClose.ToString();
                }

                foreach (CItem item in itemsData)
                {
                    item.PanelToPostQty = double.Parse(PanelToCloseInput.Text);
                }
            }
            else
            {
                CMessageBox.Show("Panels", "No remaining panels!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToCloseInput.Text))
                return;

            var remainingPanel = PanelData.PanelQty - PanelData.ClosedQty;
            if (remainingPanel != 0)
            {
                var panelToClose = int.Parse(PanelToCloseInput.Text);

                if (panelToClose > remainingPanel)
                {
                    panelToClose = remainingPanel;
                    PanelToCloseInput.Text = panelToClose.ToString();
                }

                foreach (CItem item in itemsData)
                {
                    item.PanelToPostQty = double.Parse(PanelToCloseInput.Text);
                }

                bool isReadyToPost = true;
                foreach (CItem item in itemsData)
                {
                    if (item.StockQty < item.ItemToPostQty)
                    {
                        isReadyToPost = false;
                        break;
                    }
                }

                if (isReadyToPost)
                {
                    PanelData.ClosedQty += panelToClose;
                    var newClosedPanel = new TPanel()
                    {
                        JobOrderID = PanelData.JobOrderID,
                        PanelID = PanelData.PanelID,
                        PanelName = PanelData.PanelName,
                        PanelSN = PanelData.PanelSN,
                        EnclosureType = PanelData.EnclosureType,
                        Qty = panelToClose,
                        Date = DateTime.Now,
                    };
                    PanelsTransaction.Add(newClosedPanel);

                    string query = "";
                    IEnumerable<ItemTransaction> stockItems;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        query = $"Select * From [JobOrder].[Items(StockDetails)] Where JobOrderID = {JobOrderData.ID}";
                        stockItems = connection.Query<ItemTransaction>(query);

                        query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                                $"(PanelID, Qty, Date, Action) " +
                                $"Values " +
                                $"(@PanelID, @Qty, @Date, 'Closed') Select @@IDENTITY";
                        newClosedPanel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, newClosedPanel);
                    }

                    query = "";
                    foreach (CItem item in itemsData)
                    {
                        double needQty = (double)item.ItemToPostQty;
                        item.PanelToPostQty = 0;
                        item.StockQty -= needQty;
                        item.UsedQty += needQty;

                        foreach (ItemTransaction stockItem in stockItems.Where(i => i.PartNumber == item.PartNumber))
                        {
                            if (needQty == 0)
                                break;

                            if (stockItem.Qty >= needQty)
                            {
                                query += $"Insert Into [Store].[Transactions] " +
                                         $"(JobOrderID, PanelID, PanelTransactionID, " +
                                         (stockItem.Category == null ? null : " Category, ") +
                                         $"Code, Description, Reference, InvoiceID, Unit, Qty, Cost, Date, Type, Source) " +
                                         $"Values " +
                                         $"({JobOrderData.ID}, {PanelData.PanelID}, {newClosedPanel.TransactionID}, " +
                                         (stockItem.Category == null ? null : $"'{stockItem.Category}', ") +
                                         $"'{stockItem.Code}', " +
                                         $"'{stockItem.Description}', {stockItem.ID}, {stockItem.InvoiceID}, '{stockItem.Unit}', {needQty}, " +
                                         $"{stockItem.Cost}, '{DateTime.Now:yyyy-MM-dd}', 'Used', '{stockItem.Source}'); ";

                                needQty = 0;
                                break;
                            }
                            else
                            {
                                query += $"Insert Into [Store].[Transactions] " +
                                         $"(JobOrderID, PanelID, PanelTransactionID, " +
                                         (stockItem.Category == null ? null : " Category, ") +
                                         $"Code, Description, Reference, InvoiceID, Unit, Qty, Cost, Date, Type, Source) " +
                                         $"Values " +
                                         $"({JobOrderData.ID}, {PanelData.PanelID}, {newClosedPanel.TransactionID}, " +
                                         (stockItem.Category == null ? null : $"'{stockItem.Category}', ") +
                                         $"'{stockItem.Code}', " +
                                         $"'{stockItem.Description}', {stockItem.ID}, {stockItem.InvoiceID}, '{stockItem.Unit}', {stockItem.Qty}, " +
                                         $"{stockItem.Cost}, '{DateTime.Now:yyyy-MM-dd}', 'Used', '{stockItem.Source}'); ";

                                needQty -= stockItem.Qty;
                            }
                        }
                    }

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        connection.ExecuteScalar(query);

                        if (PanelData.PanelQty == PanelData.ClosedQty)
                        {
                            PanelData.Status = Statuses.Closed.ToString();
                            connection.Execute($"Update [JobOrder].[Panels] Set Status ='{Statuses.Closed}' Where PanelID = {PanelData.PanelID}");
                        }
                    }
                }
                else
                {
                    CMessageBox.Show("Out Of Stock", "Can't post this panel due to shortage in job order stock!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
            else
            {
                CMessageBox.Show("Panels", "No remaining panels!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }

        private void ItemsList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = ItemsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanels.Text = $"Panels: {viewData.View.Cast<object>().Count()}";
            else
                NavigationPanels.Text = $"Panel: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
    }
}
