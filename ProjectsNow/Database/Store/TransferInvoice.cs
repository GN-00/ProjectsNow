using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database.Store
{
    [ReadTable("[Store].[TransferInvoices]")]
    [WriteTable("[Store].[TransferInvoices]")]
    class TransferInvoice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [ID] public int ID { get; set; }
        public int JobOrderID { get; set; }

        private int _Number;
        public int Number
        {
            get { return this._Number; }
            set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); } }
        }

        private int _Month;
        public int Month
        {
            get { return this._Month; }
            set { if (value != this._Month) { this._Month = value; NotifyPropertyChanged(); } }
        }

        private int _Year;
        public int Year
        {
            get { return this._Year; }
            set { if (value != this._Year) { this._Year = value; NotifyPropertyChanged(); } }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get { return this._Date; }
            set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
        }

        private string _Code;
        public string Code
        {
            get { return this._Code; }
            set { if (value != this._Code) { this._Code = value; NotifyPropertyChanged(); } }
        }

    }
}
