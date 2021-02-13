using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database.Suppliers
{
    [ReadTable("[Store].[SuppliersContacts]")]
    [WriteTable("[Store].[SuppliersContacts]")]
    public class Contact : INotifyPropertyChanged //[Suppliers Contacts]
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [ID] public int ID { get; set; }
        public int SupplierID { get; set; }

        private string _Name;
        public string Name
        {
            get { return this._Name; }
            set { if (value != this._Name) { this._Name = value; NotifyPropertyChanged(); } }
        }

        private string _Mobile;
        public string Mobile
        {
            get { return this._Mobile; }
            set { if (value != this._Mobile) { this._Mobile = value; NotifyPropertyChanged(); } }
        }
    }
}
