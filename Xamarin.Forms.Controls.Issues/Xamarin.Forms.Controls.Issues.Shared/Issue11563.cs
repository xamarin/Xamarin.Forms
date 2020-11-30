﻿using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;

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
	[Issue(IssueTracker.Github, 11563,
		"[Bug] Polygon.Points doesn't respond to CollectionChanged events",
		PlatformAffected.All)]
	public class Issue11563 : TestContentPage
	{
		public Issue11563()
		{
		}

		protected override void Init()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Tap the button, if the Polygon is updated, the test has passed."
			};

			var updatePointsButton = new Button
			{
				Text = "Update points"
			};

			var updatePointsCollectionButton = new Button
			{
				Text = "Update points collection"
			};

			var points1 = new PointCollection() { new Point(10, 10), new Point(100, 50), new Point(100, 95), new Point(10, 95) };
			var points2 = new PointCollection() { new Point(10, 5), new Point(100, 70), new Point(100, 95), new Point(10, 95) };

			var polygon = new Polygon
			{
				HeightRequest = 100,
				WidthRequest = 100,
				StrokeThickness = 2,
				Stroke = Brush.Red,
				Points = points1
			};

			layout.Children.Add(instructions);
			layout.Children.Add(updatePointsButton);
			layout.Children.Add(updatePointsCollectionButton);
			layout.Children.Add(polygon);

			Content = layout;

			updatePointsButton.Clicked += (sender, args) =>
			{
				if (points1.Count > 1)
					points1.RemoveAt(1);

				if (points2.Count > 1)
					points2.RemoveAt(1);
			};

			updatePointsCollectionButton.Clicked += (sender, args) =>
			{
				polygon.Points = points2;
			};
		}
	}
}
