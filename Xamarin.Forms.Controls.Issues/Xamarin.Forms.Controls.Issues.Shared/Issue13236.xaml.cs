using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13236,
		"[Bug] ScaleTo IllegalArgumentException (Android only)",
		PlatformAffected.Android)]
	public partial class Issue13236 : TestContentPage
	{
		public Issue13236()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}

#if APP
		async void OnButtonClicked(System.Object sender, System.EventArgs e)
		{
			await TheFrame.ScaleTo(0.2, 0);
			await TheFrame.ScaleTo(0.2, 0);
		}
#endif
	}
}