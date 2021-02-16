using ProjectsNow.Attributes;

using System;

namespace ProjectsNow.Database
{
	[WriteTable("[Finance].[MoneyTransactions]")]
    public class MoneyTransaction
    {
		[ID] public int ID { get; set; }
		public int AccountID { get; set; }
		public int? EmployeeID { get; set; }
		public int? JobOrderID { get; set; }
		public int? CustomerID { get; set; }
		public int? SupplierID { get; set; }
		public string ItemsPO { get; set; }
		public string ItemsInvoiceID { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public double Amount { get; set; }
		public string Type { get; set; }

		[DontWrite] public string AccountName { get; set; }
    }
}
