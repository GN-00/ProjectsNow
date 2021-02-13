using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [WriteTable("[Finance].[BankAccounts]")]
    public class BankAccount : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ID] public int ID { get; set; }

        private string _Name;
        public string Name
        {
            get { return this._Name; }
            set { if (value != this._Name) { this._Name = value; NotifyPropertyChanged(); } }
        }
        private string _Number;
        public string Number
        {
            get { return this._Number; }
            set { if (value != this._Number) { this._Number = value; NotifyPropertyChanged(); } }
        }
        private string _IBAN;
        public string IBAN
        {
            get { return this._IBAN; }
            set { if (value != this._IBAN) { this._IBAN = value; NotifyPropertyChanged(); } }
        }
    }
}
