using System;
using Dapper;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.StoreWindows.InvoicesWindows
{
    public partial class ItemsWindow : Window
    {
        public Actions ActionData { get; set; }
        public ItemTransaction ItemData { get; set; }
        public List<Reference> ReferencesData { get; set; }
        public ObservableCollection<ItemTransaction> ItemsData { get; set; }

        ItemTransaction newItemData;
        public ItemsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newItemData = new ItemTransaction();
            newItemData.Update(ItemData);

            PartNumbersList.ItemsSource = ReferencesData;
           
            if(ActionData == Actions.Edit)
            {
                PartNumbersList.Text = newItemData.PartNumber;
                Qty.Text = newItemData.Qty.ToString();
                Cost.Text = newItemData.Cost.ToString();
                VAT.Text = newItemData.VAT.ToString();
            }
            else
            {
                VAT.Text = App.VAT.ToString();
            }

            CostCalculator();
            PartNumbersList.Focus();
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
        private void PartNumbersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartNumbersList.SelectedItem is Reference referenceData)
            {
                Description.Text = referenceData.Description;
                Brand.Text = referenceData.Brand;
                Unit.Text = referenceData.Unit;
                CostCalculator();
            }
            else
            {
                Description.Text = null;
                Brand.Text = null;
                Cost.Text = null;
                Unit.Text = null;
                TotalCost.Text = null;
                Unit.Text = "Lot";
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isReady = true;
            string message = "Please select correct reference!";
            if (PartNumbersList.SelectedItem is Reference referenceData)
            {
                newItemData.Category = referenceData.Category;
                newItemData.Code = referenceData.Code;
                newItemData.Description = Description.Text;
                newItemData.Reference = null;
                newItemData.Unit = Unit.Text;
                newItemData.Qty = double.Parse(Qty.Text);
                newItemData.Cost = double.Parse(Cost.Text);
                newItemData.VAT = double.Parse(VAT.Text);
                newItemData.Source = "New";
                newItemData.Type = "Stock";
            }
            else
            {
                isReady = false;
            }

            if (newItemData.PartNumber == "") { isReady = false; message += $"\n*Part Number."; }
            if (newItemData.Description == "") { isReady = false; message += $"\n *Deacription."; }
            if (newItemData.Unit == "No" || newItemData.Unit == "Set" || newItemData.Unit == "Roll")
            {
                bool isInt = (double)(int)newItemData.Qty == newItemData.Qty;
                if (!isInt) { isReady = false; message += $"\n *Qty must be Integer."; }
            }

            if (isReady)
            {
                if (ActionData == Actions.Edit)
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"{DatabaseAI.UpdateRecord<ItemTransaction>()}";
                        connection.Execute(query, newItemData);

                        query = $"{DatabaseAI.UpdateRecord<ItemTransaction>($"Where ID = @TransferInvoiceID")}";
                        connection.Execute(query, newItemData);
                    }
                    ItemData.Update(newItemData);
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                    {
                        string query = $"{DatabaseAI.InsertRecord<ItemTransaction>()}";
                        newItemData.ID = (int)(decimal)connection.ExecuteScalar(query, newItemData);
                    }
                    ItemsData.Add(newItemData);
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
        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Unit.Text == "No" || Unit.Text == "Set")
                DataInput.Input.IntOnly(e, 4);
            else
                DataInput.Input.DoubleOnly(e);
        }
        private void Qty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Qty.Text) || Qty.Text == "0")
                Qty.Text = "1";

            CostCalculator();
        }
        private void CostCalculator()
        {
            double qty, cost, vat;
            if (string.IsNullOrWhiteSpace(Qty.Text)) qty = 0;
            else qty = double.Parse(Qty.Text);

            if (string.IsNullOrWhiteSpace(Cost.Text)) cost = 0;
            else cost = double.Parse(Cost.Text);

            if (string.IsNullOrWhiteSpace(VAT.Text)) vat = App.VAT;
            else vat = double.Parse(VAT.Text);

            TotalCost.Text = Math.Abs(cost * qty).ToString("N2");
            Price.Text = Math.Abs(cost * (1 + (vat / 100))).ToString("N2");
            TotalPrice.Text = Math.Abs(cost * (1 + (vat / 100)) * qty).ToString("N2");
        }

        private void Cost_LostFocus(object sender, RoutedEventArgs e)
        {
            CostCalculator();
        }

        private void VAT_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void VAT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VAT.Text) || VAT.Text == "0")
            {
                if (ActionData == Actions.Edit)
                    VAT.Text = newItemData.VAT.ToString();
                else
                    VAT.Text = App.VAT.ToString();
            }

            CostCalculator();
        }
    }
}
