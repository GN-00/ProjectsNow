using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[ReadTable("[Store].[Transactions]")]
	[WriteTable("[Store].[Transactions]")]
	public class ItemTransaction : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		[ID]public int ID { get; set; }
		public int JobOrderID { get; set; }
		public int? PanelID { get; set; }
		public int? PanelTransactionID { get; set; }
		public int InvoiceID { get; set; }
		public int TransferInvoiceID { get; set; }


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

		[DontRead][DontWrite]public string PartNumber
		{ get { return ($"{Category}{Code}"); } }

		private string _Description;
		public string Description
		{
			get { return this._Description; }
			set { if (value != this._Description) { this._Description = value; NotifyPropertyChanged(); } }
		}

		public int? Reference { get; set; }
		
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
			set 
			{ 
				if (value != this._Qty)
				{ 
					this._Qty = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("FinalQty");
					NotifyPropertyChanged("TotalCost");
				}
			}
		}

		private double _UsedQty;
		[DontWrite]public double UsedQty
		{
			get { return this._UsedQty; }
			set
			{
				if (value != this._UsedQty)
				{
					this._UsedQty = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("FinalQty");
					NotifyPropertyChanged("TotalCost");
				}
			}
		}

		private double _DamagedQty;
		[DontWrite]
		public double DamagedQty
		{
			get { return this._DamagedQty; }
			set
			{
				if (value != this._DamagedQty)
				{
					this._DamagedQty = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("FinalQty");
					NotifyPropertyChanged("TotalCost");
				}
			}
		}

		private double _TransferredQty;
		[DontWrite]public double TransferredQty
		{
			get { return this._TransferredQty; }
			set 
			{ if (value != this._TransferredQty)
				{
					this._TransferredQty = value; 
					NotifyPropertyChanged();
					NotifyPropertyChanged("FinalQty");
					NotifyPropertyChanged("TotalCost"); 
				}
			}
		}

		[DontRead][DontWrite]public double FinalQty { get { return (Qty - UsedQty - DamagedQty - TransferredQty); } }

		private double _Cost;
		public double Cost
		{
			get { return this._Cost; }
			set { if (value != this._Cost) { this._Cost = value; NotifyPropertyChanged(); NotifyPropertyChanged("TotalCost"); } }
		}
		[DontRead][DontWrite]public double TotalCost { get { return Cost * FinalQty; } }
		public DateTime Date { get; set; }
		public string Type { get; set; }
		public string Source { get; set; }
	}
}
