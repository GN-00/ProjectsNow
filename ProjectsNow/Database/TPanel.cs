using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    public class TPanel : INotifyPropertyChanged //TransactionPanel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int TransactionID { get; set; }
        public int JobOrderID { get; set; }
        public int PanelID { get; set; }
        public string Reference { get; set; }

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

        private DateTime? _Date;
        public DateTime? Date
        {
            get { return this._Date; }
            set { if (value != this._Date) { this._Date = value; NotifyPropertyChanged(); } }
        }
        public int Qty { get; set; }

        private string _EnclosureType;
        public string EnclosureType
        {
            get { return this._EnclosureType; }
            set { if (value != this._EnclosureType) { this._EnclosureType = value; NotifyPropertyChanged(); } }
        }
        public string Note { get; set; }
    }
}
