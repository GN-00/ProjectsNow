using Dapper;
using System.Linq;
using System.Windows;
using ProjectsNow.Enums;
using System.Windows.Data;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ProjectsNow.Windows.MessageWindows;
using System.ComponentModel;

namespace ProjectsNow.Windows.CustomerWindows
{
    public partial class ConsultantsWindow : Window
    {
        public User UserData { get; set; }
        public ObservableCollection<Consultant> RecordsData { get; set; }

        Actions action;
        Consultant oldData;
        Consultant recordData;

        CollectionViewSource viewRecordsData;
        public ConsultantsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (RecordsData == null)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    RecordsData = new ObservableCollection<Consultant>(ConsultantController.GetConsultants(connection));
                }
            }
            viewRecordsData = new CollectionViewSource() { Source = RecordsData };
            RecordsList.ItemsSource = viewRecordsData.View;

            viewRecordsData.Filter += DataFiltrer;
            viewRecordsData.View.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);

            recordData = RecordsList.SelectedItem as Consultant;
            DataContext = new { recordData, UserData };
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Visible;
            Done.Visibility = ReadOnly.Visibility = Visibility.Collapsed;
            action = Actions.New;
            recordData = new Consultant();
            DataContext = new { recordData, UserData };
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            User usedBy;
            if (recordData != null)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserConsultantID(connection, recordData.ConsultantID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.ConsultantID = recordData.ConsultantID;
                        UserController.UpdateConsultantID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Visible;
                    Done.Visibility = ReadOnly.Visibility = Visibility.Collapsed;
                    action = Actions.Edit;
                    oldData = new Consultant();
                    oldData.Update(recordData);
                }
                else
                {
                    CMessageBox.Show($"Access", $"This customer Data underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            User usedBy;
            if (recordData != null)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    usedBy = UserController.CheckUserConsultantID(connection, recordData.ConsultantID);
                    if (usedBy == null || usedBy.UserID == UserData.UserID)
                    {
                        UserData.ConsultantID = recordData.ConsultantID;
                        UserController.UpdateConsultantID(connection, UserData);
                    }
                }

                if (usedBy == null || usedBy.UserID == UserData.UserID)
                {
                    LoadingControl.Visibility = Visibility.Visible;

                    MessageBoxResult result = CMessageBox.Show($"Deleting", $"Are you sure want to delete:\n{recordData.ConsultantName} ?", CMessageBoxButton.YesNo, CMessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            if (recordData.AbilityToDelete(connection))
                            {
                                connection.Execute($"Delete From [Customer].[Consultants] Where ConsultantID = {recordData.ConsultantID} ");
                                RecordsData.Remove(recordData);

                                UserData.ConsultantID = null;
                                UserController.UpdateConsultantID(connection, UserData);
                            }
                            else
                            {
                                CMessageBox.Show("Deleting", $"You can't delete {recordData.ConsultantName} data \n Because its used in projects data", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                            }
                        }
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                        {
                            UserData.ConsultantID = null;
                            UserController.UpdateConsultantID(connection, UserData);
                        }
                    }

                    LoadingControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CMessageBox.Show($"Access", $"This customer Data underwork by {usedBy.UserName}!", CMessageBoxButton.OK, CMessageBoxImage.Warning);
                }
            }

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Collapsed;
            Done.Visibility = ReadOnly.Visibility = Visibility.Visible;
            if (action == Actions.New)
            {
                RecordsData.Add(recordData);
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.InsertRecord<Consultant>();
                    recordData.ConsultantID = (int)(decimal)connection.ExecuteScalar(query, recordData);
                }
            }
            else if (action == Actions.Edit)
            {
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    var query = DatabaseAI.UpdateRecord<Consultant>();
                    connection.Execute(query, recordData);

                    UserData.ConsultantID = null;
                    UserController.UpdateConsultantID(connection, UserData);
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.Visibility = Save.Visibility = Cancel.Visibility = Visibility.Collapsed;
            Done.Visibility = ReadOnly.Visibility = Visibility.Visible;
            if (action == Actions.New)
            {
                recordData = RecordsList.SelectedItem as Consultant;
                DataContext = new { recordData, UserData };
            }
            else if (action == Actions.Edit)
            {
                recordData.Update(oldData);

                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    UserData.ConsultantID = null;
                    UserController.UpdateConsultantID(connection, UserData);
                }
            }
        }
        private void List_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            recordData = RecordsList.SelectedItem as Consultant;
            var selectedIndex = RecordsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Consultants: {viewRecordsData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Consultant: {selectedIndex + 1} / {viewRecordsData.View.Cast<object>().Count()}";
            DataContext = new { recordData, UserData };
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selectedIndex = RecordsList.SelectedIndex;
            if (selectedIndex == -1)
                NavigationPanel.Text = $"Consultants: {viewRecordsData.View.Cast<object>().Count()}";
            else
                NavigationPanel.Text = $"Consultant: {selectedIndex + 1} / {viewRecordsData.View.Cast<object>().Count()}";

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                viewRecordsData.View.SortDescriptions.Add(new SortDescription("ConsultantName", ListSortDirection.Ascending));
            }
        }

        private void DataFiltrer(object sender, FilterEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FindRecord.Text))
            {
                try
                {
                    e.Accepted = true;
                    if (e.Item is Consultant consultant)
                    {
                        string value = consultant.ConsultantName.ToUpper();
                        if (!value.Contains(FindRecord.Text.ToUpper()))
                        {
                            e.Accepted = false;
                            return;
                        }
                    }
                }
                catch
                {
                    e.Accepted = true;
                }
            }

        }
        private void FilterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            viewRecordsData.View.Refresh();
        }
        private void RemoveFilter(object sender, RoutedEventArgs e)
        {
            FindRecord.Text = null;
            viewRecordsData.View.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                UserData.ConsultantID = null;
                UserController.UpdateConsultantID(connection, UserData);
            }
        }
    }
}
