using Dapper;
using System;
using ProjectsNow.Database;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ProjectsNow.Controllers
{
	public static class JobOrderController
	{
		public static void Update(this JobOrder oldJobOrder, JobOrder newJobOrder)
		{
			foreach (PropertyInfo property in typeof(JobOrder).GetProperties())
			{
				if (property.SetMethod != null)
					typeof(JobOrder).GetProperty(property.Name).SetValue(oldJobOrder, typeof(JobOrder).GetProperty(property.Name).GetValue(newJobOrder));
			}
		}

		public static ObservableCollection<Quotation> QuotationsWaitPO(SqlConnection connection, int year)
        {
			var query = $"Select Quotations.QuotationID, Quotations.InquiryID, QuotationCode, QuotationStatus, " +        //Quotations
						$"SubmitDate, QuotationNumber, QuotationRevise, QuotationYear, QuotationMonth, " +                //Quotations
						$"Quotations.PowerVoltage, Quotations.Phase, Quotations.Frequency, Quotations.NetworkSystem, " +  //Quotations
						$"Quotations.ControlVoltage, Quotations.TinPlating, Quotations.NeutralSize," +                    //Quotations
						$"Quotations.EarthSize, Quotations.EarthingSystem, VAT, Discount, " +                             //Quotations
						$"Inquiries.CustomerID, Inquiries.ConsultantID, EstimationID, Inquiries.SalesmanID, " +           //Inquiries
						$"Priority, ProjectName, DuoDate, RegisterDate, DeliveryCondition, " +                            //Inquiries
						$"RegisterCode, RegisterYear, RegisterMonth, RegisterNumber, " +                                  //Inquiries
						$"UserName as EstimationName, " +                                                                 //Users
						$"CustomerName, " +                                                                               //Customers
						$"QuotationCost " +                                                                               //QuotationsCost
						$"From [Quotation].[Quotations] " +
						$"LEFT OUTER JOIN [JobOrder].[JobOrders] On Quotations.QuotationID = JobOrders.QuotationID " +
						$"LEFT OUTER JOIN [Inquiry].[Inquiries] On Quotations.InquiryID = Inquiries.InquiryID " +
						$"LEFT OUTER JOIN [Customer].[Customers] On Inquiries.CustomerID = Customers.CustomerID " +
						$"LEFT OUTER JOIN [User].[Users] On Users.UserID = Inquiries.EstimationID " +
						$"LEFT OUTER JOIN [Quotation].[QuotationsCost] On [Quotations].QuotationID = [QuotationsCost].QuotationID " +
						$"Where QuotationYear = {year} And JobOrders.ID Is Null And " +
						$"QuotationStatus = 'Submitted' ";

			var records = connection.Query<Quotation>(query);
			return new ObservableCollection<Quotation>(records);
		}
		public static ObservableCollection<QuotationsYear> QuotationsWaitingPOYears(SqlConnection connection)
		{
			var query = $"Select " +
						$"QuotationYear as Year " +
						$"From [Quotation].[Quotations] " +
						$"LEFT OUTER JOIN [JobOrder].[JobOrders] On Quotations.QuotationID = JobOrders.QuotationID " +
						$"Where JobOrders.ID Is Null And QuotationStatus = 'Submitted'" +
						$"Group By QuotationYear";

			var quotations = new ObservableCollection<QuotationsYear>(connection.Query<QuotationsYear>(query));
			return quotations;
		}

		public static ObservableCollection<JobOrder> JobOrders(SqlConnection connection, int year)
		{
			string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
						   $"Where CodeYear = {year} ";
			var records = connection.Query<JobOrder>(query);
			return new ObservableCollection<JobOrder>(records);
		}
		public static ObservableCollection<JobOrdersYear> JobOrdersYears(SqlConnection connection)
		{
			var query = $"Select " +
						$"CodeYear as Year " +
						$"From [JobOrder].[JobOrders] " +
						$"Group By CodeYear";

			var records = new ObservableCollection<JobOrdersYear>(connection.Query<JobOrdersYear>(query));
			return records;
		}

		public static JobOrder JobOrder(SqlConnection connection, int jobOrderID)
		{
			string query = $"Select * From [JobOrder].[JobOrdersInformation] " +
						   $"Where ID = {jobOrderID} ";
			var record = connection.QueryFirstOrDefault<JobOrder>(query);
			return record;
		}

		public static ObservableCollection<JobOrder> GetRunningJobOrders(SqlConnection connection, int year)
		{
			string query = $"Select * From [JobOrder].[JobOrders(Running)] " +
						   $"Where CodeYear = {year} Order By CodeYear DESC, CodeNumber DESC";
			var records = connection.Query<JobOrder>(query);
			return new ObservableCollection<JobOrder>(records);
		}
		public static ObservableCollection<JobOrdersYear> GetRunningJobOrdersYears(SqlConnection connection)
		{
			string query = $"Select CodeYear as Year From [JobOrder].[JobOrders(Running)] " +
						   $"Group By CodeYear ";
			var records = connection.Query<JobOrdersYear>(query);
			return new ObservableCollection<JobOrdersYear>(records);
		}

		public static ObservableCollection<JobOrder> GetClosedJobOrders(SqlConnection connection, int year)
		{
			string query = $"Select * From [JobOrder].[JobOrders(Closed)] " +
						   $"Where CodeYear = {year} ";
			var records = connection.Query<JobOrder>(query);
			return new ObservableCollection<JobOrder>(records);
		}
		public static ObservableCollection<JobOrdersYear> GetClosedJobOrdersYears(SqlConnection connection)
		{
			string query = $"Select CodeYear as Year From [JobOrder].[JobOrders(Closed)] " +
						   $"Group By CodeYear ";
			var records = connection.Query<JobOrdersYear>(query);
			return new ObservableCollection<JobOrdersYear>(records);
		}


		public static int GetCodeNumber(SqlConnection connection)
		{
			string query = $"Select MAX(CodeNumber) as CodeNumber From [JobOrder].[JobOrders] Where CodeYear = {DateTime.Today.Year}";
			var codeNumber = (connection.QueryFirstOrDefault<JobOrder>(query)).CodeNumber;
			return ++codeNumber;
		}

	}
}
