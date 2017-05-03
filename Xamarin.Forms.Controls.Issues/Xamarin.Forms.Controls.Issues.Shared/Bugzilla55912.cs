using System;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 955912, "Tap event not always propagated to containing Grid/StackLayout",
		PlatformAffected.Android)]
	public class Bugzilla55912 : TestContentPage
	{
#if UITEST
		[Test]
		public void GestureBubblingInLayouts()
		{
		}
#endif

		protected override void Init()
		{
			var layout = new Grid();

			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

			var testGrid = new Grid { BackgroundColor = Color.Red };
			var gridLabel = new Label { Text = "This is a Grid with a TapGesture", FontSize = 24, BackgroundColor = Color.Green };
			testGrid.Children.Add(gridLabel);

			var testStack = new StackLayout { BackgroundColor = Color.Aqua };
			var stackLabel = new Label { Text = "This StackLayout also has a TapGesture", FontSize = 24, BackgroundColor = Color.Green };
			Grid.SetRow(testStack, 1);
			testStack.Children.Add(stackLabel);

			layout.Children.Add(testGrid);
			layout.Children.Add(testStack);

			Content = layout;

			testGrid.GestureRecognizers.Add(new TapGestureRecognizer
			{
				NumberOfTapsRequired = 1,
				Command = new Command(() =>
				{
					Debug.WriteLine($"***** TestGrid Tapped: {DateTime.Now} *****");
				})
			});

			testStack.GestureRecognizers.Add(new TapGestureRecognizer
			{
				NumberOfTapsRequired = 1,
				Command = new Command(() =>
				{
					Debug.WriteLine($"***** TestStack Tapped: {DateTime.Now} *****");
				})
			});
		}
	}
}