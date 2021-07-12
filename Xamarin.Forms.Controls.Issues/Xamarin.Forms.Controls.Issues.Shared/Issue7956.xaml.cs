using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7956, "[Shell] Unable to focus and unfocus search handler for Android",
		PlatformAffected.Android)]
	public partial class Issue7956 : TestShell
	{
		public Issue7956()
		{
			BindingContext = this;
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}

		private void SearchHandler_Focused(object sender, System.EventArgs e)
		{
			lblFocus.IsVisible = true;
			lblUnfocus.IsVisible = false;
		}
		private void SearchHandler_Unfocused(object sender, System.EventArgs e)
		{
			lblFocus.IsVisible = false;
			lblUnfocus.IsVisible = true;
		}
	}
}
