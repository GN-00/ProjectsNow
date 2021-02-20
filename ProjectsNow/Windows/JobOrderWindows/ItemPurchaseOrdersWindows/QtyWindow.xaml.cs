using Dapper;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.JobOrderWindows.ItemPurchaseOrdersWindows
{
    public partial class QtyWindow : Window
    {
        public Actions ActionData { get; set; }
        public ItemPurchased ItemData { get; set; }
        public CompanyPO CompanyPOData { get; set; }
        public ObservableCollection<CompanyPOTransaction> ItemsData { get; set; }
        public CompanyPOTransaction CompanyPOTransactionData { get; set; }

        public QtyWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.New)
                ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
            else
            {
                ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
                CostInput.Text = CompanyPOTransactionData.Cost.ToString();
            }

            DataContext = ItemData;
            ItemsToPostInput.Focus();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.Edit)
            {
                double qty = int.Parse(ItemsToPostInput.Text);
                double cost = double.Parse(CostInput.Text);
                if (qty > 0)
                {
                    ItemData.InOrderQty += qty;

                    CompanyPOTransactionData.Qty = qty;
                    CompanyPOTransactionData.Cost = cost;

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"{DatabaseAI.UpdateRecord<CompanyPOTransaction>()}";
                        connection.Execute(query, CompanyPOTransactionData);
                    }

                    this.Close();
                }
            }
            else
            {
                double qty = int.Parse(ItemsToPostInput.Text);
                double cost = double.Parse(CostInput.Text);
                if (qty > 0)
                {
                    ItemData.InOrderQty += qty;

                    CompanyPOTransaction newItem = new CompanyPOTransaction()
                    {
                        PurchaseOrderID = CompanyPOData.ID,
                        Category = ItemData.Category,
                        Code = ItemData.Code,
                        Description = ItemData.Description,
                        Unit = ItemData.Unit,
                        Qty = qty,
                        Cost = cost,
                    };

                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"{DatabaseAI.InsertRecord<CompanyPOTransaction>()}";
                        newItem.ID = (int)(decimal)connection.ExecuteScalar(query, newItem);
                    }

                    if (ItemsData != null)
                        ItemsData.Add(newItem);

                    this.Close();
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ItemsToPostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.New)
            {
                if (string.IsNullOrWhiteSpace(ItemsToPostInput.Text))
                {
                    ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
                }
                else
                {
                    int qty = int.Parse(ItemsToPostInput.Text);
                    if (qty > ItemData.RemainingQty)
                        ItemsToPostInput.Text = ItemData.RemainingQty.ToString();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ItemsToPostInput.Text))
                {
                    ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
                }
                else
                {
                    int qty = int.Parse(ItemsToPostInput.Text);
                    if (qty > (ItemData.RemainingQty + ItemData.InOrderQty))
                        ItemsToPostInput.Text = (ItemData.RemainingQty + ItemData.InOrderQty).ToString();
                }
            }

        }
        private void ItemsToPostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ItemData.Unit == "No" || ItemData.Unit == "Set")
                DataInput.Input.IntOnly(e, 4);
            else
                DataInput.Input.DoubleOnly(e);
        }
        private void CostInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void CostInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CostInput.Text))
            {
                CostInput.Text = "0";
            }
        }
    }
}
