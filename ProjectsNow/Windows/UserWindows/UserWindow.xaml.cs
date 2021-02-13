using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;

namespace ProjectsNow.Windows.UserWindows
{
    public partial class UserWindow : Window
    {
        public User UserData { get; set; }

        User newUserData;
        public UserWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newUserData = new User();
            newUserData.Update(UserData);
            DataContext = newUserData;
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
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UserData.Update(newUserData);
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                var query = DatabaseAI.UpdateRecord<User>();
                connection.Execute(query, UserData);
            }
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseWindow_Click(sender, e);
        }
    }
}
