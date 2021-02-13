using Dapper;
using System.Windows;
using System.Windows.Input;
using ProjectsNow.Database;
using System.Data.SqlClient;
using ProjectsNow.Controllers;
using System.Collections.ObjectModel;
using ProjectsNow.Windows.MessageWindows;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ProjectsNow.Windows.QuotationWindows
{
    public partial class RevisionsWindow : Window
    {
        class Revision
        {
            public int Number { get; set; }
            public DateTime? Date { get; set; }
        }
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }

        List<Revision> revisions;
        public RevisionsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                string query = $"Select QuotationRevise as Number, QuotationReviseDate as Date From [Quotation].[Quotations] Where InquiryID = {QuotationData.InquiryID} And QuotationStatus = 'Revision' ";
                revisions = connection.Query<Revision>(query).ToList();
            }

            RevisionsList.ItemsSource = revisions;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (RevisionsList.SelectedItem is Revision revisionData)
            {
                Quotation quotation;
                using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
                {
                    quotation = QuotationController.GetRevision(connection, QuotationData.InquiryID, revisionData.Number);
                }

                var quotationPanelsWindow = new QuotationsInformationWindows.QuotationPanelsWindow()
                {
                    UserData = this.UserData,
                    QuotationData = quotation,
                };
                this.Close();
                quotationPanelsWindow.ShowDialog();
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Select_Click(sender, e);
        }
    }
}
