using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13573, "[Bug] Editor PlaceholderColor and BackgroundColor incorrect on UWP after upgrading to Xamarin.Forms 4.8 (from 4.5)", PlatformAffected.UWP)]
	public partial class Issue13573 : TestContentPage
	{
		public Issue13573()
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