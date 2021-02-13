using System;
using ProjectsNow.Enums;
using System.Reflection;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[Quotation].[Quotations]")]
    [WriteTable("[Quotation].[Quotations]")]
    public class Quotation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ID] public int QuotationID { get; set; }
        public string QuotationCode { get; set; }
        public int QuotationNumber { get; set; }
        public int QuotationRevise { get; set; } = 0;
        public DateTime? QuotationReviseDate { get; set; }
        public int QuotationYear { get; set; } = DateTime.Today.Year;
        public int QuotationMonth { get; set; } = DateTime.Today.Month;

        private string _QuotationStatus = Statuses.Running.ToString();
        public string QuotationStatus
        {
            get { return this._QuotationStatus; }
            set { if (value != this._QuotationStatus) { this._QuotationStatus = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _SubmitDate;
        public DateTime? SubmitDate
        {
            get { return this._SubmitDate; }
            set { if (value != this._SubmitDate) { this._SubmitDate = value; NotifyPropertyChanged(); } }
        }
        public string PowerVoltage { get; set; } = "220-380V";
        public string Phase { get; set; } = "3P + N";
        public string Frequency { get; set; } = "60Hz";
        public string NetworkSystem { get; set; } = "AC";
        public string ControlVoltage { get; set; } = "230V AC";
        public string TinPlating { get; set; } = "Bare Copper";
        public string NeutralSize { get; set; } = "Full of Phase";
        public string EarthSize { get; set; } = "Half of Neutral";
        public string EarthingSystem { get; set; } = "TNS";

        private double _Discount;
        public double Discount
        {
            get { return this._Discount; }
            set
            {
                if (value != this._Discount)
                {
                    this._Discount = value;

                    NotifyPropertyChanged("QuotationPrice");
                    NotifyPropertyChanged("QuotationDiscountValue");
                    NotifyPropertyChanged("QuotationPriceWithVAT");
                    NotifyPropertyChanged("VATPrice");
                    NotifyPropertyChanged("VATPercentage");
                    NotifyPropertyChanged("DiscountPercentage");
                    NotifyPropertyChanged();
                }
            }
        }
        public double VAT { get; set; } = 0.15;
        [DontWrite]
        [DontRead]
        public double VATPercentage
        {
            get { return this.VAT * 100; }
        }

        //Join Data
        private double _QuotationCost;
        [DontWrite]
        public double QuotationCost
        {
            get { return this._QuotationCost; }
            set
            {
                if (value != this._QuotationCost)
                {
                    this._QuotationCost = value;

                    NotifyPropertyChanged("QuotationPrice");
                    NotifyPropertyChanged("QuotationDiscountValue");
                    NotifyPropertyChanged("QuotationPriceWithVAT");
                    NotifyPropertyChanged("VATPrice");
                    NotifyPropertyChanged("VATPercentage");
                    NotifyPropertyChanged("DiscountPercentage");
                    NotifyPropertyChanged();
                }
            }
        }

        [DontWrite]
        [DontRead]
        public double QuotationPrice
        {
            get { return Math.Round(this.QuotationCost * (1 - Discount / 100), 3); }
        }
        [DontWrite]
        [DontRead]
        public double VATPrice
        {
            get { return Math.Round((this.VAT) * this.QuotationPrice, 3); }
        }
        [DontWrite]
        [DontRead]
        public double QuotationDiscountValue
        {
            get { return Math.Round(this.QuotationCost * (Discount / 100), 3); }
        }
        [DontWrite]
        [DontRead]
        public double QuotationPriceWithVAT
        {
            get { return Math.Round((this.VAT + 1) * this.QuotationCost * (1 - Discount / 100), 3); }
        }

        //Inquiry Data
        public int InquiryID { get; set; }
        [DontWrite] public int CustomerID { get; set; }
        [DontWrite] public int ConsultantID { get; set; }
        [DontWrite] public int EstimationID { get; set; }
        [DontWrite] public int SalesmanID { get; set; }

        private string _RegisterCode;
        [DontWrite]
        public string RegisterCode
        {
            get { return this._RegisterCode; }
            set { if (value != this._RegisterCode) { this._RegisterCode = value; NotifyPropertyChanged(); } }
        }

        private string _ProjectName;
        [DontWrite]
        public string ProjectName
        {
            get { return this._ProjectName; }
            set { if (value != this._ProjectName) { this._ProjectName = value; NotifyPropertyChanged(); } }
        }

        private DateTime _RegisterDate = DateTime.Today;
        [DontWrite]
        public DateTime RegisterDate
        {
            get { return this._RegisterDate; }
            set { if (value != this._RegisterDate) { this._RegisterDate = value; NotifyPropertyChanged(); } }
        }

        private DateTime _DuoDate = DateTime.Today.AddDays(7);
        [DontWrite]
        public DateTime DuoDate
        {
            get { return this._DuoDate; }
            set { if (value != this._DuoDate) { this._DuoDate = value; NotifyPropertyChanged(); } }
        }

        private string _Priority = "Normal";
        [DontWrite]
        public string Priority
        {
            get { return this._Priority; }
            set { if (value != this._Priority) { this._Priority = value; NotifyPropertyChanged(); } }
        }

        [DontWrite] public int RegisterNumber { get; set; }
        [DontWrite] public int RegisterMonth { get; set; }
        [DontWrite] public int RegisterYear { get; set; }
        [DontWrite] public string DeliveryCondition { get; set; }

        //Inquiry Data Join
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


        public Quotation() { }
        public Quotation(Inquiry inquiry)
        {
            foreach (PropertyInfo property in typeof(Inquiry).GetProperties())
            {
                if (property.SetMethod != null)
                    this.GetType().GetProperty(property.Name).SetValue(this, typeof(Inquiry).GetProperty(property.Name).GetValue(inquiry));
            }
        }
    }

    public class QuotationsYear
    {
        public int Year { get; set; }
    }


}
