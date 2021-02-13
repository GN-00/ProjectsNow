using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class CategoryDiscountWindow : Window
    {
        public int QuotationID { get; set; }
        public Category CategoryData { get; set; }
        public QuotationPanelsWindow QuotationPanelsWindowData { get; set; }
        public CategoryDiscountWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewDiscountInput.Text = CategoryData.Discount.ToString();
            DataContext = CategoryData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string ids = "";
            string query;
            int discount = int.Parse(NewDiscountInput.Text);
            List<int> PanelsIDs;
            if (discount > -1)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    CategoryData.Discount = discount;
                    query = $"Select PanelID From [Quotation].[QuotationsPanels] Where QuotationID = {QuotationID}";
                    PanelsIDs = connection.Query<int>(query).ToList();
                    
                    foreach (int ID in PanelsIDs)
                        ids += $"{ID}, ";

                    ids = ids.Substring(0, ids.Length - 2);

                    query = $"Update [Quotation].[QuotationsPanelsItems] Set " +
                            $"ItemDiscount = {CategoryData.Discount} " +
                            $"Where PanelID in({ids}) And Category = '{CategoryData.Name}'";

                    connection.Execute(query);
                }

                QuotationPanelsWindowData.Window_Loaded(sender, e);
                QuotationPanelsWindowData.QuotationData.QuotationCost = QuotationPanelsWindowData.panelsData.Sum(p =>  p.PanelsPrice);
                this.Close();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void NewDiscountInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewDiscountInput.Text))
            {
                NewDiscountInput.Text = CategoryData.Discount.ToString();
            }
            else
            {
                int discount = int.Parse(NewDiscountInput.Text);
                if (discount > 100)
                    NewDiscountInput.Text = CategoryData.Discount.ToString();
            }
        }
        private void NewDiscountInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.IntOnly(e, 3);
        }
    }
}
