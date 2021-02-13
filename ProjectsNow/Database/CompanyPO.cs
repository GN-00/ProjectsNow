using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[WriteTable("[Purchase].[Orders]")]
	public class CompanyPO : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID]public int ID { get; set; }
		public int JobOrderID { get; set; }
		[DontRead][DontWrite] public string JobOrderCode { get; set; }
		public int SupplierID { get; set; }

		private int _Number;
		public int Number
		{
			get { return this._Number; }
			set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); } }
		}

		private string _Code;
		public string Code
		{
			get { return this._Code; }
			set { if (value != this._Code) { this._Code = value; NotifyPropertyChanged(); } }
		}


		private string _QuotationCode;
		public string QuotationCode
		{
			get { return this._QuotationCode; }
			set { if (value != this._QuotationCode) { this._QuotationCode = value; NotifyPropertyChanged(); } }
		}

		private string _SupplierName;
		[DontWrite] public string SupplierName
		{
			get { return this._SupplierName; }
			set { if (value != this._SupplierName) { this._SupplierName = value; NotifyPropertyChanged(); } }
		}

		private string _SupplierCode;
		[DontWrite] public string SupplierCode
		{
			get { return this._SupplierCode; }
			set { if (value != this._SupplierCode) { this._SupplierCode = value; NotifyPropertyChanged(); } }
		}

		private string _SupplierAttentionID;
		public string SupplierAttentionID
		{
			get { return this._SupplierAttentionID; }
			set { if (value != this._SupplierAttentionID) { this._SupplierAttentionID = value; NotifyPropertyChanged(); } }
		}

		private string _SupplierAttention;
		[DontWrite] public string SupplierAttention
		{
			get { return this._SupplierAttention; }
			set { if (value != this._SupplierAttention) { this._SupplierAttention = value; NotifyPropertyChanged(); } }
		}


		private DateTime _Date;
		public DateTime Date
		{
			get { return this._Date; }
			set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
		}

		private string _DeliverToPlace;
		public string DeliverToPlace
		{
			get { return this._DeliverToPlace; }
			set { if (value != this._DeliverToPlace) { this._DeliverToPlace = value; NotifyPropertyChanged(); } }
		}

		private string _DeliverToPerson;
		public string DeliverToPerson
		{
			get { return this._DeliverToPerson; }
			set { if (value != this._DeliverToPerson) { this._DeliverToPerson = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryAddress;
		public string DeliveryAddress
		{
			get { return this._DeliveryAddress; }
			set { if (value != this._DeliveryAddress) { this._DeliveryAddress = value; NotifyPropertyChanged(); } }
		}

		private string _Payment;
		public string Payment
		{
			get { return this._Payment; }
			set { if (value != this._Payment) { this._Payment = value; NotifyPropertyChanged(); } }
		}

		private double _VAT;
		public double VAT
		{
			get { return this._VAT; }
			set { if (value != this._VAT) { this._VAT = value; NotifyPropertyChanged(); } }
		}
	}
}
