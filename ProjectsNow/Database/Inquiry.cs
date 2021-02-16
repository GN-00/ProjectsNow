using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[Inquiry].[Inquiries]")]
	[WriteTable("[Inquiry].[Inquiries]")]
	public class Inquiry : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int InquiryID { get; set; }
		public int CustomerID { get; set; }
		public int ConsultantID { get; set; }
		public int EstimationID { get; set; }
		public int SalesmanID { get; set; }

		private int? _QuotationID;
		[DontWrite] public int? QuotationID
		{
			get { return this._QuotationID; }
			set { if (value != this._QuotationID) { this._QuotationID = value; NotifyPropertyChanged(); NotifyPropertyChanged("Status"); } }
		}

		private string _RegisterCode;
		public string RegisterCode
		{
			get { return this._RegisterCode; }
			set { if (value != this._RegisterCode) { this._RegisterCode = value; NotifyPropertyChanged(); } }
		}

		private string _ProjectName;
		public string ProjectName
		{
			get { return this._ProjectName; }
			set { if (value != this._ProjectName) { this._ProjectName = value; NotifyPropertyChanged(); } }
		}

		private DateTime _RegisterDate = DateTime.Now;
		public DateTime RegisterDate
		{
			get { return this._RegisterDate; }
			set { if (value != this._RegisterDate) { this._RegisterDate = value; NotifyPropertyChanged(); } }
		}

		private DateTime _DuoDate = DateTime.Now.AddDays(7);
		public DateTime DuoDate
		{
			get { return this._DuoDate; }
			set { if (value != this._DuoDate) { this._DuoDate = value; NotifyPropertyChanged(); } }
		}

		private string _Priority = "Normal";
		public string Priority
		{
			get { return this._Priority; }
			set { if (value != this._Priority) { this._Priority = value; NotifyPropertyChanged(); } }
		}

		public int RegisterNumber { get; set; }
		public int RegisterMonth { get; set; }
		public int RegisterYear { get; set; }
		public string DeliveryCondition { get; set; } = "Ex-Factory";

		//Join
		private string _CustomerName;
		[DontWrite]
		public string CustomerName
		{
			get { return this._CustomerName; }
			set { if (value != this._CustomerName) { this._CustomerName = value; NotifyPropertyChanged(); } }
		}

		private string _EstimationName;
		[DontWrite]
		public string EstimationName
		{
			get { return this._EstimationName; }
			set { if (value != this._EstimationName) { this._EstimationName = value; NotifyPropertyChanged(); } }
		}

		private string _EstimationCode;
		[DontWrite]
		public string EstimationCode
		{
			get { return this._EstimationCode; }
			set { if (value != this._EstimationCode) { this._EstimationCode = value; NotifyPropertyChanged(); } }
		}

		[DontRead][DontWrite] public string Status { get { return (QuotationID == null ? "New" : "Quoting"); } }
	}

	public class InquiryYear
	{
		public int Year { get; set; }
	}
}
