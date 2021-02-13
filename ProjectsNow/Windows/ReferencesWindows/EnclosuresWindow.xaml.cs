using Dapper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectsNow.Windows.ReferencesWindows
{
    public partial class EnclosuresWindow : Window
    {
        public User UserData { get; set; }
        public QPanel PanelData { get; set; }
        public ObservableCollection<QItem> ItemsDetails { get; set; }
        public ObservableCollection<QItem> ItemsEnclosure { get; set; }

        List<Enclosure> enclosures;
        public EnclosuresWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                enclosures = connection.Query<Enclosure>($"Select Name As Name From [Reference].[GroupProperties] Where Category ='Enclosure' Order By Sort").ToList();
            }
            EnclosuresList.ItemsSource = enclosures;
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if(EnclosuresList.SelectedItem is Enclosure enclosureData)
            {
                if(enclosureData.Name == "NSYCRN" || enclosureData.Name == "NSYSM" || enclosureData.Name == "Disbo")
                {
                    SelectionWindow selectionWindow = new SelectionWindow()
                    {
                        UserData = this.UserData,
                        GroupName = enclosureData.Name,
                        PanelData = this.PanelData,
                        ItemsDetails = this.ItemsDetails,
                        ItemsEnclosure = this.ItemsEnclosure,
                    };
                    this.Close();
                    selectionWindow.ShowDialog();
                }
                else if (enclosureData.Name == "NSYSF")
                {
                    SFWindow SFWindow = new SFWindow()
                    {
                        UserData = this.UserData,
                        PanelData = this.PanelData,
                        ItemsDetails = this.ItemsDetails,
                        ItemsEnclosure = this.ItemsEnclosure,
                    };
                    this.Close();
                    SFWindow.ShowDialog();
                }
                else if(enclosureData.Name == "Prisma P")
                {
                    PrismaPWindow prismaPWindow = new PrismaPWindow()
                    {
                        UserData = this.UserData,
                        PanelData = this.PanelData,
                        ItemsDetails = this.ItemsDetails,
                        ItemsEnclosure = this.ItemsEnclosure,
                    };
                    this.Close();
                    prismaPWindow.ShowDialog();
                }
                else if (enclosureData.Name == "Prisma G")
                {
                    PrismaGWindow prismaGWindow = new PrismaGWindow()
                    {
                        UserData = this.UserData,
                        PanelData = this.PanelData,
                        ItemsDetails = this.ItemsDetails,
                        ItemsEnclosure = this.ItemsEnclosure,
                    };
                    this.Close();
                    prismaGWindow.ShowDialog();
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }
}
