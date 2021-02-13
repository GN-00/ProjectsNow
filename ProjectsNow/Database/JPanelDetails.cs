using System;
using ProjectsNow.Enums;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[JobOrder].[Panels]")]
    [WriteTable("[JobOrder].[Panels]")]
    public class JPanelDetails : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int? _PanelSN;
        private string _PanelName;
        private string _PanelType;
        private string _Status = Statuses.New.ToString();
        private DateTime? _DateOfCreation = DateTime.Today;
        private string _EnclosureIP;
        private double? _EnclosureWidth;
        private double? _EnclosureDepth;
        private double? _EnclosureHeight;
        private string _EnclosureInstallation;
        private string _EnclosureType;
        private string _EnclosureName;
        
        [ID][QPanelProperty][JoinID("[Quotation].[QuotationsPanelsCost]")] public int PanelID { get; set; }
        public int JobOrderID { get; set; }
        [QPanelProperty] public string PurchaseOrdersNumber { get; set; }
        [QPanelProperty] public int? PanelSN
        {
            get { return this._PanelSN; }
            set { if (value != this._PanelSN) { this._PanelSN = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public string PanelName
        {
            get { return this._PanelName; }
            set { if (value != this._PanelName) { this._PanelName = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty]  public string PanelType
        {
            get { return this._PanelType; }
            set { if (value != this._PanelType) { this._PanelType = value; NotifyPropertyChanged(); } }
        }
        public string Status
        {
            get { return this._Status; }
            set { if (value != this._Status) { this._Status = value; NotifyPropertyChanged(); } }
        }
        public DateTime? DateOfCreation
        {
            get { return this._DateOfCreation; }
            set { if (value != this._DateOfCreation) { this._DateOfCreation = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public int PanelQty { get; set; }
        [QPanelProperty] public double PanelProfit { get; set; }
        [QPanelProperty] public string EnclosureName
        {
            get { return this._EnclosureName; }
            set { if (value != this._EnclosureName) { this._EnclosureName = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public string EnclosureType
        {
            get { return this._EnclosureType; }
            set { if (value != this._EnclosureType) { this._EnclosureType = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public string EnclosureInstallation
        {
            get { return this._EnclosureInstallation; }
            set { if (value != this._EnclosureInstallation) { this._EnclosureInstallation = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public double? EnclosureHeight
        {
            get { return this._EnclosureHeight; }
            set { if (value != this._EnclosureHeight) { this._EnclosureHeight = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public double? EnclosureWidth
        {
            get { return this._EnclosureWidth; }
            set { if (value != this._EnclosureWidth) { this._EnclosureWidth = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public double? EnclosureDepth
        {
            get { return this._EnclosureDepth; }
            set { if (value != this._EnclosureDepth) { this._EnclosureDepth = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public string EnclosureIP
        {
            get { return this._EnclosureIP; }
            set { if (value != this._EnclosureIP) { this._EnclosureIP = value; NotifyPropertyChanged(); } }
        }
        [QPanelProperty] public string EnclosureLocation { get; set; }
        [QPanelProperty] public string EnclosureColor { get; set; }
        [QPanelProperty] public string EnclosureMetalType { get; set; }
        [QPanelProperty] public string EnclosureForm { get; set; }
        [QPanelProperty] public string EnclosureDoor { get; set; }
        [QPanelProperty] public string EnclosureFunctional { get; set; }

        [QPanelProperty] public string Source { get; set; }
        [QPanelProperty] public double? Icu { get; set; }
        [QPanelProperty] public string Frequency { get; set; }
        [QPanelProperty] public string PowerSupplyOperation { get; set; }
        [QPanelProperty] public string EarthingSystem { get; set; }
        [QPanelProperty] public string DirectCurrent { get; set; }

        [QPanelProperty] public string Busbar { get; set; }
        [QPanelProperty] public string BusbarHorizontal { get; set; }
        [QPanelProperty] public string BusbarVertical { get; set; }
        [QPanelProperty] public string NeutralSize { get; set; }
        [QPanelProperty] public string EarthSize { get; set; }

        [QPanelProperty] public string SignallingVoltage { get; set; }
        [QPanelProperty] public string SignallingSource { get; set; }
        [QPanelProperty] public string ControlVoltage { get; set; }
        [QPanelProperty] public string ControlSource { get; set; }
        [QPanelProperty] public string SensorsVoltage { get; set; }
        [QPanelProperty] public string SensorsSource { get; set; }
        [QPanelProperty] public string LightingVoltage { get; set; }
        [QPanelProperty] public string LightingSource { get; set; }

        [QPanelProperty] public string ClimateType { get; set; }
        [QPanelProperty] public string AtmosphereType { get; set; }
        [QPanelProperty] public string PollutionRisks { get; set; }
        [QPanelProperty] public double? AmbientTemperature { get; set; }
        [QPanelProperty] public double? RelativeHumidity { get; set; }
        [QPanelProperty] public double SeaLevel { get; set; }

        [QPanelProperty] public string IndicationType { get; set; }
        [QPanelProperty] public string IndicationSize { get; set; }
        [QPanelProperty] public string IndicationOther { get; set; }
        [QPanelProperty] public string MeterRelay { get; set; }

        [QPanelProperty] public bool IncomingFixed { get; set; }
        [QPanelProperty] public bool IncomingPlugIn { get; set; }
        [QPanelProperty] public bool IncomingDrawout { get; set; }
        [QPanelProperty] public bool IncomingMotorized { get; set; }
        [QPanelProperty] public bool IncomingBehindDoor { get; set; }
        [QPanelProperty] public bool IncomingThroughPlate { get; set; }
        [QPanelProperty] public bool IncomingInterLock { get; set; }
        [QPanelProperty] public bool IncomingPadlocking { get; set; }
        [QPanelProperty] public bool IncomingShutter { get; set; }
        [QPanelProperty] public bool IncomingThroughDoor { get; set; }
        [QPanelProperty] public bool IncomingThroughCover { get; set; }
        [QPanelProperty] public bool IncomingDirect { get; set; }
        [QPanelProperty] public bool IncomingTerminalBlocks { get; set; }
        [QPanelProperty] public bool IncomingBusbarLinks { get; set; }
        [QPanelProperty] public bool IncomingFront { get; set; }
        [QPanelProperty] public bool IncomingRear { get; set; }
        [QPanelProperty] public bool IncomingLeftRight { get; set; }
        [QPanelProperty] public bool IncomingTopCables { get; set; }
        [QPanelProperty] public bool IncomingTopBusduct { get; set; }
        [QPanelProperty] public bool IncomingBottomCables { get; set; }
        [QPanelProperty] public bool IncomingScrews { get; set; }
        [QPanelProperty] public bool IncomingGlandPlate { get; set; }
        [QPanelProperty] public bool IncomingCableGland { get; set; }
        [QPanelProperty] public bool IncomingShrouding { get; set; }

        [QPanelProperty] public bool OutgoingFixed { get; set; }
        [QPanelProperty] public bool OutgoingPlugIn { get; set; }
        [QPanelProperty] public bool OutgoingDrawout { get; set; }
        [QPanelProperty] public bool OutgoingMotorized { get; set; }
        [QPanelProperty] public bool OutgoingBehindDoor { get; set; }
        [QPanelProperty] public bool OutgoingThroughPlate { get; set; }
        [QPanelProperty] public bool OutgoingInterLock { get; set; }
        [QPanelProperty] public bool OutgoingPadlocking { get; set; }
        [QPanelProperty] public bool OutgoingShutter { get; set; }
        [QPanelProperty] public bool OutgoingThroughDoor { get; set; }
        [QPanelProperty] public bool OutgoingThroughCover { get; set; }
        [QPanelProperty] public bool OutgoingDirect { get; set; }
        [QPanelProperty] public bool OutgoingTerminalBlocks { get; set; }
        [QPanelProperty] public bool OutgoingBusbarLinks { get; set; }
        [QPanelProperty] public bool OutgoingFront { get; set; }
        [QPanelProperty] public bool OutgoingRear { get; set; }
        [QPanelProperty] public bool OutgoingLeftRight { get; set; }
        [QPanelProperty] public bool OutgoingTopCables { get; set; }
        [QPanelProperty] public bool OutgoingTopBusduct { get; set; }
        [QPanelProperty] public bool OutgoingBottomCables { get; set; }
        [QPanelProperty] public bool OutgoingScrews { get; set; }
        [QPanelProperty] public bool OutgoingGlandPlate { get; set; }
        [QPanelProperty] public bool OutgoingCableGland { get; set; }
        [QPanelProperty] public bool OutgoingShrouding { get; set; }

        [QPanelProperty] public string PushButtonON { get; set; }
        [QPanelProperty] public string PushButtonOFF { get; set; }
        [QPanelProperty] public string PushButtonReset { get; set; }
        [QPanelProperty] public string SignallingON { get; set; }
        [QPanelProperty] public string SignallingOFF { get; set; }
        [QPanelProperty] public string SignallingTrip { get; set; }

        [QPanelProperty] public string ExternalLabelType { get; set; }
        [QPanelProperty] public string ExternalLabelFixeing { get; set; }
        [QPanelProperty] public string ExternalLabelLanguage { get; set; }
        [QPanelProperty] public string InternalLabelType { get; set; }
        [QPanelProperty] public string InternalLabelFixeing { get; set; }
        [QPanelProperty] public string InternalLabelLanguage { get; set; }
        [QPanelProperty] public string EquipmentLabelType { get; set; }
        [QPanelProperty] public string EquipmentLabelFixeing { get; set; }
        [QPanelProperty] public string EquipmentLabelLanguage { get; set; }
        [QPanelProperty] public string LabelBackground { get; set; }
        [QPanelProperty] public string LabelFont { get; set; }

        [QPanelProperty] public double? AuxiliaryVoltageSection { get; set; }
        [QPanelProperty] public string AuxiliaryVoltageColor { get; set; }
        [QPanelProperty] public string AuxiliaryVoltageType { get; set; }
        [QPanelProperty] public double? AuxiliaryCurrentSection { get; set; }
        [QPanelProperty] public string AuxiliaryCurrentColor { get; set; }
        [QPanelProperty] public string AuxiliaryCurrentType { get; set; }

        [QPanelProperty] public string ApparatusDefind { get; set; }
        [QPanelProperty] public string Weight { get; set; }
        [QPanelProperty] public bool ForInformation { get; set; }
        [QPanelProperty] public bool ForProduction { get; set; }
        [QPanelProperty] public bool ForApproval { get; set; }
        [QPanelProperty] public bool AsManufactured { get; set; }
        [QPanelProperty] public string Remarks { get; set; }

        private double _PanelEstimatedCost;
        [DontWrite][Join("[Quotation].[QuotationsPanelsCost]")]
        public double PanelEstimatedCost
        {
            get { return this._PanelEstimatedCost; }
            set
            {
                if (value != this._PanelEstimatedCost)
                {
                    this._PanelEstimatedCost = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("PanelsEstimatedCost");
                    NotifyPropertyChanged("PanelEstimatedPrice");
                    NotifyPropertyChanged("PanelsEstimatedPrice");
                }
            }
        }
        [DontRead][DontWrite] public double PanelsEstimatedCost
        {
            get { return Math.Round(this._PanelEstimatedCost * this.PanelQty, 3); }
        }
        [DontRead][DontWrite] public double PanelEstimatedPrice
        {
            get { return Math.Round((this._PanelEstimatedCost / (1 - this.PanelProfit / 100)), 3); }
        }
        [DontRead][DontWrite] public double PanelsEstimatedPrice
        {
            get { return Math.Round((this._PanelEstimatedCost * this.PanelQty / (1 - this.PanelProfit / 100)), 3); }
        }

        private int? _DrawingNo;
        public int? DrawingNo
        {
            get { return this._DrawingNo; }
            set { if (value != this._DrawingNo) { this._DrawingNo = value; NotifyPropertyChanged(); } }
        }
    }
}
