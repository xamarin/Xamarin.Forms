using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13058, "[Bug] GradientBrush not working with Material on Android", PlatformAffected.Android)]
	public partial class Issue13058 : ContentPage
	{
		public Issue13058()
		{
#if APP
			InitializeComponent();
#endif
		}
	}
}