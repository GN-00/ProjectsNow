using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[JobOrder].[ItemsPosting]")]
	public class CItem : INotifyPropertyChanged //Closing Item
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public int JobOrderID { get; set; }
		public int PanelID { get; set; }
		public int PanelTransactionID { get; set; }

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

		[DontRead]
		[DontWrite]
		public string PartNumber
		{ get { return ($"{Category}{Code}"); } }

		private string _Description;
		public string Description
		{
			get { return this._Description; }
			set { if (value != this._Description) { this._Description = value; NotifyPropertyChanged(); } }
		}

		public string Unit { get; set; }
		public double PanelQty { get; set; }
		public double ItemQty { get; set; }
		public double TotalQty { get; set; }

		private double _StockQty;
		public double StockQty
		{
			get { return this._StockQty; }
			set { if (value != this._StockQty) { this._StockQty = value; NotifyPropertyChanged(); } }
		}

		private double _UsedQty;
		public double UsedQty
		{
			get { return this._UsedQty; }
			set { if (value != this._UsedQty) { this._UsedQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("RemainingQty"); } }
		}

		[DontRead] public double RemainingQty { get { return (TotalQty - UsedQty); } }

		private double _PanelToPostQty;
		[DontRead]
		public double PanelToPostQty
		{
			get { return this._PanelToPostQty; }
			set { if (value != this._PanelToPostQty) { this._PanelToPostQty = value; NotifyPropertyChanged(); NotifyPropertyChanged("ItemToPostQty"); NotifyPropertyChanged("IsReady"); } }
		}

		[DontRead] public double? ItemToPostQty
		{ 
			get 
			{
				if (ItemQty * PanelToPostQty != 0) return (ItemQty * PanelToPostQty);
				else return null;
			}
		}
		
		[DontRead]
		public bool? IsReady
		{
			get
			{
				if (ItemToPostQty <= StockQty)
					return true;
				else if (ItemToPostQty > StockQty)
					return false;
				else
					return null;
			}
		}



	}
}
