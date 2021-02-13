using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[Quotation].[Terms&Conditions]")]
    [WriteTable("[Quotation].[Terms&Conditions]")]
    public class Term : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [ID] public int TermID { get; set; }
        public int QuotationID { get; set; }
        public int Sort { get; set; }

        private string _Condition;
        public string Condition
        {
            get { return this._Condition; }
            set { if (value != this._Condition) { this._Condition = value; NotifyPropertyChanged(); } }
        }
        public string ConditionType { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefault { get; set; }

    }
}
