using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12885, "[Bug] Background brush does not support transparency in certain views on Android", PlatformAffected.Android)]
	public partial class Issue12885 : TestContentPage
	{
		public Issue12885()
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