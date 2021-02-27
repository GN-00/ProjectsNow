using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Windows.MainWindows;
using ProjectsNow.Windows.MessageWindows;


namespace ProjectsNow.Windows.UserWindows
{
    public partial class LoginWindow : Window
    {
        User userData;
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                userData = connection.QueryFirstOrDefault<User>($"Select * From [User].[Users] Where LoginName ='{txtUsername.Text}' And Password ='{txtPassword.Password}'");
            }

            if (userData != null)
            {
                MainWindow mainWindow = new MainWindow()
                {
                    UserData = this.userData,
                };
                this.Close();
                mainWindow.ShowDialog();
            }
            else
            {
                CMessageBox.Show("Login Error", "Incorrect username or password", CMessageBoxButton.OK, CMessageBoxImage.Warning);
            }
        }
        private void UserKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                txtPassword.Focus();
        }
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_Click(sender, e);
        }
    }
}
