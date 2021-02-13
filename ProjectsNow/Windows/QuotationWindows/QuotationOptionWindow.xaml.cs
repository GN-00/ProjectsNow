using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class QuotationOptionWindow : Window
    {
        public int QuotationID { get; set; }
        public Actions ActionData { get; set; }
        public QuotationOption QuotationOptionData { get; set; }
        public ObservableCollection<QuotationOption> QuotationOptionsData { get; set; }

        QuotationOption newQuotationOption;
        public QuotationOptionWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.New) newQuotationOption = new QuotationOption();
            else newQuotationOption = QuotationOptionData;

            DataContext = new { newQuotationOption };
        }
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
            bool isReady = true;
            string message = $"Please Enter:";
            if (string.IsNullOrWhiteSpace(OptionName.Text))
            {
                isReady = false;
                message += $"\n Option Name!";
            }

            if (isReady)
            {
                if (ActionData == Actions.New)
                {
                    newQuotationOption.QuotationID = QuotationID;
                    if (QuotationOptionsData == null || QuotationOptionsData.Count == 0)
                        newQuotationOption.Number = 1;
                    else
                        newQuotationOption.Number = QuotationOptionsData.Max(o => o.Number) + 1;
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.InsertRecord<QuotationOption>();
                        newQuotationOption.ID = (int)(decimal)connection.ExecuteScalar(query, newQuotationOption);
                    }

                    QuotationOptionsData.Add(newQuotationOption);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.UpdateRecord<QuotationOption>();
                        connection.Execute(query, newQuotationOption);
                    }

                    QuotationOptionData.Update(newQuotationOption);
                }

                Close();
            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Information);
            }
        }
    }
}
