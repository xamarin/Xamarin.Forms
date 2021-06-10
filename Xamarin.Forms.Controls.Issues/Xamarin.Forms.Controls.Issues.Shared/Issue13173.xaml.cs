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
	[Issue(IssueTracker.Github, 13173, "[Bug] Line's Stroke and Rectangle's Fill doesn't rendered as expected with SolidColorBrush while changing the system's appearance",
		PlatformAffected.iOS)]
	public partial class Issue13173 : TestContentPage
	{
		public Issue13173()
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