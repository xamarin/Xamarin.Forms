using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 31967, "Grid Layout on Bound RowDefinition")]
	public partial class Bugzilla31967 : ContentPage
	{
		public Bugzilla31967()
		{
			InitializeComponent();
			BindingContext = new Bugzilla31967Vm();
		}

		public class Bugzilla31967Vm : INotifyPropertyChanged
		{
			GridLength _toolbarHeight;

			public Command Fire
			{
				get { return new Command(() => ToolbarHeight = 50); }
			}

			public GridLength ToolbarHeight
			{
				get { return _toolbarHeight; }
				set
				{
					_toolbarHeight = value;
					OnPropertyChanged("ToolbarHeight");
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(string propertyName)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
#endif
}