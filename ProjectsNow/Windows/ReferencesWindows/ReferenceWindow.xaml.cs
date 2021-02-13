using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using ProjectsNow.DataInput;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class ReferenceWindow : Window
    {
        public User UserData { get; set; }
        public Actions ActionData { get; set; }
        public Reference ReferenceData { get; set; }
        public ObservableCollection<Reference> ReferencesData { get; set; }

        Reference referenceData;
        public ReferenceWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            referenceData = new Reference();
            referenceData.Update(ReferenceData);

            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                Article1.ItemsSource = ReferenceController.GetArticle1(connection);
                Article2.ItemsSource = ReferenceController.GetArticle2(connection);
            }

            DataContext = new { referenceData };
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isReady = true;
            string message = "Please fill:";

            if (string.IsNullOrWhiteSpace(referenceData.Category)) { isReady = false; message += $"\nCategory."; }
            if (string.IsNullOrWhiteSpace(referenceData.Code)) { isReady = false; message += $"\nCode."; }
            if (string.IsNullOrWhiteSpace(referenceData.Description)) { isReady = false; message += $"\nDescription."; }
            if (string.IsNullOrWhiteSpace(referenceData.Brand)) { isReady = false; message += $"\nBrand."; }
            if (string.IsNullOrWhiteSpace(referenceData.Discount.ToString())) { isReady = false; message += $"\nDiscount."; }
            if (string.IsNullOrWhiteSpace(referenceData.Unit)) { isReady = false; message += $"\nUnit."; }
            if (string.IsNullOrWhiteSpace(referenceData.Cost.ToString())) { isReady = false; message += $"\nCost."; }

            if (isReady)
            {
                if (ActionData == Actions.New)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.InsertRecord<Reference>();
                        referenceData.ReferenceID = (int)(decimal)connection.ExecuteScalar(query, referenceData);
                    }

                    List<Reference> references = ReferencesData.ToList();
                    references.Add(referenceData);
                    references = references.OrderBy(r => r.PartNumber).ToList();
                    int index = references.IndexOf(referenceData);
                    ReferencesData.Insert(index, referenceData);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = DatabaseAI.UpdateRecord<Reference>();
                        connection.Execute(query, referenceData);
                    }

                    ReferenceData.Update(referenceData);
                }

                this.Close();
            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cost_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.DoubleOnly(e);
        }

        private void Discount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.DoubleOnly(e);
        }

        private void Discount_LostFocus(object sender, RoutedEventArgs e)
        {
            double.TryParse(((TextBox)sender).Text, out double discount);
            if (discount > 100)
            {
                ((TextBox)sender).Text = "0";
                referenceData.Discount = 0;
            }
        }
    }
}
