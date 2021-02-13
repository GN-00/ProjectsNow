using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class CategoryDiscountWindow : Window
    {
        public Category CategoryData { get; set; }
        public ReferencesWindow ReferencesWindowData { get; set; }

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
            string query;
            int discount = int.Parse(NewDiscountInput.Text);
            if (discount > -1)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    CategoryData.Discount = discount;
                    query = $"Update [Reference].[References] Set " +
                            $"Discount = {CategoryData.Discount} " +
                            $"Where Category = '{CategoryData.Name}'";

                    connection.Execute(query);
                }

                ReferencesWindowData.Window_Loaded(sender, e);
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
