using System.ComponentModel;
using ProjectsNow.Attributes;
using System.Runtime.CompilerServices;

namespace ProjectsNow.Database
{
    [ReadTable("[Customer].[Consultants]")]
    [WriteTable("[Customer].[Consultants]")]
	public class Consultant : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[ID] public int ConsultantID { get; set; }

		private string _ConsultantName;
		public string ConsultantName
		{
			get { return this._ConsultantName; }
			set { if (value != this._ConsultantName) { this._ConsultantName = value; NotifyPropertyChanged(); } }
		}

		private string _Address;
		public string Address
		{
			get { return this._Address; }
			set { if (value != this._Address) { this._Address = value; NotifyPropertyChanged(); } }
		}

		private string _Mobile;
		public string Mobile
		{
			get { return this._Mobile; }
			set { if (value != this._Mobile) { this._Mobile = value; NotifyPropertyChanged(); } }
		}

		private string _Email;
		public string Email
		{
			get { return this._Email; }
			set { if (value != this._Email) { this._Email = value; NotifyPropertyChanged(); } }
		}

		private string _Company;
		public string Company
		{
			get { return this._Company; }
			set { if (value != this._Company) { this._Company = value; NotifyPropertyChanged(); } }
		}

		private string _Website;
		public string Website
		{
			get { return this._Website; }
			set { if (value != this._Website) { this._Website = value; NotifyPropertyChanged(); } }
		}

		private string _Job;
		public string Job
		{
			get { return this._Job; }
			set { if (value != this._Job) { this._Job = value; NotifyPropertyChanged(); } }
		}

		private string _Note;
		public string Note
		{
			get { return this._Note; }
			set { if (value != this._Note) { this._Note = value; NotifyPropertyChanged(); } }
		}

	}
}
