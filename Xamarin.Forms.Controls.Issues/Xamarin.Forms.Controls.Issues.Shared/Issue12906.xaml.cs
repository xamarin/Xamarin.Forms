using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12906,
		"Xamarin.Forms - programmatically changing CornerRadius of a BoxView causes it to become invisible",
		PlatformAffected.Android)]
	public partial class Issue12906 : TestContentPage
	{
		public Issue12906()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}

#if APP
		void OnUpdateCornerRadiusClicked(object sender, EventArgs e) => Box.CornerRadius = new CornerRadius(12, 0, 24, 0);

		void OnUpdateColorClicked(object sender, EventArgs e)
		{
			Box.Color = Color.Orange;
			Box.Background = Brush.Default;
		}

		void OnUpdateBackgroundClicked(object sender, EventArgs e)
		{
			Box.Color = Color.Default;

			LinearGradientBrush background = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0)
			};

			background.GradientStops.Add(new GradientStop { Color = Color.BlueViolet, Offset = 0.1f });
			background.GradientStops.Add(new GradientStop { Color = Color.CornflowerBlue, Offset = 1.0f });

			Box.Background = background;
		}
#endif
	}
}