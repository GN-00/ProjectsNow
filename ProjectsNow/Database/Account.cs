using System;
using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
	[WriteTable("[Finance].[CompanyAccounts]")]
    public class Account : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ID { get; set; }
		public int BankID { get; set; }

		private string _Name;
		public string Name
		{
			get { return this._Name; }
			set { if (value != this._Name) { this._Name = value; NotifyPropertyChanged();} }
		}


		private DateTime? _CreateDate;
		public DateTime? CreateDate
		{
			get { return this._CreateDate; }
			set { if (value != this._CreateDate) { this._CreateDate = value; NotifyPropertyChanged(); } }
		}

		[DontWrite]public double Balance { get; set; } 
	}
}
