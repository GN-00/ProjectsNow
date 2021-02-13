using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[JobOrder].[PurchaseOrders]")]
    [WriteTable("[JobOrder].[PurchaseOrders]")]
    public class PurchaseOrder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ID] public int ID { get; set; }
        public int JobOrderID { get; set; }

        private string _Number;
        public string Number
        {
            get { return this._Number; }
            set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); } }
        }

        private DateTime? _Date = DateTime.Today;
        public DateTime? Date
        {
            get { return this._Date; }
            set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
        }

        [DontWrite]
        [DontRead]
        public double Cost { get; set; } = 56565.666; //need work
        [DontWrite]
        [DontRead]
        public double Discount { get; set; } = 0; //need work
        [DontWrite]
        [DontRead]
        public double VAT { get; set; } = 15;//need work

        [DontWrite]
        [DontRead]
        public double Price
        {
            get { return Math.Round(this.Cost * (1 - Discount / 100), 3); }
        }
        [DontWrite]
        [DontRead]
        public double DiscountValue
        {
            get { return Math.Round(this.Cost * (Discount / 100), 3); }
        }
        [DontWrite]
        [DontRead]
        public double PriceWithVAT
        {
            get { return Math.Ceiling((this.VAT + 1) * this.Cost * (1 - Discount / 100)); }
        }
        [DontWrite]
        [DontRead]
        public string TextPrice
        {
            get { return "Saudi Riyals " + DataInput.Input.NumberToWords((int)Math.Ceiling(PriceWithVAT)) + " Only."; }
        }

    }
}
