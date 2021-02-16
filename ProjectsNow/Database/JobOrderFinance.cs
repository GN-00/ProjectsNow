using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;
using System;

namespace ProjectsNow.Database
{
	public class JobOrderFinance : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		[ID] public int ID { get; set; }
		public int CustomerID { get; set; }
		public string Code { get; set; }
		public string QuotationCode { get; set; }
		public string ProjectName { get; set; }
		public string CustomerName { get; set; }
        [DontWrite] public double QuotationCost { get; set; }
		[DontWrite] public double ProjectPrice { get; set; }

		[DontWrite] public double Discount { get; set; }
		[DontWrite] public double DiscountPrice
		{
			get { return ((this.Discount / 100) * this.ProjectPrice); }
		}
		[DontWrite] public double VAT { get; set; }
		[DontWrite] public double VATPercentage
		{
			get { return this.VAT * 100; }
		}
		[DontWrite] public double VATPrice
		{
			get { return (this.VAT * this.QuotationCost); }
		}


		private double _Paid;
		[DontWrite]public double Paid
		{
			get { return this._Paid; }
			set { if (value != this._Paid) { this._Paid = value; NotifyPropertyChanged(); NotifyPropertyChanged("Balance"); } }
		}

		public double Balance
        {
            get
            {
				return this.ProjectPrice - this._Paid;
            }
        }

		public string BalanceView
		{ get { return Math.Round(Balance).ToString("N2"); } }

	}

}
