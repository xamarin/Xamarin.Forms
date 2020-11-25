using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12429, "[Bug] Shell flyout items have a minimum height", PlatformAffected.iOS)]
	public partial class Issue12429 : TestShell
	{
		public Issue12429()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}
	}


	[Preserve(AllMembers = true)]
	class Issue12429Page : ContentPage
	{
		public Issue12429Page()
		{
			var label = new Label
			{
				Text = "The FlyoutItems shouldn't have any extra space. You should see 4 items",
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.Black,
				FontSize = 30,
				IsVisible = false
			};

			Content = label;
		}
	}
}