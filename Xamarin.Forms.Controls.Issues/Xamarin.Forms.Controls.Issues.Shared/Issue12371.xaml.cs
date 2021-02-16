using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 12371, "[Bug] OnPlatform crashes app on iOS if no default value given",
		PlatformAffected.iOS)]
	public partial class Issue12371 : TestContentPage
	{
		public Issue12371()
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