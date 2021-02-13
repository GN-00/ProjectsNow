using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    public class Category : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private string _Name;
		public string Name
		{
			get { return this._Name; }
			set { if (value != this._Name) { this._Name = value; NotifyPropertyChanged(); } }
		}

		private double _Discount;
		public double Discount
		{
			get { return this._Discount; }
			set { if (value != this._Discount) { this._Discount = value; NotifyPropertyChanged(); } }
		}

		private int _TotalItems;
		public int TotalItems
		{
			get { return this._TotalItems; }
			set { if (value != this._TotalItems) { this._TotalItems = value; NotifyPropertyChanged(); } }
		}
	}
}
