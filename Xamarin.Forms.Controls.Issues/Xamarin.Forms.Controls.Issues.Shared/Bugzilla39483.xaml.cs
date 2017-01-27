using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Bugzilla, 39483, "ListView Context Menu localization", PlatformAffected.iOS)]
	public partial class Bugzilla39483 : ContentPage
	{
		public Bugzilla39483()
		{
#if APP

			InitializeComponent();

			BindingContext = new DemoViewModel();

#endif
		}
	}

	public class DemoViewModel : ViewModelBase
	{
		List<string> _dataList;

		public DemoViewModel()
		{
			DataList = new List<string>();
			DataList.Add("Listenelement 1");
			DataList.Add("Listenelement 2");
			DataList.Add("Listenelement 3");
			DataList.Add("Listenelement 4");
		}

		public List<string> DataList
		{
			get { return _dataList; }
			set
			{
				_dataList = value;
				OnPropertyChanged();
			}
		}
	}
}