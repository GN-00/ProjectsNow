using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[Store].[Invoices]")]
    [WriteTable("[Store].[Invoices]")]
    public class SupplierInvoice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [ID] public int ID { get; set; }
        public int JobOrderID { get; set; }
        public int SupplierID { get; set; }

        private string _Number;
        public string Number
        {
            get { return this._Number; }
            set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); } }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get { return this._Date; }
            set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
        }

        private string _SupplierCode;
        [DontWrite]public string SupplierCode
        {
            get { return this._SupplierCode; }
            set { if (value != this._SupplierCode) { this._SupplierCode = value; NotifyPropertyChanged(); } }
        }

        private string _SupplierName;
        [DontWrite]public string SupplierName
        {
            get { return this._SupplierName; }
            set { if (value != this._SupplierName) { this._SupplierName = value; NotifyPropertyChanged(); } }
        }
    }
}
