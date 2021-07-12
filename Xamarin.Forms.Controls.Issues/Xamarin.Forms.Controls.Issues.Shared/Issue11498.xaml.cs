using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11498, "[Bug] [Shapes] Drawing inconsistencies", PlatformAffected.Android)]
	public partial class Issue11498 : TestContentPage
	{
		public Issue11498()
		{
#if APP
			Title = "Issue 11498";
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}
	}
}