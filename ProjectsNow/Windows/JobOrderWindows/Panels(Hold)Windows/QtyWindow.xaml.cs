using System;
using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.JobOrderWindows.Panels_Hold_Windows
{
    public partial class QtyWindow : Window
    {
        public JPanel PanelData { get; set; }
        public ObservableCollection<TPanel> PanelsTransaction { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
            DataContext = PanelData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            string query;
            int qty = int.Parse(PanelToHoldInput.Text);
            if (qty > 0)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    PanelData.HoldQty += qty;
                    query = $"Insert Into [JobOrder].[PanelsTransactions] " +
                             $"(PanelID, Reference, Qty, Date, Action) " +
                             $"Values " +
                             $"(@PanelID, @Reference, @Qty, @Date, 'Hold') Select @@IDENTITY";

                    TPanel newPanel = new TPanel()
                    {
                        JobOrderID = PanelData.JobOrderID,
                        PanelID = PanelData.PanelID,
                        PanelSN = PanelData.PanelSN,
                        PanelName = PanelData.PanelName,
                        EnclosureType = PanelData.EnclosureType,
                        Qty = qty,
                        Date = DateTime.Today,
                    };

                    newPanel.TransactionID = (int)(decimal)connection.ExecuteScalar(query, newPanel);
                    PanelsTransaction.Add(newPanel);
                }

                this.Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PanelToHoldInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PanelToHoldInput.Text))
            {
                PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
            }
            else
            {
                int qty = int.Parse(PanelToHoldInput.Text);
                if (qty > PanelData.ReadyToHoldQty)
                    PanelToHoldInput.Text = PanelData.ReadyToHoldQty.ToString();
            }
        }
        private void PanelToHoldInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 4);
        }
    }
}
