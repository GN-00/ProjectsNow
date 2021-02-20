using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[WriteTable("[Finance].[MoneyTransactions]")]
    public class MoneyTransaction : INotifyPropertyChanged  
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ID { get; set; }
		public int AccountID { get; set; }
		public int? EmployeeID { get; set; }
		public int? JobOrderID { get; set; }
		public int? CustomerID { get; set; }
		public int? SupplierID { get; set; }
		public string ItemsPO { get; set; }
		public int? SupplierInvoiceID { get; set; }

		private DateTime _Date;
		public DateTime Date
		{
			get { return this._Date; }
			set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
		}
		private string _Description;
		public string Description
		{
			get { return this._Description; }
			set { if (value != this._Description) { this._Description = value; NotifyPropertyChanged(); } }
		}
		private double _Amount;
		public double Amount
		{
			get { return this._Amount; }
			set { if (value != this._Amount) { this._Amount = value; NotifyPropertyChanged(); } }
		}

		public string Type { get; set; }

		private string _AccountName;
		[DontWrite]public string AccountName
		{
			get { return this._AccountName; }
			set { if (value != this._AccountName) { this._AccountName = value; NotifyPropertyChanged(); } }
		}
	}
}
