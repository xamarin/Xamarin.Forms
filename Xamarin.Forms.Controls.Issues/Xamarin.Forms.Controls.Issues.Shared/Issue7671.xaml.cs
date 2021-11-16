using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 7671, "Check ControlTemplate rendering", PlatformAffected.WPF)]
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	public partial class Issue7671 : ContentPage
	{
		public Issue7671()
		{
#if APP
			InitializeComponent();
#endif
		}
	}
}