using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12896, "[Bug] Swipeview in Listview alters Listview Item Margin functionality on Android",
		PlatformAffected.Android)]
	public partial class Issue12896 : TestContentPage
	{
		public Issue12896()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}
	}
}