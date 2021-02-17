using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database.Suppliers
{
	public class SupplierInvoice : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		[ID] public int ID { get; set; }
		public int CustomerID { get; set; }
		public int JobOrderID { get; set; }
		public int SupplierID { get; set; }
		public string Code { get; set; }
		public string InvoiceNumber { get; set; }
		public string SupplierName { get; set; }
		public DateTime Date { get; set; }
		public string QuotationCode { get; set; }
		public string ProjectName { get; set; }
		public string CustomerName { get; set; }
		public double Amount { get; set; }
		public double VAT { get; set; }
		public double InvoiceTotal { get; set; }

		private double _Paid;
		[DontWrite]
		public double Paid
		{
			get { return this._Paid; }
			set 
			{ 
				if (value != this._Paid) 
				{
					this._Paid = value;
					NotifyPropertyChanged(); 
					NotifyPropertyChanged("Balance"); 
					NotifyPropertyChanged("BalanceView");
					NotifyPropertyChanged("Status");
				} 
			}
		}

		public double Balance
		{
			get
			{
				return this.InvoiceTotal - this.Paid;
			}
		}

		public string BalanceView
		{ get { return Balance.ToString("N2"); } }

		public string Status
		{ 
			get 
			{
				double percentage = Math.Round((Paid / InvoiceTotal) * 100);
				if (percentage >= 100) return "Paid in Full";
				else return $"Paid {percentage:N2}%";
			}
		}

	}
}
