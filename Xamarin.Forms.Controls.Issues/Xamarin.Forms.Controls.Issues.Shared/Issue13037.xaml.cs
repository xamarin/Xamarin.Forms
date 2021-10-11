using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13037, "[Bug] Placeholder text in Editor control does not wrap text on UWP",
		PlatformAffected.UWP)]
	public partial class Issue13037 : TestContentPage
	{
		public Issue13037()
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