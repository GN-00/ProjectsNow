using System;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;

namespace ProjectsNow.Windows.JobOrderWindows
{
    public partial class Items_Standard_Window : Window
    {
        public int PanelID { get; set; }
        public MItem ItemData { get; set; }
        public Actions ActionData { get; set; }
        public Modification ModificationData { get; set; }
        public List<MItem> JobOrderItemsData { get; set; }
        public List<Reference> ReferencesData { get; set; }
        public ObservableCollection<MItem> ItemsData { get; set; }

        MItem newItemData;
        public Items_Standard_Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActionData == Actions.Edit)
            {
                newItemData = new MItem();
                newItemData.Update(ItemData);

                if (newItemData.Source == "Additional")
                    PartNumbersList.ItemsSource = ReferencesData;
                else
                    PartNumbersList.ItemsSource = JobOrderItemsData.Where(i => i.PanelID == PanelID && i.ModificationID != ModificationData.ID);

                PartNumbersList.Text = newItemData.PartNumber;
                Qty.Text = newItemData.ItemQty.ToString();
            }
            else
            {
                if (ActionData == Actions.New)
                    PartNumbersList.ItemsSource = ReferencesData;
                else
                    PartNumbersList.ItemsSource = JobOrderItemsData.Where(i => i.PanelID == PanelID && i.ItemQty > 0 && i.ModificationID != ModificationData.ID);

                newItemData = new MItem();
                if (ActionData == Actions.Remove) Qty.Text = "-1";
                else Qty.Text = "1";
            }

