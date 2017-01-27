using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2288, "ToolbarItem.Text change", PlatformAffected.iOS | PlatformAffected.Android)]
	public partial class Issue2288 : ContentPage
	{
		int _count = 0;

		string _mainText;

		public Issue2288()
		{
			InitializeComponent();
			MainText = "initial";
			MainTextCommand = new Command(o => { MainText = "changed " + _count++; });

			BindingContext = this;
		}

		public string MainText
		{
			get { return _mainText; }
			set
			{
				_mainText = value;
				OnPropertyChanged();
			}
		}

		public Command MainTextCommand { get; set; }
	}
#endif
}