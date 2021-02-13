using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [WriteTable("[Purchase].[Transactions]")]
    public class CompanyPOTransaction : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ID { get; set; }
		[DontRead][DontWrite] public int SN { get; set; }
		public int PurchaseOrderID { get; set; }

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

		[DontWrite] public string PartNumber
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
			set { if (value != this._Qty) { this._Qty = value; NotifyPropertyChanged(); NotifyPropertyChanged("TotalCost"); } }
		}

		private double _Cost;
		public double Cost
		{
			get { return this._Cost; }
			set { if (value != this._Cost) { this._Cost = value; NotifyPropertyChanged(); NotifyPropertyChanged("TotalCost"); } }
		}

		[DontWrite] public double TotalCost
        {
            get { return (_Qty * _Cost); }
        }
        public int? Reference { get; set; }

		[DontWrite][DontRead] public bool Received { get { return Reference == null ? false : true; } }
    }
}
