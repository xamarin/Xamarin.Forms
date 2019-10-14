using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7949, "Aspect settings not working on MacOS", PlatformAffected.macOS)]
	public class Issue7949 : TestContentPage
	{
		protected override void Init()
		{
			var grid = new Grid
			{
				BackgroundColor = Color.Yellow
			};

			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = GridLength.Star
			});

			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = GridLength.Star
			});

			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = GridLength.Star
			});

			grid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = GridLength.Star
			});

			var image0 = new Image
			{
				Source = "photo.jpg",
				Aspect = Aspect.AspectFill,
				BackgroundColor = Color.Red,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var image1 = new Image
			{
				Source = "photo.jpg",
				Aspect = Aspect.AspectFit,
				BackgroundColor = Color.Green,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var image2 = new Image
			{
				Source = "photo.jpg",
				Aspect = Aspect.Fill,
				BackgroundColor = Color.Blue,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			grid.Children.Add(image0, 0, 0);
			grid.Children.Add(image1, 1, 0);
			grid.Children.Add(image2, 0, 1);

			Content = grid;
		}
	}
}