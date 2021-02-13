using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class QuotationPanelsWindow : Window
    {
        public JobOrder JobOrderData { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public ObservableCollection<JPanelDetails> JobOrderPanels { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<QPanel> quotationPanels;
        public QuotationPanelsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                quotationPanels = QPanelController.GetQuotationPanelsWaitingPurcheaseOrder(connection, JobOrderData.QuotationID);
            }

            viewData = new CollectionViewSource() { Source = quotationPanels };
            PanelsList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (quotationPanels.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (PanelsList.SelectedItem is QPanel panelData)
            {
                panelData.PurchaseOrdersNumber = PurchaseOrderNumber;
                JPanelDetails newPanelData = new JPanelDetails() { JobOrderID = JobOrderData.ID, PanelEstimatedCost = panelData.PanelCost };
                newPanelData.Update(panelData);
                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    query = DatabaseAI.UpdateRecord<QPanel>();
                    connection.Execute(query, panelData);

                    query = $"Select DrawingNo From [JobOrder].[DrawingNo] Where Year = {DateTime.Today.Year}";
                    int? drawingNo = connection.QueryFirstOrDefault<int?>(query);
                    if (drawingNo == null) newPanelData.DrawingNo = 1;
                    else newPanelData.DrawingNo = ++drawingNo;

                    query = DatabaseAI.InsertRecordWithID<JPanelDetails>();
                    connection.Execute(query, newPanelData);

                    connection.InsertSelect<JItem, QItem>($"Where PanelID = {newPanelData.PanelID} ");
                }
                quotationPanels.Remove(panelData);
                JobOrderPanels.Add(newPanelData);
            }
        }
    }
}
