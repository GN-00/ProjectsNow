using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Windows.Controls;
using ProjectsNow.Controllers;
using System.Collections.Generic;

namespace ProjectsNow.Printing
{
    public partial class QuotationCoverPage : UserControl
    {
        public bool? BackgroundData { get; set; }
        public User UserData { get; set; }
        public Quotation QuotationData { get; set; }
        public List<QuotationContent> Contents { get; set; }

        Contact ContactData { get; set; }

        public QuotationCoverPage()
        {
            InitializeComponent();
        }

        public QuotationCoverPage(User user, Quotation quotation, List<QuotationContent> contents)
        {
            InitializeComponent();
            UserData = user;
            QuotationData = quotation;
            using (SqlConnection connection = new SqlConnection(DatabaseAI.ConnectionString))
            {
                ContactData = ContactController.GetProjectAttention(connection, QuotationData.InquiryID);
            }

            Contents = contents;
            DataContext = new { UserData, QuotationData, ContactData, Contents };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;
            if (BackgroundData == null) BackgroundData = false;
            if (BackgroundData.Value) Background.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
