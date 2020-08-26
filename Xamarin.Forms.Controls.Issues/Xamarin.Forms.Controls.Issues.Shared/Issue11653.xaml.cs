﻿using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Shapes;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shape)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11653,
		"[Bug] Polygon.Points doesn't respond to CollectionChanged events",
		PlatformAffected.All)]
	public partial class Issue11653 : TestContentPage
	{
		public Issue11653()
		{
#if APP
			Device.SetFlags(new List<string> { ExperimentalFlags.BrushExperimental, ExperimentalFlags.ShapesExperimental });
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		
		}
	}
}