using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class CategoriesDiscountsWindow : Window
    {
        public ReferencesWindow ReferencesWindowData { get; set; }

        CollectionViewSource viewData;
        ObservableCollection<Category> categories;
        public CategoriesDiscountsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query = $"Select * From [Reference].[CategoriesDiscounts] ";
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                categories = new ObservableCollection<Category>(connection.Query<Category>(query));
            }

            viewData = new CollectionViewSource() { Source = categories };
            CategoriesList.ItemsSource = viewData.View;

            viewData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);

            if (categories.Count == 0)
                CollectionChanged(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = CategoriesList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Categories: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Category: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
        }
        private void CategoriesList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectedIndex = CategoriesList.SelectedIndex;
            if (selectedIndex == -1)
                Navigation.Text = $"Categories: {viewData.View.Cast<object>().Count()}";
            else
                Navigation.Text = $"Category: {selectedIndex + 1} / {viewData.View.Cast<object>().Count()}";
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

        private void ChangeDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesList.SelectedItem is Category categoryData)
            {
                CategoryDiscountWindow categoryDiscountWindow = new CategoryDiscountWindow()
                {
                    ReferencesWindowData = ReferencesWindowData,
                    CategoryData = categoryData,
                };
                categoryDiscountWindow.ShowDialog();
            }
        }
    }
}
