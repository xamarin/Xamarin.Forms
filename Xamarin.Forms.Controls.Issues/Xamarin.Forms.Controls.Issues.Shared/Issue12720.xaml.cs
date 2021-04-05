using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.WebView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12720, "WebView Navigating Does Not Support Async Cancellation", PlatformAffected.All)]
	public partial class Issue12720 : TestContentPage
	{
		public Issue12720()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
#endif
		}

		protected override void Init()
		{
			
		}
	}
}
