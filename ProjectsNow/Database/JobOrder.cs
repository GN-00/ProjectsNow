using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[JobOrder].[JobOrdersInformation]")]
	[WriteTable("[JobOrder].[JobOrders]")]
	public class JobOrder : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		[ID]public int ID { get; set; }

		private string _Code;
		public string Code
		{
			get { return this._Code; }
			set { if (value != this._Code) { this._Code = value; NotifyPropertyChanged(); } }
		}
		public int CodeNumber { get; set; }
		public int CodeMonth { get; set; }
		public int CodeYear { get; set; }

		public DateTime? Date { get; set; }

		[JoinID("[Quotation].[Quotations]")] public int QuotationID { get; set; }
		[DontWrite] [Join("[Quotation].[Quotations]")] public string QuotationCode { get; set; }

		[DontWrite] [Join("[Quotation].[Quotations]")] public int InquiryID { get; set; }
					
		private string _ProjectName;
		[DontWrite] [Join("[Inquiry].[Inquiries]")] public string ProjectName
		{
			get { return this._ProjectName; }
			set { if (value != this._ProjectName) { this._ProjectName = value; NotifyPropertyChanged(); } }
		}
		[DontWrite] [Join("[Inquiry].[Inquiries]")] public int CustomerID { get; set; }
		
		private string _CustomerName;
		[DontWrite] [Join("[Customer].[Customers]")] public string CustomerName
		{
			get { return this._CustomerName; }
			set { if (value != this._CustomerName) { this._CustomerName = value; NotifyPropertyChanged(); } }
		}

		[DontWrite] [Join("[Inquiry].[Inquiries]")] public int EstimationID { get; set; }
		[DontWrite] [Join("[User].[Estimation]")] public string EstimationName { get; set; }

	}

	public class JobOrdersYear
	{
		public int Year { get; set; }
	}
}
