using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14350,
		"[Bug] RefreshView to completes instantly when using AsyncCommand or Command with CanExecute functionality",
		PlatformAffected.Android)]
	public partial class Issue14350 : ContentPage
	{
		public Issue14350()
		{
#if APP
			InitializeComponent();
			BindingContext = new ViewModelIssue8384();
#endif
		}
	}
}
