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
	[Issue(IssueTracker.Github, 11481, "[Bug] Visual Material crashes when using LinearGradientBrush")]
	public partial class Issue11481 : TestContentPage
    {
        public Issue11481()
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