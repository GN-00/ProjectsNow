using System;
using ProjectsNow.Enums;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[JobOrder].[Panels]")]
    [WriteTable("[JobOrder].[Panels]")]
    public class JPanel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ID]
        [QPanelProperty]
        [JoinID("[Quotation].[QuotationsPanelsCost]", "[JobOrder].[PanelsDesignCost]", "[JobOrder].[PanelsCount(Closed)]", "[JobOrder].[PanelsCount(Delivered)]", "[JobOrder].[PanelsCount(Invoiced)]", "[JobOrder].[PanelsCount(Hold)]", "[JobOrder].[PanelsCount(Cancelled)]")]
        public int PanelID { get; set; }
        public int JobOrderID { get; set; }
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
        public int PanelQty { get; set; }




        private string _Status = Statuses.New.ToString();
        public string Status
        {
            get { return this._Status; }
            set { if (value != this._Status) { this._Status = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfCreation = DateTime.Today;
        public DateTime? DateOfCreation
        {
            get { return this._DateOfCreation; }
            set { if (value != this._DateOfCreation) { this._DateOfCreation = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfDesign;
        public DateTime? DateOfDesign
        {
            get { return this._DateOfDesign; }
            set { if (value != this._DateOfDesign) { this._DateOfDesign = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfSendingForApproval;
        public DateTime? DateOfSendingForApproval
        {
            get { return this._DateOfSendingForApproval; }
            set { if (value != this._DateOfSendingForApproval) { this._DateOfSendingForApproval = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfProduction;
        public DateTime? DateOfProduction
        {
            get { return this._DateOfProduction; }
            set { if (value != this._DateOfProduction) { this._DateOfProduction = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfClosing;
        public DateTime? DateOfClosing
        {
            get { return this._DateOfClosing; }
            set { if (value != this._DateOfClosing) { this._DateOfClosing = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfDelivery;
        public DateTime? DateOfDelivery
        {
            get { return this._DateOfDelivery; }
            set { if (value != this._DateOfDelivery) { this._DateOfDelivery = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfHolding;
        public DateTime? DateOfHolding
        {
            get { return this._DateOfHolding; }
            set { if (value != this._DateOfHolding) { this._DateOfHolding = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _DateOfCancellation;
        public DateTime? DateOfCancellation
        {
            get { return this._DateOfCancellation; }
            set { if (value != this._DateOfCancellation) { this._DateOfCancellation = value; NotifyPropertyChanged(); } }
        }


        private int _ClosedQty;
        [DontWrite]
        [Join("[JobOrder].[PanelsCount(Closed)]")]
        public int ClosedQty
        {
            get { return this._ClosedQty; }
            set
            {
                if (value != this._ClosedQty)
                {
                    this._ClosedQty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NotClosedQty");
                    NotifyPropertyChanged("ClosedToTotalQty");
                    NotifyPropertyChanged("NotClosedToTotalQty");

                    NotifyPropertyChanged("ReadyToCloseQty");
                    NotifyPropertyChanged("ReadyToDeliverQty");
                    NotifyPropertyChanged("ReadyToHoldQty");
                }
            }
        }
        [DontRead][DontWrite] public int NotClosedQty { get { return (PanelQty - ClosedQty); } }
        [DontRead][DontWrite] public int ReadyToCloseQty { get { return (PanelQty - ClosedQty - HoldQty - CancelledQty); } }
        [DontRead][DontWrite] public string ClosedToTotalQty { get { return $"{ClosedQty} / {PanelQty}"; } }
        [DontRead][DontWrite] public string NotClosedToTotalQty { get { return $"{NotClosedQty} / {PanelQty}"; } }


        private int _InvoicedQty;
        [DontWrite]
        [Join("[JobOrder].[PanelsCount(Invoiced)]")]
        public int InvoicedQty
        {
            get { return this._InvoicedQty; }
            set
            {
                if (value != this._InvoicedQty)
                {
                    this._InvoicedQty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NotInvoicedQty");
                    NotifyPropertyChanged("ReadyToInvoicedQty");
                    NotifyPropertyChanged("InvoicedToTotalQty");
                    NotifyPropertyChanged("ReadyToHoldQty");
                }
            }
        }
        [DontRead][DontWrite] public int NotInvoicedQty { get { return (PanelQty - InvoicedQty); } }
        [DontRead][DontWrite] public int ReadyToInvoicedQty { get { return (PanelQty - InvoicedQty - HoldQty - CancelledQty); } }
        [DontRead][DontWrite] public string InvoicedToTotalQty { get { return $"{InvoicedQty} / {PanelQty}"; } }


        private int _DeliveredQty;
        [DontWrite]
        [Join("[JobOrder].[PanelsCount(Delivered)]")]
        public int DeliveredQty
        {
            get { return this._DeliveredQty; }
            set
            {
                if (value != this._DeliveredQty)
                {
                    this._DeliveredQty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NotDeliveredQty");
                    NotifyPropertyChanged("DeliveredToTotalQty");

                    NotifyPropertyChanged("ReadyToCloseQty");
                    NotifyPropertyChanged("ReadyToDeliverQty");
                    NotifyPropertyChanged("ReadyToHoldQty");
                }
            }
        }
        [DontRead][DontWrite] public int NotDeliveredQty { get { return (PanelQty - DeliveredQty); } }
        [DontRead][DontWrite] public int ReadyToDeliverQty 
        {
            get
            {
                if (ClosedQty >= InvoicedQty) return (InvoicedQty - DeliveredQty);
                else return (ClosedQty - DeliveredQty);
            } 
        }
        [DontRead][DontWrite] public string DeliveredToTotalQty { get { return $"{DeliveredQty} / {PanelQty}"; } }


        private int _HoldQty;
        [DontWrite]
        [Join("[JobOrder].[PanelsCount(Hold)]")]
        public int HoldQty
        {
            get { return this._HoldQty; }
            set
            {
                if (value != this._HoldQty)
                {
                    this._HoldQty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NotHoldQty");

                    NotifyPropertyChanged("ReadyToCloseQty");
                    NotifyPropertyChanged("ReadyToInvoicedQty");
                    NotifyPropertyChanged("ReadyToDeliverQty");
                    NotifyPropertyChanged("ReadyToHoldQty");
                    NotifyPropertyChanged("PanelQtyView");
                }
            }
        }
        [DontRead][DontWrite] public int NotHoldQty { get { return (PanelQty - HoldQty); } }
        [DontRead][DontWrite] public int ReadyToHoldQty { get { return (NotClosedQty - HoldQty - CancelledQty); } }


        private int _CancelledQty;
        [DontWrite]
        [Join("[JobOrder].[PanelsCount(Cancelled)]")]
        public int CancelledQty
        {
            get { return this._CancelledQty; }
            set
            {
                if (value != this._CancelledQty)
                {
                    this._CancelledQty = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("NotCancelledQty");

                    NotifyPropertyChanged("ReadyToCloseQty");
                    NotifyPropertyChanged("ReadyToInvoicedQty");
                    NotifyPropertyChanged("ReadyToDeliverQty");
                    NotifyPropertyChanged("PanelQtyView"); 
                }
            }
        }
        [DontRead][DontWrite] public int NotCancelledQty { get { return (PanelQty - CancelledQty); } }

        [DontRead][DontWrite] public string PanelQtyView
        {
            get
            {
                string qty = (PanelQty - HoldQty - CancelledQty).ToString();
                if (HoldQty != 0) qty += $"/{HoldQty}H";
                if (CancelledQty != 0) qty += $"/{CancelledQty}C";
                return qty;
            }
        }

        private string _EnclosureType;
        public string EnclosureType
        {
            get { return this._EnclosureType; }
            set { if (value != this._EnclosureType) { this._EnclosureType = value; NotifyPropertyChanged(); } }
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

        public double PanelProfit { get; set; }

        private double _PanelEstimatedCost;
        [DontWrite]
        [Join("[Quotation].[QuotationsPanelsCost]")]
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
        [DontRead][DontWrite]public double PanelsEstimatedCost
        {
            get { return Math.Round(this._PanelEstimatedCost * this.PanelQty, 3); }
        }
        [DontRead][DontWrite]public double PanelEstimatedPrice
        {
            get { return Math.Round((this._PanelEstimatedCost / (1 - this.PanelProfit / 100)), 3); }
        }
        [DontRead][DontWrite]public double PanelsEstimatedPrice
        {
            get { return Math.Round((this._PanelEstimatedCost * this.PanelQty / (1 - this.PanelProfit / 100)), 3); }
        }

        private double _PanelDesignCost;
        [DontWrite]
        [Join("[JobOrder].[PanelsDesignCost]")]
        public double PanelDesignCost
        {
            get { return this._PanelDesignCost; }
            set
            {
                if (value != this._PanelDesignCost)
                {
                    this._PanelDesignCost = value;

                    NotifyPropertyChanged("PanelsDesignCost");
                    NotifyPropertyChanged("PanelDesignPrice");
                    NotifyPropertyChanged("PanelsDesignPrice");
                    NotifyPropertyChanged();
                }
            }
        }
        [DontWrite][DontRead]public double PanelsDesignCost
        {
            get { return Math.Round(this._PanelDesignCost * this.PanelQty, 3); }
        }
        [DontRead][DontWrite]public double PanelDesignPrice
        {
            get { return Math.Round((this._PanelDesignCost / (1 - this.PanelProfit / 100)), 3); }
        }
        [DontRead][DontWrite]public double PanelsDesignPrice
        {
            get { return Math.Round((this._PanelDesignCost * this.PanelQty / (1 - this.PanelProfit / 100)), 3); }
        }


        private double _PanelCost;
        //[Join("[JobOrder].[PanelsCost]")]
        [DontWrite]
        [DontRead]
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
            get { return Math.Round(this._PanelCost * this.PanelQty, 3); }
        }

        public int Revision { get; set; }

    }
}
