using Dapper;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Data.SqlClient;
using ProjectsNow.Attributes;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[Quotation].[QuotationsPanels]")]
	[WriteTable("[Quotation].[QuotationsPanels]")]
	public class QPanel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] [JoinID("[Quotation].[QuotationsPanelsCost]")] public int PanelID { get; set; }
		public int RevisePanelID { get; set; }
		public int QuotationID { get; set; }
		public string PurchaseOrdersNumber { get; set; }

		private int? _PanelSN;
		public int? PanelSN
		{
			get { return this._PanelSN; }
			set { if (value != this._PanelSN) { this._PanelSN = value; NotifyPropertyChanged(); } }
		}

		private string _PanelName;
		public string PanelName
		{
			get { return this._PanelName; }
			set { if (value != this._PanelName) { this._PanelName = value; NotifyPropertyChanged(); } }
		}

		private int _PanelQty = 1;
		public int PanelQty
		{
			get { return this._PanelQty; }
			set
			{
				if (value != this._PanelQty)
				{
					if (value > 0)
						this._PanelQty = value;

					NotifyPropertyChanged("PanelsCost");
					NotifyPropertyChanged("PanelPrice");
					NotifyPropertyChanged("PanelsPrice");
					NotifyPropertyChanged();
				}
			}
		}

		private string _PanelType = "Power Panel";
		public string PanelType
		{
			get { return this._PanelType; }
			set { if (value != this._PanelType) { this._PanelType = value; NotifyPropertyChanged(); } }
		}

		private double _PanelProfit;
		public double PanelProfit
		{
			get { return this._PanelProfit; }
			set
			{
				if (value != this._PanelProfit)
				{
					if (value <= 100)
						this._PanelProfit = value;

					NotifyPropertyChanged("PanelsCost");
					NotifyPropertyChanged("PanelPrice");
					NotifyPropertyChanged("PanelsPrice");
					NotifyPropertyChanged();
				}
			}
		}

		private string _EnclosureName;
		public string EnclosureName
		{
			get { return this._EnclosureName; }
			set { if (value != this._EnclosureName) { this._EnclosureName = value; NotifyPropertyChanged(); } }
		}

		private string _EnclosureType;
		public string EnclosureType
		{
			get { return this._EnclosureType; }
			set { if (value != this._EnclosureType) { this._EnclosureType = value; NotifyPropertyChanged(); } }
		}

		private string _EnclosureInstallation;
		public string EnclosureInstallation
		{
			get { return this._EnclosureInstallation; }
			set { if (value != this._EnclosureInstallation) { this._EnclosureInstallation = value; NotifyPropertyChanged(); } }
		}

		private double? _EnclosureHeight;
		public double? EnclosureHeight
		{
			get { return this._EnclosureHeight; }
			set { if (value != this._EnclosureHeight) { this._EnclosureHeight = value; NotifyPropertyChanged(); } }
		}

		private double? _EnclosureWidth;
		public double? EnclosureWidth
		{
			get { return this._EnclosureWidth; }
			set { if (value != this._EnclosureWidth) { this._EnclosureWidth = value; NotifyPropertyChanged(); } }
		}

		private double? _EnclosureDepth;
		public double? EnclosureDepth
		{
			get { return this._EnclosureDepth; }
			set { if (value != this._EnclosureDepth) { this._EnclosureDepth = value; NotifyPropertyChanged(); } }
		}

		private string _EnclosureIP;
		public string EnclosureIP
		{
			get { return this._EnclosureIP; }
			set { if (value != this._EnclosureIP) { this._EnclosureIP = value; NotifyPropertyChanged(); } }
		}

		public string EnclosureLocation { get; set; }
		public string EnclosureColor { get; set; }
		public string EnclosureMetalType { get; set; }
		public string EnclosureForm { get; set; }
		public string EnclosureDoor { get; set; }
		public string EnclosureFunctional { get; set; }


		public string Source { get; set; }
		public double? Icu { get; set; }
		public string Frequency { get; set; } = "60Hz";
		public string PowerSupplyOperation { get; set; } = "No Parallel Operation";
		public string EarthingSystem { get; set; } = "Earthed (TNS)";
		public string DirectCurrent { get; set; } = "To Earth";

		public string Busbar { get; set; } = "Bare Copper";
		public string BusbarHorizontal { get; set; }
		public string BusbarVertical { get; set; }
		public string NeutralSize { get; set; }
		public string EarthSize { get; set; }

		public string SignallingVoltage { get; set; } = "N/A";
		public string SignallingSource { get; set; } = "N/A";
		public string ControlVoltage { get; set; } = "N/A";
		public string ControlSource { get; set; } = "N/A";
		public string SensorsVoltage { get; set; } = "N/A";
		public string SensorsSource { get; set; } = "N/A";
		public string LightingVoltage { get; set; } = "N/A";
		public string LightingSource { get; set; } = "N/A";

		public string ClimateType { get; set; } = "Normal";
		public string AtmosphereType { get; set; } = "Ordinary";
		public string PollutionRisks { get; set; }
		public double? AmbientTemperature { get; set; } = 40;
		public double? RelativeHumidity { get; set; } = 50;
		public double SeaLevel { get; set; } = 0;

		public string IndicationType { get; set; } = "Without";
		public string IndicationSize { get; set; }
		public string IndicationOther { get; set; }
		public string MeterRelay { get; set; }

		public bool IncomingFixed { get; set; } = false;
		public bool IncomingPlugIn { get; set; } = false;
		public bool IncomingDrawout { get; set; } = false;
		public bool IncomingMotorized { get; set; } = false;
		public bool IncomingBehindDoor { get; set; } = false;
		public bool IncomingThroughPlate { get; set; } = false;
		public bool IncomingInterLock { get; set; } = false;
		public bool IncomingPadlocking { get; set; } = false;
		public bool IncomingShutter { get; set; } = false;
		public bool IncomingThroughDoor { get; set; } = false;
		public bool IncomingThroughCover { get; set; } = false;
		public bool IncomingDirect { get; set; } = false;
		public bool IncomingTerminalBlocks { get; set; } = false;
		public bool IncomingBusbarLinks { get; set; } = false;
		public bool IncomingFront { get; set; } = false;
		public bool IncomingRear { get; set; } = false;
		public bool IncomingLeftRight { get; set; } = false;
		public bool IncomingTopCables { get; set; } = false;
		public bool IncomingTopBusduct { get; set; } = false;
		public bool IncomingBottomCables { get; set; } = false;
		public bool IncomingScrews { get; set; } = false;
		public bool IncomingGlandPlate { get; set; } = false;
		public bool IncomingCableGland { get; set; } = false;
		public bool IncomingShrouding { get; set; } = false;

		public bool OutgoingFixed { get; set; } = false;
		public bool OutgoingPlugIn { get; set; } = false;
		public bool OutgoingDrawout { get; set; } = false;
		public bool OutgoingMotorized { get; set; } = false;
		public bool OutgoingBehindDoor { get; set; } = false;
		public bool OutgoingThroughPlate { get; set; } = false;
		public bool OutgoingInterLock { get; set; } = false;
		public bool OutgoingPadlocking { get; set; } = false;
		public bool OutgoingShutter { get; set; } = false;
		public bool OutgoingThroughDoor { get; set; } = false;
		public bool OutgoingThroughCover { get; set; } = false;
		public bool OutgoingDirect { get; set; } = false;
		public bool OutgoingTerminalBlocks { get; set; } = false;
		public bool OutgoingBusbarLinks { get; set; } = false;
		public bool OutgoingFront { get; set; } = false;
		public bool OutgoingRear { get; set; } = false;
		public bool OutgoingLeftRight { get; set; } = false;
		public bool OutgoingTopCables { get; set; } = false;
		public bool OutgoingTopBusduct { get; set; } = false;
		public bool OutgoingBottomCables { get; set; } = false;
		public bool OutgoingScrews { get; set; } = false;
		public bool OutgoingGlandPlate { get; set; } = false;
		public bool OutgoingCableGland { get; set; } = false;
		public bool OutgoingShrouding { get; set; } = false;

		public string PushButtonON { get; set; } = "Without";
		public string PushButtonOFF { get; set; } = "Without";
		public string PushButtonReset { get; set; } = "Without";
		public string SignallingON { get; set; } = "Without";
		public string SignallingOFF { get; set; } = "Without";
		public string SignallingTrip { get; set; } = "Without";

		public string ExternalLabelType { get; set; } = "Gravely";
		public string ExternalLabelFixeing { get; set; } = "Glued";
		public string ExternalLabelLanguage { get; set; } = "English";
		public string InternalLabelType { get; set; } = "Gravely";
		public string InternalLabelFixeing { get; set; } = "Glued";
		public string InternalLabelLanguage { get; set; } = "English";
		public string EquipmentLabelType { get; set; } = "Sticker";
		public string EquipmentLabelFixeing { get; set; } = "Self Stick";
		public string EquipmentLabelLanguage { get; set; } = "English";
		public string LabelBackground { get; set; } = "Black";
		public string LabelFont { get; set; } = "White";

		public double? AuxiliaryVoltageSection { get; set; } = 1.5;
		public string AuxiliaryVoltageColor { get; set; } = "Black";
		public string AuxiliaryVoltageType { get; set; } = "XLPE";
		public double? AuxiliaryCurrentSection { get; set; } = 1.5;
		public string AuxiliaryCurrentColor { get; set; } = "Black";
		public string AuxiliaryCurrentType { get; set; } = "XLPE";

		public string ApparatusDefind { get; set; } = "Yes";
		public string Weight { get; set; } = "XX";
		public bool ForInformation { get; set; }
		public bool ForProduction { get; set; }
		public bool ForApproval { get; set; }
		public bool AsManufactured { get; set; }
		public string Remarks { get; set; }


		private double _PanelCost;
		[DontWrite]
		[Join("[Quotation].[QuotationsPanelsCost]")]
		public double PanelCost
		{
			get { return this._PanelCost; }
			set
			{
				if (value != this._PanelCost)
				{
					this._PanelCost = value;

					NotifyPropertyChanged("PanelsCost");
					NotifyPropertyChanged("PanelPrice");
					NotifyPropertyChanged("PanelsPrice");
					NotifyPropertyChanged();
				}
			}
		}

		[DontWrite]
		[DontRead]
		public double PanelsCost
		{
			get { return Math.Round(this._PanelCost * this._PanelQty, 3); }
		}

		[DontWrite]
		[DontRead]
		public double PanelPrice
		{
			get { return Math.Round((this._PanelCost / (1 - this._PanelProfit / 100)), 3); }
		}

		[DontWrite]
		[DontRead]
		public double PanelsPrice
		{
			get { return Math.Round((this._PanelCost * this._PanelQty / (1 - this._PanelProfit / 100)), 3); }
		}

	}
}
