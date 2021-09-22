using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11957, "[Bug] LinearGradientBrush with Ellipse/Shapes causes memory leak/problems", PlatformAffected.Android)]
	public partial class Issue11957 : TestContentPage
	{
		public Issue11957()
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