﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12912,
		"[Bug] ImageButton Clip Shapes",
		PlatformAffected.Android)]
	public partial class Issue12912 : TestContentPage
	{
		public Issue12912()
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