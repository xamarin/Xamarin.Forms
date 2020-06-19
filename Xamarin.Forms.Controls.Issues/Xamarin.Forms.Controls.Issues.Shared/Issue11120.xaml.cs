using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11120, "[iOS] Opacity on Frame behavior change in 4.5", PlatformAffected.iOS)]
	public partial class Issue11120 : ContentPage
	{
		public Issue11120()
		{
#if APP
			InitializeComponent();
#endif

		}
	}
}