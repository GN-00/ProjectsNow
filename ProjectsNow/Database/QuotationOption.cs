using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[Quotation].[QuotationsOptions]")]
    [WriteTable("[Quotation].[QuotationsOptions]")]
    public class QuotationOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [ID] public int ID { get; set; }
        public int QuotationID { get; set; }

        private int _Number;
        public int Number
        {
            get { return this._Number; }
            set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); NotifyPropertyChanged(); } }
        }

        [DontRead]
        [DontWrite]
        public string Code
        {
            get
            {
                return (((char)(64 + Number)).ToString());
            }
        }

        private string _Name;
        public string Name
        {
            get { return this._Name; }
            set { if (value != this._Name) { this._Name = value; NotifyPropertyChanged(); } }
        }

    }
}
