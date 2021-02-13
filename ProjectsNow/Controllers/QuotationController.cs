using Dapper;
using System.Linq;
using ProjectsNow.Enums;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace ProjectsNow.Controllers
{
    public static class QuotationController
    {
        public static void Update(this Quotation oldQuotation, Quotation newQuotation)
        {
            foreach (PropertyInfo property in typeof(Quotation).GetProperties())
            {
                if (property.SetMethod != null)
                    oldQuotation.GetType().GetProperty(property.Name).
                    SetValue(oldQuotation, typeof(Quotation).GetProperty(property.Name).GetValue(newQuotation));
            }
        }

        public static void Update(this Quotation quotation, Inquiry inquiry)
        {
            foreach (PropertyInfo property in typeof(Inquiry).GetProperties())
            {
                if (property.SetMethod != null)
                    quotation.GetType().GetProperty(property.Name).
                    SetValue(quotation, typeof(Inquiry).GetProperty(property.Name).GetValue(inquiry));
            }
        }

        public static int NewQuotationNumber(SqlConnection connection, int year)
        {
            int newQuotationNumber = (connection.Query<Quotation>($"Select MAX(QuotationNumber) as QuotationNumber From [Quotation].[Quotations] Where QuotationYear = {year}").FirstOrDefault()).QuotationNumber;
            return ++newQuotationNumber;
        }

        public static Quotation GetRevision(SqlConnection connection, int inquiryID, int revise)
        {
            var query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +                                //Quotations
                        $"SubmitDate, QuotationNumber, QuotationRevise, QuotationReviseDate, QuotationYear, QuotationMonth, " +                   //Quotations
                        $"PowerVoltage, Phase, Frequency, NetworkSystem, " +                                                                      //Quotations
                        $"ControlVoltage, TinPlating, NeutralSize, EarthSize EarthingSystem, VAT, Discount, " +                                   //Quotations
                        $"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +                                   //Inquiries
                        $"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                                                    //Inquiries
                        $"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                                                          //Inquiries
                        $"UserName as EstimationName, " +                                                                                         //Users
                        $"CustomerName, " +                                                                                                       //Customers
                        $"QuotationCost " +                                                                                                       //QuotationsCost
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
                        $"Where QuotationRevise = {revise} And QuotationStatus = 'Revision' And Quotations.InquiryID = {inquiryID} ";

            var quotation = connection.QueryFirstOrDefault<Quotation>(query);
            return quotation;
        }
        public static ObservableCollection<Quotation> GetQuotations(SqlConnection connection, int year, Statuses quotationStatus)
        {
            var query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +                                //Quotations
                        $"SubmitDate, QuotationNumber, QuotationRevise, QuotationReviseDate, QuotationYear, QuotationMonth, " +                   //Quotations
                        $"PowerVoltage, Phase, Frequency, NetworkSystem, " +                                                                      //Quotations
                        $"ControlVoltage, TinPlating, NeutralSize, EarthSize EarthingSystem, VAT, Discount, " +                                   //Quotations
                        $"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +                                   //Inquiries
                        $"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                                                    //Inquiries
                        $"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                                                          //Inquiries
                        $"UserName as EstimationName, " +                                                                                         //Users
                        $"CustomerName, " +                                                                                                       //Customers
                        $"QuotationCost " +                                                                                                       //QuotationsCost
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
                        $"Where QuotationYear = {year} ";

            if (quotationStatus == Statuses.All)
                query += $"And QuotationStatus != 'Revision' ";
            else
                query += $"And QuotationStatus = '{quotationStatus}' ";

            query += $"Order By QuotationYear Desc, QuotationNumber Desc";

            var quotations = new ObservableCollection<Quotation>(connection.Query<Quotation>(query));
            return quotations;
        }

        public static ObservableCollection<Quotation> UserQuotations(SqlConnection connection, int userID, int year, Statuses quotationStatus)
        {
            var query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +        //Quotations
                        $"SubmitDate, QuotationNumber, QuotationRevise, QuotationReviseDate, QuotationYear, QuotationMonth, " +                //Quotations
                        $"PowerVoltage, Phase, Frequency, NetworkSystem, " +                                              //Quotations
                        $"ControlVoltage, TinPlating, NeutralSize, EarthSize EarthingSystem, VAT, Discount, " +           //Quotations
                        $"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +           //Inquiries
                        $"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                            //Inquiries
                        $"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                                  //Inquiries
                        $"UserName as EstimationName, " +                                                                 //Users
                        $"CustomerName, " +                                                                               //Customers
                        $"QuotationCost " +                                                                               //QuotationsCost
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
                        $"Where QuotationYear = {year} And EstimationID = {userID} ";

            if (quotationStatus == Statuses.All)
                query += $"And QuotationStatus != 'Revision' ";
            else
                query += $"And QuotationStatus = '{quotationStatus}' ";

            query += $"Order By QuotationYear Desc, QuotationNumber Desc";

            var quotations = new ObservableCollection<Quotation>(connection.Query<Quotation>(query));
            return quotations;
        }

        public static Quotation GetQuotation(SqlConnection connection, int quotationID)
        {
            var query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +    //Quotations
                        $"SubmitDate, QuotationNumber, QuotationRevise, QuotationYear, QuotationMonth, " +            //Quotations
                        $"PowerVoltage, Phase, Frequency, NetworkSystem, " +                                          //Quotations
                        $"ControlVoltage, TinPlating, NeutralSize, EarthSize, EarthingSystem, VAT, Discount, " +      //Quotations
                        $"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +       //Inquiries
                        $"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                        //Inquiries
                        $"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                              //Inquiries
                        $"UserName as EstimationName, " +                                                             //Users
                        $"CustomerName, " +                                                                           //Customers
                        $"QuotationCost " +                                                                           //QuotationsCost
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
                        $"WHERE Quotations.QuotationID = {quotationID}";

            var quotation = connection.Query<Quotation>(query).FirstOrDefault();
            return quotation;
        }

        public static ObservableCollection<QuotationsYear> UserQuotationsYears(SqlConnection connection, int userID, Statuses quotationStatus)
        {
            var query = $"Select " +
                        $"QuotationYear as Year " +
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"Where EstimationID = {userID} ";

            if (quotationStatus == Statuses.All)
                query += $"And QuotationStatus != 'Revision' ";
            else
                query += $"And QuotationStatus = '{quotationStatus}' ";

            query += $"Group By QuotationYear ORDER BY Year";

            var quotations = new ObservableCollection<QuotationsYear>(connection.Query<QuotationsYear>(query));
            return quotations;
        }

        public static ObservableCollection<QuotationsYear> QuotationsYears(SqlConnection connection, Statuses quotationStatus)
        {
            var query = $"Select " +
                        $"QuotationYear as Year " +
                        $"From [Quotation].[Quotations] " +
                        $"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
                        $"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
                        $"Where ";

            if (quotationStatus == Statuses.All)
                query += $"QuotationStatus != 'Revision' ";
            else
                query += $"QuotationStatus = '{quotationStatus}' ";

            query += $"Group By QuotationYear ORDER BY Year";

            var quotations = new ObservableCollection<QuotationsYear>(connection.Query<QuotationsYear>(query));
            return quotations;
        }

    }
}
