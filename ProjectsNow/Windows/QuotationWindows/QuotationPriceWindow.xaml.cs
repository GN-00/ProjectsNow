using Dapper;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationPriceWindow : Window
    {
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }
        public bool ResetUserQuotationID { get; set; }


        Quotation newQuotationData;
        public QuotationPriceWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newQuotationData = new Quotation();
            newQuotationData.Update(QuotationData);
            DataContext = newQuotationData;

            if (UserData.QuotationsDiscount == true && QuotationData.QuotationStatus == Statuses.Running.ToString())
            {
                DiscountInput.IsReadOnly = false;
                DiscountInput.Focusable = true;
                DiscountInput.IsHitTestVisible = true;

                DiscountValueInput.IsReadOnly = false;
                DiscountValueInput.Focusable = true;
                DiscountValueInput.IsHitTestVisible = true;

                Save.Visibility = Visibility.Visible;
            }
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query;
                query = DatabaseAI.UpdateRecord<Quotation>();
                connection.Execute(query, newQuotationData);

                if (ResetUserQuotationID)
                {
                    UserData.QuotationID = null;
                    UserController.UpdateQuotationID(connection, UserData);
                }
            }

            QuotationData.Update(newQuotationData);
            this.Close();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.QuotationID = null;
                UserController.UpdateQuotationID(connection, UserData);
            }
        }

        private void DiscountInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DiscountInput.Text))
            {
                newQuotationData.Discount = double.Parse(((TextBox)sender).Text);
                if (newQuotationData.Discount > UserData.QuotationsDiscountValue)
                {
                    CMessageBox.Show("Discount", $"Max discount can be apply is {UserData.QuotationsDiscountValue}!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                    newQuotationData.Discount = 0;
                    DiscountInput.Text = (0).ToString("N3");
                    DiscountValueInput.Text = newQuotationData.QuotationDiscountValue.ToString("N3");
                }
                else
                {
                    DiscountValueInput.Text = newQuotationData.QuotationDiscountValue.ToString("N3");
                }
            }
            else
            {
                newQuotationData.Discount = 0;
                DiscountInput.Text = (0).ToString("N3");
                DiscountValueInput.Text = newQuotationData.QuotationDiscountValue.ToString("N3");
            }
        }

        private void DiscountValueInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DiscountValueInput.Text))
            {
                double value = double.Parse(DiscountValueInput.Text);
                if (value != newQuotationData.QuotationDiscountValue)
                {
                    newQuotationData.Discount = (value / newQuotationData.QuotationCost) * 100;
                    if (newQuotationData.Discount > UserData.QuotationsDiscountValue)
                    {
                        CMessageBox.Show("Discount", $"Max discount can be apply is {UserData.QuotationsDiscountValue}!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                        newQuotationData.Discount = 0;
                        DiscountInput.Text = (0).ToString("N3");
                        DiscountValueInput.Text = (0).ToString("N3");
                    }
                    else
                    {
                        DiscountValueInput.Text = newQuotationData.QuotationDiscountValue.ToString("N3");
                    }
                }
            }
            else
            {
                newQuotationData.Discount = 0;
                DiscountInput.Text = (0).ToString("N3");
                DiscountValueInput.Text = (0).ToString("N3");
            }
        }
    }
}
