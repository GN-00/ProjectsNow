using Dapper;
using System.Linq;
using System.Windows;
using System.Reflection;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Closing_Windows
{
    public partial class PanelsWindow : Window
    {
        public User UserData { get; set; }
        public JobOrder JobOrderData { get; set; }
        public ObservableCollection<JPanel> PanelsData { get; set; }


        ObservableCollection<TPanel> panelsTransaction;

        readonly string filter1 = Statuses.Production.ToString();
        public PanelsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select * From [JobOrder].[Panels(Closed)] Where JobOrderID  = {JobOrderData.ID} ";
                panelsTransaction = new ObservableCollection<TPanel>(connection.Query<TPanel>(query));
            }
            viewData1 = new CollectionViewSource() { Source = PanelsData };
            viewData2 = new CollectionViewSource() { Source = panelsTransaction };

            viewData1.Filter += Data1Filter;
            List1.ItemsSource = viewData1.View;
            List2.ItemsSource = viewData2.View;

            viewData1.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Collection1Changed);
            viewData2.View.CollectionChanged += new NotifyCollectionChangedEventHandler(Collection2Changed);

            if (viewData1.View.Cast<object>().Count() == 0)
                Collection1Changed(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (viewData2.View.Cast<object>().Count() == 0)
                Collection2Changed(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            DataContext = new { JobOrderData, UserData };
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

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (List1.SelectedItem is JPanel panelData)
            {
                var panel_Closing_Window = new ClosingWindow()
                {
                    UserData = this.UserData,
                    PanelData = panelData,
                    JobOrderData = this.JobOrderData,
                    PanelsTransaction = panelsTransaction
                };
                panel_Closing_Window.ShowDialog();
                viewData1.View.Refresh();
            }
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (List2.SelectedItem is TPanel panelData)
            {
                var checkingPanel = PanelsData.FirstOrDefault(p => p.PanelID == panelData.PanelID);
                if (checkingPanel.Status == Statuses.Hold.ToString())
                {
                    CMessageBox.Show("Panels", "The panel is hold!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    return;
                }

                if (checkingPanel.ReadyToInvoicedQty >= panelData.Qty)
                {
                    checkingPanel.ClosedQty -= panelData.Qty;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"Delete From [JobOrder].[PanelsTransactions] Where ID  = {panelData.TransactionID};" +
                                       $"Delete From [Store].[Transactions] Where PanelTransactionID = {panelData.TransactionID};";

                        connection.Execute(query);
                    }
                    panelsTransaction.Remove(panelData);
                }
                else
                {
                    CMessageBox.Show("Panels", "The panel Invoiced!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }

        private void Collection1Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            else
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";
        }
        private void Collection2Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            else
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
        }

        private void List1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = List1.SelectedIndex;
            if (selectedIndex == -1)
                Navigation1.Text = $"Panels: {viewData1.View.Cast<object>().Count()}";
            else
                Navigation1.Text = $"Panel: {selectedIndex + 1} / {viewData1.View.Cast<object>().Count()}";
        }
        private void List2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = List2.SelectedIndex;
            if (selectedIndex == -1)
                Navigation2.Text = $"Panels: {viewData2.View.Cast<object>().Count()}";
            else
                Navigation2.Text = $"Panel: {selectedIndex + 1} / {viewData2.View.Cast<object>().Count()}";
        }

        #region Filters
        CollectionViewSource viewData1;
        CollectionViewSource viewData2;
        private readonly List<PropertyInfo> filterProperties = new List<PropertyInfo>()
        {
            (typeof(JPanel)).GetProperty("Status"),
        };
        private void Data1Filter(object sender, FilterEventArgs e)
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
                        value = $"{record.GetType().GetProperty(columnName).GetValue(record)}";

                        if (value != filter1)
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

    }
}
