using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    public class ItemPurchased : INotifyPropertyChanged  //Job Orders Items (PurchaseDetails)
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
			set { if (value != this._Qty) { this._Qty = value; NotifyPropertyChanged(); NotifyPropertyChanged("RemainingQty"); } }
		}

		private double _PurchasedQty;
		public double PurchasedQty
		{
			get { return this._PurchasedQty; }
			set { if (value != this._PurchasedQty) { this._PurchasedQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("RemainingQty"); } }
		}

		private double _DamagedQty;
		public double DamagedQty
		{
			get { return this._DamagedQty; }
			set { if (value != this._DamagedQty) { this._DamagedQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("RemainingQty"); } }
		}

		private double _InOrderQty;
		public double InOrderQty
		{
			get { return this._InOrderQty; }
			set { if (value != this._InOrderQty) { this._InOrderQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("RemainingQty"); } }
		}

		public double RemainingQty
		{ get { return (Qty - PurchasedQty + DamagedQty - InOrderQty); } }
	}
}
