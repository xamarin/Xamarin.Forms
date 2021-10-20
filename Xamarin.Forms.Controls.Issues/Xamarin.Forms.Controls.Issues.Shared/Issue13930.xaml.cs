using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13930,
		"[Bug] Entry Background doesn't work to set color",
		PlatformAffected.iOS)]
	public partial class Issue13930 : TestContentPage
	{
		public Issue13930()
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