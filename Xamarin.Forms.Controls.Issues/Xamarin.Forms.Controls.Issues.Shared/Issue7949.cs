using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7949, "Aspect settings not working on MacOS", PlatformAffected.macOS)]
	public class Issue7949 : TestContentPage
	{
		readonly Aspect[] _aspectSettings = { Aspect.AspectFit, Aspect.AspectFill, Aspect.Fill };
		const string CycleButtonAutomationId = "CycleAspectsThingaMejiggy";

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

			var stack = new StackLayout();

			var image3 = new Image
			{
				Source = "photo.jpg",
				Aspect = Aspect.AspectFit,
				BackgroundColor = Color.Blue,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var button = new Button
			{
				AutomationId = CycleButtonAutomationId,
				Text = "Cycle"
			};

			button.Clicked += (o, a) =>
			{
				var i = _aspectSettings.IndexOf(image3.Aspect) + 1;

				if (i == _aspectSettings.Length)
					i = 0;

				image3.Aspect = _aspectSettings[i];
			};

			stack.Children.Add(image3);
			stack.Children.Add(button);

			grid.Children.Add(image0, 0, 0);
			grid.Children.Add(image1, 1, 0);
			grid.Children.Add(image2, 0, 1);
			grid.Children.Add(stack, 1, 1);

			Content = grid;
		}

#if UITEST
		[Test]
		[UiTest (typeof(Image))]
		public async Task Issue852TestsEntriesClickable()
		{
			RunningApp.WaitForElement (CycleButtonAutomationId);
			RunningApp.Screenshot("All aspects should show properly, lower right should show AspectFit");

			RunningApp.Tap(CycleButtonAutomationId);
			await Task.Delay(1000);
			RunningApp.Screenshot("Lower right should show AspectFill");

			RunningApp.Tap(CycleButtonAutomationId);
			await Task.Delay(1000);
			RunningApp.Screenshot("Lower right should show Fill");

			RunningApp.Tap(CycleButtonAutomationId);
			await Task.Delay(1000);
			RunningApp.Screenshot("Lower right should show AspectFit");
		}
#endif
	}
}