            if (ActionData == Actions.New) { Image.Source = new BitmapImage(new Uri("/Images/Icons/Add.png", UriKind.Relative)); NewItem.Visibility = Visibility.Visible; }
            else if (ActionData == Actions.Remove) { Image.Source = new BitmapImage(new Uri("/Images/Icons/Undo.png", UriKind.Relative)); }
            else if (ActionData == Actions.Edit) { Image.Source = new BitmapImage(new Uri("/Images/Icons/Edit.png", UriKind.Relative)); }
            
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
                Discount.Text = referenceData.Discount.ToString("N2");
                Cost.Text = referenceData.Cost.ToString("N2");
                Unit.Text = referenceData.Unit;
                QtyLabel.Text = null;
                CostCalculator();
            }
            else if (PartNumbersList.SelectedItem is MItem itemData)
            {
                Description.Text = itemData.Description;
                Brand.Text = itemData.Brand;
                Discount.Text = itemData.ItemDiscount.ToString("N2");
                Cost.Text = itemData.ItemCost.ToString("N2");
                Unit.Text = itemData.Unit;
                Table.Text = itemData.ItemTable;

                if (ActionData == Actions.Edit)
                    QtyLabel.Text = $"({itemData.ItemQty + Math.Abs(newItemData.ItemQty)})"; 
                else
                    QtyLabel.Text = $"({itemData.ItemQty})";

                CostCalculator();
            }
            else
            {
                Description.Text = null;
                Brand.Text = null;
                Discount.Text = null;
                Cost.Text = null;
                Unit.Text = null;
                QtyLabel.Text = null;
                TotalCost.Text = null;
                TotalPrice.Text = null;
                Unit.Text = "Lot";
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MItem updateItem;
            bool isReady = true;
            string message = "Please select correct reference!";

            if (PartNumbersList.SelectedItem is Reference referenceData)
            {
                newItemData.Category = referenceData.Category;
                newItemData.Code = referenceData.Code;
                newItemData.Description = Description.Text;
                newItemData.Brand = Brand.Text;
                updateItem = JobOrderItemsData.FirstOrDefault(i => i.PartNumber.Equals(newItemData.PartNumber));
                if (updateItem != null)
                {
                    newItemData.ItemSort = updateItem.ItemSort;
                    newItemData.ItemTable = updateItem.ItemTable;
                    newItemData.ItemType = updateItem.ItemType;
                }
                else
                {
                    newItemData.ItemSort = -1;
                    newItemData.ItemTable = Table.Text;
                    newItemData.ItemType = ItemTypes.Standard.ToString();
                }
                newItemData.ItemQty = double.Parse(Qty.Text);
                newItemData.ItemDiscount = double.Parse(Discount.Text);
                newItemData.ItemCost = double.Parse(Cost.Text);
                newItemData.Unit = Unit.Text;
                newItemData.Date = ModificationData.Date;
                newItemData.ModificationID = ModificationData.ID;
                if (ActionData == Actions.New) newItemData.Source = "Additional";
                else if (ActionData == Actions.Remove) { newItemData.Source = "Removed"; newItemData.ItemSort = ItemData.ItemSort; };
            }
            else if (PartNumbersList.SelectedItem is MItem itemData)
            {
                newItemData.Category = itemData.Category;
                newItemData.Code = itemData.Code;
                newItemData.Description = Description.Text;
                newItemData.Brand = Brand.Text;
                newItemData.ItemTable = itemData.ItemTable;
                newItemData.ItemQty = double.Parse(Qty.Text);
                newItemData.ItemDiscount = double.Parse(Discount.Text);
                newItemData.ItemCost = double.Parse(Cost.Text);
                newItemData.ItemType = itemData.ItemType;
                newItemData.Unit = Unit.Text;
                newItemData.Date = ModificationData.Date;
                newItemData.ModificationID = ModificationData.ID;
                if (ActionData == Actions.New) newItemData.Source = "Additional";
                else if (ActionData == Actions.Remove) 
                { 
                    newItemData.Source = "Removed";
                    newItemData.ItemSort = itemData.ItemSort;
                };
            }
            else
            {
                isReady = false;
            }

            if (newItemData.PartNumber == "") { isReady = false; message += $"\n*Part Number."; }
            if (newItemData.Description == "") { isReady = false; message += $"\n *Deacription."; }
            if (newItemData.Unit == "No" || newItemData.Unit == "Set" || newItemData.Unit == "Roll")
            {
                bool isInt = (double)(int)newItemData.ItemQty == newItemData.ItemQty;
                if (!isInt) { isReady = false; message += $"\n *Qty must be Integer."; }
            }

            if (isReady)
            {
                if (ActionData == Actions.Edit)
                {
                    updateItem = JobOrderItemsData.FirstOrDefault<MItem>(i => i.PartNumber == newItemData.PartNumber);
                    if (updateItem != null)
                        updateItem.ItemQty += ItemData.ItemQty * -1;

                    ItemData.Update(newItemData);
                }
                else
                {
                    newItemData.PanelID = PanelID;
                    ItemsData.Add(newItemData);
                }

                var newJobOrderItem = new MItem();
                newJobOrderItem.Update(newItemData);

                updateItem = JobOrderItemsData.FirstOrDefault(i => i.PartNumber == newItemData.PartNumber && i.PanelID == PanelID);
                if (updateItem != null)
                    updateItem.ItemQty += newItemData.ItemQty;
                else
                    JobOrderItemsData.Add(newJobOrderItem);

                this.Close();
            }
            else
            {
                CMessageBox.Show("Error", message, CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow_Click(sender, e);
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
                Qty.Text = "-1";

            if (ActionData == Actions.Remove || newItemData.Source == "Removed")
            {
                if (PartNumbersList.SelectedItem is MItem itemData)
                {
                    double qty = double.Parse(Qty.Text);
                    if(ActionData == Actions.Edit)
                    {
                        if (Math.Abs(qty) > (itemData.ItemQty + Math.Abs(newItemData.ItemQty)))
                            qty = (itemData.ItemQty + +Math.Abs(newItemData.ItemQty)) * -1;
                    }
                    else
                    {
                        if (Math.Abs(qty) > itemData.ItemQty)
                            qty = itemData.ItemQty * -1;
                    }

                    if (qty > 0)
                        Qty.Text = (qty * -1).ToString();
                    else
                        Qty.Text = (qty).ToString();
                }
            }    

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

            TotalCost.Text = Math.Abs(cost * qty).ToString("N2");
            TotalPrice.Text = Math.Abs(cost * qty * discount).ToString("N2");
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            Items_New_Window window = new Items_New_Window()
            {
                PanelID = PanelID,
                ItemData = ItemData,
                ActionData = ActionData,
                ModificationData = ModificationData,
                ItemsData = ItemsData,
                JobOrderItemsData = JobOrderItemsData,
            };
            this.Close();
            window.ShowDialog();
        }
    }
}
