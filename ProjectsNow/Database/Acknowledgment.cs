using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[JobOrder].[Acknowledgment]")]
	[WriteTable("[JobOrder].[Acknowledgment]")]
	public class Acknowledgment : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ID { get; set; }
		public int JobOrderID { get; set; }

		private bool _PaymentToggle1 = true;
		public bool PaymentToggle1
		{
			get { return this._PaymentToggle1; }
			set { if (value != this._PaymentToggle1) { this._PaymentToggle1 = value; NotifyPropertyChanged(); } }
		}

		private bool _PaymentToggle2 = false;
		public bool PaymentToggle2
		{
			get { return this._PaymentToggle2; }
			set { if (value != this._PaymentToggle2) { this._PaymentToggle2 = value; NotifyPropertyChanged(); } }
		}

		private double? _DownPayment = 50;
		public double? DownPayment
		{
			get { return this._DownPayment; }
			set { if (value != this._DownPayment) { this._DownPayment = value; NotifyPropertyChanged(); } }
		}

		private double? _BeforeDelivery = 50;
		public double? BeforeDelivery
		{
			get { return this._BeforeDelivery; }
			set { if (value != this._BeforeDelivery) { this._BeforeDelivery = value; NotifyPropertyChanged(); } }
		}

		private double? _AfterDelivery;
		public double? AfterDelivery
		{
			get { return this._AfterDelivery; }
			set { if (value != this._AfterDelivery) { this._AfterDelivery = value; NotifyPropertyChanged(); } }
		}

		private double? _Testing;
		public double? Testing
		{
			get { return this._Testing; }
			set { if (value != this._Testing) { this._Testing = value; NotifyPropertyChanged(); } }
		}

		private string _PaymentOther;
		public string PaymentOther
		{
			get { return this._PaymentOther; }
			set { if (value != this._PaymentOther) { this._PaymentOther = value; NotifyPropertyChanged(); } }
		}

		private bool _DrawingToggle1 = false;
		public bool DrawingToggle1
		{
			get { return this._DrawingToggle1; }
			set { if (value != this._DrawingToggle1) { this._DrawingToggle1 = value; NotifyPropertyChanged(); } }
		}

		private bool _DrawingToggle2 = false;
		public bool DrawingToggle2
		{
			get { return this._DrawingToggle2; }
			set { if (value != this._DrawingToggle2) { this._DrawingToggle2 = value; NotifyPropertyChanged(); } }
		}

		private bool _DrawingToggle3 = true;
		public bool DrawingToggle3
		{
			get { return this._DrawingToggle3; }
			set { if (value != this._DrawingToggle3) { this._DrawingToggle3 = value; NotifyPropertyChanged(); } }
		}

		private bool _DrawingToggle4 = false;
		public bool DrawingToggle4
		{
			get { return this._DrawingToggle4; }
			set { if (value != this._DrawingToggle4) { this._DrawingToggle4 = value; NotifyPropertyChanged(); } }
		}

		private bool _DrawingToggle5 = false;
		public bool DrawingToggle5
		{
			get { return this._DrawingToggle5; }
			set { if (value != this._DrawingToggle5) { this._DrawingToggle5 = value; NotifyPropertyChanged(); } }
		}

		private DateTime? _DrawingDate;
		public DateTime? DrawingDate
		{
			get { return this._DrawingDate; }
			set { if (value != this._DrawingDate) { this._DrawingDate = value; NotifyPropertyChanged(); } }
		}

		private int? _DrawingPeriod = 1;
		public int? DrawingPeriod
		{
			get { return this._DrawingPeriod; }
			set { if (value != this._DrawingPeriod) { this._DrawingPeriod = value; NotifyPropertyChanged(); } }
		}

		private string _DrawingUnit1 = "Week";
		public string DrawingUnit1
		{
			get { return this._DrawingUnit1; }
			set { if (value != this._DrawingUnit1) { this._DrawingUnit1 = value; NotifyPropertyChanged(); } }
		}

		private string _DrawingCondition1 = "Advance Payment";
		public string DrawingCondition1 
		{
			get { return this._DrawingCondition1 ; }
			set { if (value != this._DrawingCondition1 ) { this._DrawingCondition1  = value; NotifyPropertyChanged(); } }
		}

		private int? _DrawingStartPeriod;
		public int? DrawingStartPeriod
		{
			get { return this._DrawingStartPeriod; }
			set { if (value != this._DrawingStartPeriod) { this._DrawingStartPeriod = value; NotifyPropertyChanged(); } }
		}

		private int? _DrawingEndPeriod;
		public int? DrawingEndPeriod
		{
			get { return this._DrawingEndPeriod; }
			set { if (value != this._DrawingEndPeriod) { this._DrawingEndPeriod = value; NotifyPropertyChanged(); } }
		}

		private string _DrawingUnit2;
		public string DrawingUnit2
		{
			get { return this._DrawingUnit2; }
			set { if (value != this._DrawingUnit2) { this._DrawingUnit2 = value; NotifyPropertyChanged(); } }
		}

		private string _DrawingCondition2;
		public string DrawingCondition2
		{
			get { return this._DrawingCondition2; }
			set { if (value != this._DrawingCondition2) { this._DrawingCondition2 = value; NotifyPropertyChanged(); } }
		}

		private string _DrawingOther;
		public string DrawingOther
		{
			get { return this._DrawingOther; }
			set { if (value != this._DrawingOther) { this._DrawingOther = value; NotifyPropertyChanged(); } }
		}

		private bool _DeliveryToggle1 = false;
		public bool DeliveryToggle1
		{
			get { return this._DeliveryToggle1; }
			set { if (value != this._DeliveryToggle1) { this._DeliveryToggle1 = value; NotifyPropertyChanged(); } }
		}

		private bool _DeliveryToggle2 = false;
		public bool DeliveryToggle2
		{
			get { return this._DeliveryToggle2; }
			set { if (value != this._DeliveryToggle2) { this._DeliveryToggle2 = value; NotifyPropertyChanged(); } }
		}

		private bool _DeliveryToggle3 = true;
		public bool DeliveryToggle3
		{
			get { return this._DeliveryToggle3; }
			set { if (value != this._DeliveryToggle3) { this._DeliveryToggle3 = value; NotifyPropertyChanged(); } }
		}

		private bool _DeliveryToggle4 = false;
		public bool DeliveryToggle4
		{
			get { return this._DeliveryToggle4; }
			set { if (value != this._DeliveryToggle4) { this._DeliveryToggle4 = value; NotifyPropertyChanged(); } }
		}

		private bool _DeliveryToggle5 = false;
		public bool DeliveryToggle5
		{
			get { return this._DeliveryToggle5; }
			set { if (value != this._DeliveryToggle5) { this._DeliveryToggle5 = value; NotifyPropertyChanged(); } }
		}

		private DateTime? _DeliveryDate;
		public DateTime? DeliveryDate
		{
			get { return this._DeliveryDate; }
			set { if (value != this._DeliveryDate) { this._DeliveryDate = value; NotifyPropertyChanged(); } }
		}

		private int? _DeliveryPeriod = 1;
		public int? DeliveryPeriod
		{
			get { return this._DeliveryPeriod; }
			set { if (value != this._DeliveryPeriod) { this._DeliveryPeriod = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryUnit1 = "Week";
		public string DeliveryUnit1
		{
			get { return this._DeliveryUnit1; }
			set { if (value != this._DeliveryUnit1) { this._DeliveryUnit1 = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryCondition1 = "Advance Payment & Drawing Approval";
		public string DeliveryCondition1
		{
			get { return this._DeliveryCondition1; }
			set { if (value != this._DeliveryCondition1) { this._DeliveryCondition1 = value; NotifyPropertyChanged(); } }
		}

		private int? _DeliveryStartPeriod;
		public int? DeliveryStartPeriod
		{
			get { return this._DeliveryStartPeriod; }
			set { if (value != this._DeliveryStartPeriod) { this._DeliveryStartPeriod = value; NotifyPropertyChanged(); } }
		}

		private int? _DeliveryEndPeriod;
		public int? DeliveryEndPeriod
		{
			get { return this._DeliveryEndPeriod; }
			set { if (value != this._DeliveryEndPeriod) { this._DeliveryEndPeriod = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryUnit2;
		public string DeliveryUnit2
		{
			get { return this._DeliveryUnit2; }
			set { if (value != this._DeliveryUnit2) { this._DeliveryUnit2 = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryCondition2;
		public string DeliveryCondition2
		{
			get { return this._DeliveryCondition2; }
			set { if (value != this._DeliveryCondition2) { this._DeliveryCondition2 = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryOther;
		public string DeliveryOther
		{
			get { return this._DeliveryOther; }
			set { if (value != this._DeliveryOther) { this._DeliveryOther = value; NotifyPropertyChanged(); } }
		}

		private string _DeliveryPlace = "Ex-Factory";
		public string DeliveryPlace
		{
			get { return this._DeliveryPlace; }
			set { if (value != this._DeliveryPlace) { this._DeliveryPlace = value; NotifyPropertyChanged(); } }
		}

		private int? _WarrantyPeriod = 1;
		public int? WarrantyPeriod
		{
			get { return this._WarrantyPeriod; }
			set { if (value != this._WarrantyPeriod) { this._WarrantyPeriod = value; NotifyPropertyChanged(); } }
		}

		private string _WarrantyUnit = "Year";
		public string WarrantyUnit
		{
			get { return this._WarrantyUnit; }
			set { if (value != this._WarrantyUnit) { this._WarrantyUnit = value; NotifyPropertyChanged(); } }
		}

		private string _WarrantyCondition = "Delivery";
		public string WarrantyCondition
		{
			get { return this._WarrantyCondition; }
			set { if (value != this._WarrantyCondition) { this._WarrantyCondition = value; NotifyPropertyChanged(); } }
		}

		private bool _CancelationToggle = false;
		public bool CancelationToggle
		{
			get { return this._CancelationToggle; }
			set { if (value != this._CancelationToggle) { this._CancelationToggle = value; NotifyPropertyChanged(); } }
		}

		private double _Cancellation1 = 0;
		public double Cancellation1
		{
			get { return this._Cancellation1; }
			set { if (value != this._Cancellation1) { this._Cancellation1 = value; NotifyPropertyChanged(); } }
		}
		private double _Cancellation2 = 20;
		public double Cancellation2
		{
			get { return this._Cancellation2; }
			set { if (value != this._Cancellation2) { this._Cancellation2 = value; NotifyPropertyChanged(); } }
		}
		private double _Cancellation3 = 50;
		public double Cancellation3
		{
			get { return this._Cancellation3; }
			set { if (value != this._Cancellation3) { this._Cancellation3 = value; NotifyPropertyChanged(); } }
		}
		private double _Cancellation4 = 100;
		public double Cancellation4
		{
			get { return this._Cancellation4; }
			set { if (value != this._Cancellation4) { this._Cancellation4 = value; NotifyPropertyChanged(); } }
		}

	}
}
