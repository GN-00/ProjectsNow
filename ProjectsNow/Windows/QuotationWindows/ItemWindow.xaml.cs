using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class ItemWindow : Window
    {
        public User UserData { get; set; }
        public QItem ItemData { get; set; }
        public int SelectedIndex { get; set; }
        public Actions ActionData { get; set; }
        public Reference ReferenceData { get; set; }
        public List<Reference> ReferencesListData { get; set; }
        public QuotationPanelItemsWindow PanelItemsWindowData { get; set; }


        QItem newItemData;
        public ItemWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                if (ReferencesListData == null)
                    ReferencesListData = ReferenceController.GetReferences(connection);

                Article1.ItemsSource = ReferenceController.GetArticle1(connection);
                Article2.ItemsSource = ReferenceController.GetArticle2(connection);
            }

            PartNumbersList.ItemsSource = ReferencesListData;

            if (ActionData == Actions.Edit)
            {
                newItemData = new QItem();
                newItemData.Update(ItemData);
                PartNumbersList.SelectedItem = ReferencesListData.Single(item => item.PartNumber == newItemData.PartNumber);
                Qty.Text = newItemData.ItemQty.ToString();
                Remarks.Text = newItemData.Remarks;
            }
            else
            {
                newItemData = new QItem();
                Qty.Text = "1";
            }

            Table.Text = PanelItemsWindowData.TableData.ToString();
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
            ReferenceData = PartNumbersList.SelectedItem as Reference;
            if (ReferenceData == null)
            {
                Description.Text = null;
                if (newItemData.Article1 == null) Article1.Text = null;
                if (newItemData.Article2 == null) Article2.Text = null;
                Brand.Text = null;
                Remarks.Text = null;
                Discount.Text = null;
                Cost.Text = null;
                Unit.Text = null;
                TotalCost.Text = null;
                TotalPrice.Text = null;
                Unit.Text = "Lot";
            }
            else
            {
                Description.Text = ReferenceData.Description;
                if (newItemData.Article1 == null) Article1.Text = ReferenceData.Article1;
                else Article1.Text = newItemData.Article1;
                if (newItemData.Article2 == null) Article2.Text = ReferenceData.Article2;
                else Article2.Text = newItemData.Article2;
                Brand.Text = ReferenceData.Brand;
                Remarks.Text = ReferenceData.Remarks;
                Discount.Text = ReferenceData.Discount.ToString("N2");
                Cost.Text = ReferenceData.Cost.ToString("N2");
                Unit.Text = ReferenceData.Unit;
                CostCalculator();
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
                newItemData.Article1 = Article1.Text;
                if (string.IsNullOrWhiteSpace(Article2.Text)) newItemData.Article2 = null;
                else newItemData.Article2 = Article2.Text;
                newItemData.Brand = Brand.Text;
                newItemData.ItemTable = Table.Text;
                newItemData.ItemQty = double.Parse(Qty.Text);
                newItemData.ItemDiscount = double.Parse(Discount.Text);
                newItemData.ItemCost = double.Parse(Cost.Text);
                newItemData.ItemType = ItemTypes.Standard.ToString();
                newItemData.Remarks = Remarks.Text;
                newItemData.Unit = Unit.Text;
            }
            else
            {
                isReady = false;
            }

            if (newItemData.PartNumber == "") { isReady = false; message += $"\n*Part Number."; }
            if (newItemData.Description == "") { isReady = false; message += $"\n *Deacription."; }
            if (string.IsNullOrWhiteSpace(newItemData.Article1)) { isReady = false; message += $"\n *Article."; }
            if (newItemData.Unit == "No" || newItemData.Unit == "Set" || newItemData.Unit == "Roll")
            {
                bool isInt = (double)(int)newItemData.ItemQty == newItemData.ItemQty;
                if (!isInt) { isReady = false; message += $"\n *Qty must be Integer."; }
            }

            if (isReady)
            {
                string query;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    if (ActionData == Actions.Edit)
                    {
                        query = DatabaseAI.UpdateRecord<QItem>();
                        connection.Execute(query, newItemData);
                        ItemData.Update(newItemData);
                    }
                    else
                    {
                        newItemData.ItemSort = SelectedIndex;
                        newItemData.PanelID = PanelItemsWindowData.PanelData.PanelID;

                        if (ActionData == Actions.New)
                        {
                            PanelItemsWindowData.ItemsData.Add(newItemData);
                        }
                        else
                        {
                            connection.Execute($"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = ItemSort + 1 Where ItemSort >= {newItemData.ItemSort} and PanelID ={newItemData.PanelID} And ItemTable = '{PanelItemsWindowData.TableData}'");
                            foreach (QItem item in PanelItemsWindowData.ItemsData.Where(item => item.ItemSort >= newItemData.ItemSort))
                            {
                                item.ItemSort++;
                            }
                            PanelItemsWindowData.ItemsData.Insert(SelectedIndex, newItemData);
                        }

                        query = DatabaseAI.InsertRecord<QItem>();
                        newItemData.ItemID = (int)(decimal)connection.ExecuteScalar(query, newItemData);

                    }
                }

                PanelItemsWindowData.PanelData.PanelCost =
                        PanelItemsWindowData.ItemsDetails.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100)) +
                        PanelItemsWindowData.ItemsEnclosure.Sum<QItem>(item => item.ItemCost * item.ItemQty * (1 - item.ItemDiscount / 100));

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
        private void Table_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.ArrowsOnly(e);
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
        private void Discount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataInput.Input.DoubleOnly(e);
        }
        private void Discount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Discount.Text)) Discount.Text = "0";
            if (double.Parse(Discount.Text) > 100) Discount.Text = "0";
            CostCalculator();
        }
        private void CostCalculator()
        {
            double qty, discount, cost;
            if (string.IsNullOrWhiteSpace(Qty.Text)) qty = 0;
            else qty = double.Parse(Qty.Text);

            if (string.IsNullOrWhiteSpace(Discount.Text)) discount = 1;
            else discount = (1 - double.Parse(Discount.Text) / 100);

            if (string.IsNullOrWhiteSpace(Cost.Text)) cost = 0;
            else cost = double.Parse(Cost.Text);

            TotalCost.Text = (cost * qty).ToString("N2");
            TotalPrice.Text = (cost * qty * discount).ToString("N2");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PanelItemsWindowData.ReferencesListData == null)
                PanelItemsWindowData.ReferencesListData = this.ReferencesListData;
        }

        private void NewItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(ActionData == Actions.Edit)
            {
                CMessageBox.Show("Items", "Can't change this Item type!!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
            else
            {
                NewItemWindow newItemWindow = new NewItemWindow()
                {
                    UserData = UserData,
                    ActionData = ActionData,
                    SelectedIndex = SelectedIndex,
                    PanelItemsWindowData = PanelItemsWindowData,
                    ReferencesListData = ReferencesListData,
                };
                this.Close();
                newItemWindow.ShowDialog();
            }
        }
    }
}
