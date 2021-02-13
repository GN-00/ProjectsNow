using ProjectsNow.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    public class ItemStock : INotifyPropertyChanged  //Job Orders Items (Stock Avg)
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public int JobOrderID { get; set; }

		private string _Category;
		public string Category
		{
			get { return this._Category; }
			set { if (value != this._Category) { this._Category = value; NotifyPropertyChanged(); NotifyPropertyChanged("PartNumber"); } }
		}

		private string _Code;
		public string Code
		{
			get { return this._Code; }
			set { if (value != this._Code) { this._Code = value; NotifyPropertyChanged(); NotifyPropertyChanged("PartNumber"); } }
		}

		public string PartNumber
		{ get { return ($"{Category}{Code}"); } }

		private string _Description;
		public string Description
		{
			get { return this._Description; }
			set { if (value != this._Description) { this._Description = value; NotifyPropertyChanged(); } }
		}

		private string _Unit;
		public string Unit
		{
			get { return this._Unit; }
			set { if (value != this._Unit) { this._Unit = value; NotifyPropertyChanged(); } }
		}

		private double _Qty;
		public double Qty
		{
			get { return this._Qty; }
			set { if (value != this._Qty) { this._Qty = value; NotifyPropertyChanged(); } }
		}

		private double _AvgCost;
		[DontWrite]public double AvgCost
		{
			get { return this._AvgCost; }
			set { if (value != this._AvgCost) { this._AvgCost = value; NotifyPropertyChanged(); } }
		}

		[DontRead][DontWrite]public double TotalAvgCost
		{
			get { return (Qty * AvgCost); }
		}

		private string _Brand;
		public string Brand
		{
			get { return this._Brand; }
			set { if (value != this._Brand) { this._Brand = value; NotifyPropertyChanged(); } }
		}
	}
}
