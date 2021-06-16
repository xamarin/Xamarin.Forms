using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13392, "[Bug] Brush dissappear after scroll in ListView on Android", PlatformAffected.Android)]
	public partial class Issue13392 : TestContentPage
	{
		public Issue13392()
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
