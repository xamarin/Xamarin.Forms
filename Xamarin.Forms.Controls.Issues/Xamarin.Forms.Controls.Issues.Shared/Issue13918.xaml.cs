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
	[Issue(IssueTracker.Github, 13918, "[Bug] iOS DatePicker only displays the last two digits of the year",
		PlatformAffected.iOS)]
	public partial class Issue13918 : TestContentPage
	{
		public Issue13918()
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