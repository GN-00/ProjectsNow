using Dapper;
using System.Linq;
using System.Reflection;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using ProjectsNow.Enums;

namespace ProjectsNow.Controllers
{
    public static class InquiryController
	{
		public static void QuotationToInquiry(this Inquiry inquiry, Quotation quotation)
		{
			inquiry.InquiryID = quotation.InquiryID;
			inquiry.CustomerID = quotation.CustomerID;
			inquiry.ConsultantID = quotation.ConsultantID;
			inquiry.EstimationID = quotation.EstimationID;
			inquiry.SalesmanID = quotation.SalesmanID;
			inquiry.RegisterCode = quotation.RegisterCode;
			inquiry.ProjectName = quotation.ProjectName;
			inquiry.RegisterDate = quotation.RegisterDate;
			inquiry.DuoDate = quotation.DuoDate;
			inquiry.Priority = quotation.Priority;
			inquiry.RegisterNumber = quotation.RegisterNumber;
			inquiry.RegisterMonth = quotation.RegisterMonth;
			inquiry.RegisterYear = quotation.RegisterYear;
			inquiry.DeliveryCondition = quotation.DeliveryCondition;
			inquiry.CustomerName = quotation.CustomerName;
			inquiry.EstimationName = quotation.EstimationName;
		}

		public static void Update(this Inquiry newInquiry, Inquiry oldInquiry)
		{
			foreach (PropertyInfo property in oldInquiry.GetType().GetProperties())
			{
				if (property.SetMethod != null)
					newInquiry.GetType().GetProperty(property.Name).SetValue(newInquiry, oldInquiry.GetType().GetProperty(property.Name).GetValue(oldInquiry));
			}
		}

		public static int NewRegisterNumber(SqlConnection connection, int year)
		{
			int newRegisterNumber = (connection.Query<Inquiry>($"Select MAX(RegisterNumber) as RegisterNumber From [Inquiry].[Inquiries] Where RegisterYear = {year}").FirstOrDefault()).RegisterNumber;
			return ++newRegisterNumber;
		}


		public static int CountNewInquiries(SqlConnection connection, int userID)
		{
			var query = $"SELECT COUNT(Inquiries.InquiryID) AS Count " +
						$"FROM [Inquiry].[Inquiries] LEFT OUTER JOIN " +
						$"[User].[Users] ON Users.UserID = Inquiries.EstimationID LEFT OUTER JOIN " +
						$"[Quotation].[Quotations] ON Quotations.InquiryID = Inquiries.InquiryID " +
						$"WHERE(Inquiries.EstimationID = {userID}) AND(Quotations.QuotationID IS NULL) ";
			int Count = (int)connection.ExecuteScalar(query);

			return Count;
		}


		public static ObservableCollection<Inquiry> GetInquiries(SqlConnection connection, User userData, Statuses status, int year)
		{
			var query = $"Select Inquiries.InquiryID, Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, ProjectName, " +
						$"RegisterDate, DuoDate, Priority, UserName as EstimationName, UserCode as EstimationCode, RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +
						$"CustomerName, Quotations.QuotationID " +
						$"FROM [Inquiry].[Inquiries] " +
						$"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Inquiries.CustomerID " +
						$"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
						$"LEFT OUTER JOIN [Quotation].[Quotations] On Quotations.InquiryID = Inquiries.InquiryID " +
						$"Where (RegisterYear = {year}) ";

			if (userData.AccessInquiriesData != true)
				query += $"And (EstimationID = {userData.UserID}) ";

			if(status == Statuses.New)
				query += $"And (Quotations.QuotationID Is Null) ";
			else if(status == Statuses.Running)
				query += $"And (Quotations.QuotationID Is Not Null) ";


			query += $" Order By RegisterYear DESC, RegisterMonth DESC, RegisterNumber DESC";

			var records = new ObservableCollection<Inquiry>(connection.Query<Inquiry>(query));

			return records;
		}

		public static ObservableCollection<InquiryYear> GetInquiriesYears(SqlConnection connection, User userData,Statuses status)
		{
			var query = $"Select RegisterYear as Year " +
						$"FROM [Inquiry].[Inquiries] " +
						$"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
						$"LEFT OUTER JOIN [Quotation].[Quotations] On Quotations.InquiryID = Inquiries.InquiryID " +
						$"Where (1 = 1) ";

			if (userData.AccessInquiriesData != true)
				query += $"And (EstimationID = {userData.UserID}) ";

			if (status == Statuses.New)
				query += $"And (Quotations.QuotationID Is Null) ";
			else if (status == Statuses.Running)
				query += $"And (Quotations.QuotationID Is Not Null) ";

			query += $"GROUP BY Inquiry.Inquiries.RegisterYear " +
					 $"Order By RegisterYear DESC ";

			var records = new ObservableCollection<InquiryYear>(connection.Query<InquiryYear>(query));

			return records;
		}


		public static ObservableCollection<Inquiry> GetNewInquiries(SqlConnection connection, User userData)
		{
			var query = $"Select Inquiries.InquiryID, Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, ProjectName, " +
						$"RegisterDate, DuoDate, Priority, UserName as EstimationName, RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +
						$"CustomerName, Quotations.QuotationID " +
						$"FROM [Inquiry].[Inquiries] " +
						$"LEFT OUTER JOIN [Customer].[Customers] On Customers.CustomerID = Inquiries.CustomerID " +
						$"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
						$"LEFT OUTER JOIN [Quotation].[Quotations] On Quotations.InquiryID = Inquiries.InquiryID " +
						$"Where EstimationID = {userData.UserID} And Quotations.QuotationID Is Null " +
						$"Order By RegisterYear DESC, RegisterMonth DESC, RegisterNumber DESC";

			var records = new ObservableCollection<Inquiry>(connection.Query<Inquiry>(query));

			return records;
		}

		public static Quotation CheckQuotation(SqlConnection connection, int inquiryID)
		{
			var query = $"Select Quotations.QuotationID, QuotationStatus " +
						$"From [Quotation].[Quotations] " +
						$"WHERE Quotations.InquiryID = {inquiryID}";

			var quotation = connection.QueryFirstOrDefault<Quotation>(query);
			return quotation;
		}
	}
}
