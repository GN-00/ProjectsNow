using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using ProjectsNow.Database;
using System.Windows.Input;
using ProjectsNow.DataInput;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class SFWindow : Window
    {
        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> ItemsDetails { get; set; }
        public ObservableCollection<QItem> ItemsEnclosure { get; set; }
        public SFWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.ArrowsOnly(e);
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Input.IntOnly(e, 2);
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = "0";
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            int qty;
            string depth = PPDepth.Text;
            List<QItem> itemsData = new List<QItem>();
            List<QItem> references = new List<QItem>();
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query;
                if (SF60.Text != "0")
                {
                    int width = 60;
                    qty = int.Parse(SF60.Text);
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF80.Text != "0")
                {
                    int width = 80;
                    qty = int.Parse(SF80.Text);
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF100.Text != "0")
                {
                    int width = 100;
                    qty = int.Parse(SF100.Text);
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }
                if (SF120.Text != "0")
                {
                    int width = 120;
                    qty = int.Parse(SF120.Text);
                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Upright' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Base' And Width ='{width}' And Depth ='{depth}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Door' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Rear' And Width ='{width}'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }

                    query = $"Select * From [Reference].[SFView] " +
                            $"Where GroupName = 'Accessories'";
                    references = connection.Query<QItem>(query).ToList();
                    foreach (QItem reference in references)
                    { reference.ItemQty *= qty; itemsData.Add(reference); }
                }

                query = $"Select * From [Reference].[SFView] " +
                        $"Where GroupName = 'Side' And Depth ='{depth}'";
                references = connection.Query<QItem>(query).ToList();
                foreach (QItem reference in references)
                    itemsData.Add(reference);

                var smartItemsData = itemsData.GroupBy(g => new { g.Article1, g.Category, g.Code, g.Description, g.Unit, g.Brand, g.ItemType, g.ItemCost, g.ItemDiscount, g.ItemSort })
                    .Select(g => new QItem()
                    {
                        Article1 = g.Key.Article1,
                        Category = g.Key.Category,
                        Code = g.Key.Code,
                        Description = g.Key.Description,
                        Unit = g.Key.Unit,
                        Brand = g.Key.Brand,
                        ItemType = g.Key.ItemType,
                        ItemQty = g.Sum(q => q.ItemQty),
                        ItemCost = g.Key.ItemCost,
                        ItemDiscount = g.Key.ItemDiscount,
                        ItemSort = g.Key.ItemSort,
                    })
                    .ToList();
                itemsData = smartItemsData.OrderBy(i => i.ItemSort).ThenBy(i => i.Code).ToList();
                int newDetailsCount = itemsData.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                int newEnclosureCount = itemsData.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                List<QItem> smartEnclosure = new List<QItem>();
                smartEnclosure.AddRange(ItemsDetails.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString()));
                smartEnclosure.AddRange(ItemsEnclosure.Where(i => i.SelectionGroup == SelectionGroups.SmartEnclosure.ToString()));
                int oldDetailsCount = smartEnclosure.Where(i => i.ItemTable == Tables.Details.ToString()).Count();
                int oldEnclosureCount = smartEnclosure.Where(i => i.ItemTable == Tables.Enclosure.ToString()).Count();

                query = $"Delete From [Quotation].[QuotationsPanelsItems] Where PanelID = {PanelData.PanelID} And SelectionGroup = '{SelectionGroups.SmartEnclosure}';";
                query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldDetailsCount} + {newDetailsCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Details}'); ";
                query += $"Update [Quotation].[QuotationsPanelsItems] Set ItemSort = (ItemSort - {oldEnclosureCount} + {newEnclosureCount}) Where (PanelID = {PanelData.PanelID}) And (ItemTable = '{Tables.Enclosure}'); ";

                foreach (QItem item in smartEnclosure)
                {
                    if (item.ItemTable == Tables.Details.ToString())
                        ItemsDetails.Remove(item);
                    else
                        ItemsEnclosure.Remove(item);
                }

                foreach (QItem item in ItemsDetails)
                {
                    item.ItemSort = item.ItemSort - oldDetailsCount + newDetailsCount;
                }

                foreach (QItem item in ItemsEnclosure)
                {
                    item.ItemSort = item.ItemSort - oldEnclosureCount + newEnclosureCount;
                }

                foreach (QItem itemData in itemsData)
                {
                    itemData.ItemTable = Tables.Enclosure.ToString();
                    if (itemData.ItemType == null) itemData.ItemType = ItemTypes.Standard.ToString();
                    itemData.SelectionGroup = SelectionGroups.SmartEnclosure.ToString();
                    query += $"Insert Into [Quotation].[QuotationsPanelsItems] " +
                             $"(PanelID, Article1, Article2, Category, Code, Description, Unit, ItemQty, Brand, Remarks, ItemCost, ItemDiscount, ItemTable, ItemType, ItemSort, SelectionGroup) " +
                             $"Values ";
                    itemData.ItemSort = itemsData.IndexOf(itemData);
                    query += $"({PanelData.PanelID}, '{itemData.Article1}', '{itemData.Article2}', '{itemData.Category}', '{itemData.Code}', '{itemData.Description}', '{itemData.Unit}', '{itemData.ItemQty}', '{itemData.Brand}', '{itemData.Remarks}', {itemData.ItemCost}, {itemData.ItemDiscount}, '{itemData.ItemTable}', '{itemData.ItemType}', {itemData.ItemSort}, '{itemData.SelectionGroup}'); ";

                    ItemsEnclosure.Insert(itemData.ItemSort, itemData);
                }

                connection.Execute(query);

                PanelData.EnclosureType = "Universal SF";
                PanelData.EnclosureHeight = 200;
                PanelData.EnclosureWidth = (int.Parse(SF60.Text) * 60) + (int.Parse(SF80.Text) * 80) + (int.Parse(SF100.Text) * 100) + (int.Parse(SF120.Text) * 120);
                PanelData.EnclosureDepth = int.Parse(depth);

                PanelData.EnclosureMetalType = "Steel";
                PanelData.EnclosureColor = "7035";
                PanelData.EnclosureIP = "55";
                PanelData.EnclosureForm = "1";
                PanelData.EnclosureLocation = "Indoor";
                PanelData.EnclosureInstallation = "Floor Standing";
                PanelData.EnclosureFunctional = "With";

                PanelData.EnclosureName = $"Universal SF IP{PanelData.EnclosureIP} {PanelData.EnclosureLocation}";
                query = DatabaseAI.UpdateRecord<QPanel>();
                connection.Execute(query, PanelData);
            }

            this.Close();

        }
    }
}
