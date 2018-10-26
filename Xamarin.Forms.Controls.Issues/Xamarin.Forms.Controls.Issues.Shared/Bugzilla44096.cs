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
	[Issue(IssueTracker.Bugzilla, 44096, "Grid, StackLayout, and ContentView still participate in hit testing on " 
		+ "Android after IsEnabled is set to false", PlatformAffected.Android)]
	public class Bugzilla44096 : TestContentPage
	{
		bool _flag;
		const string Original = "Original";
		const string ToggleColor = "color";
		const string ToggleIsEnabled = "disabled";

		const string ResultLabel = "resultLabel";
		const string StackLayout = "stackLayout";
		const string StackLayoutButton = "stackLayoutButton";
		const string ContentView = "contentView";
		const string ContentViewButton = "contentViewButton";
		const string Grid = "grid";
		const string GridButton = "gridButton";
		const string RelativeLayout = "relativeLayout";
		const string RelativeLayoutButton = "relativeLayoutButton";

		protected override void Init()
		{
			var instructions = new Label
			{
				Text = @"Clicking the buttons or the buttons parent views should display their automation ids in the label." +
				       "Clicking the \"Toggle IsEnabled\" button should prevent click events on buttons or their parent view to trigger."+
				       "The label should then display \"Original\". If it does not, the test failed."
			};

			var result = new Label
			{
				Text = Original,
				AutomationId = ResultLabel
			};

			var grid = new Grid
			{
				IsEnabled = true,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = Grid,
				Padding = new Thickness(10)
			};
			grid.Children.Add(new Button
			{
				Text = GridButton,
				Command = new Command(() =>
				{
					result.Text = GridButton;
				})
			});
			AddTapGesture(result, grid);

			var contentView = new ContentView
			{
				IsEnabled = true,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = ContentView,
				Padding = new Thickness(10),
				Content = new Button
				{
					Text = ContentViewButton,
					Command = new Command(() =>
					{
						result.Text = ContentViewButton;
					})
				}
			};
			AddTapGesture(result, contentView);

			var stackLayout = new StackLayout
			{
				IsEnabled = true,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = StackLayout,
				Padding = new Thickness(10)
			};
			stackLayout.Children.Add(new Button
			{
				Text = StackLayoutButton,
				Command = new Command(() =>
					{
						result.Text = StackLayoutButton;
					}),
				VerticalOptions = LayoutOptions.Center
			});
			AddTapGesture(result, stackLayout);

			var relativeLayout = new RelativeLayout
			{
				IsEnabled = true,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = RelativeLayout,
				Padding = new Thickness(10)
			};
			relativeLayout.Children.Add(new Button
			{
				WidthRequest = 150,
				HeightRequest = 40,
				Text = RelativeLayoutButton,
				Command = new Command(() =>
				{
					result.Text = RelativeLayoutButton;
				})
			},()=>0, ()=>0,()=>200,()=>50 );
			AddTapGesture(result, relativeLayout);

			var color = new Button
			{
				Text = "Toggle colors",
				Command = new Command(() =>
				{
					if (!_flag)
					{
						grid.BackgroundColor = Color.Red;
						contentView.BackgroundColor = Color.Blue;
						stackLayout.BackgroundColor = Color.Yellow;
						relativeLayout.BackgroundColor = Color.Green;
					}
					else
					{
						grid.BackgroundColor = Color.Default;
						contentView.BackgroundColor = Color.Default;
						stackLayout.BackgroundColor = Color.Default;
						relativeLayout.BackgroundColor = Color.Default;
					}

					_flag = !_flag;
				}),
				AutomationId = ToggleColor
			};

			var disabled = new Button
			{
				Text = "Toggle IsEnabled",
				Command = new Command(() =>
				{
					grid.IsEnabled = !grid.IsEnabled;
					contentView.IsEnabled = !contentView.IsEnabled;
					stackLayout.IsEnabled = !stackLayout.IsEnabled;
					relativeLayout.IsEnabled = !relativeLayout.IsEnabled;

					result.Text = Original;
				}),
				AutomationId = ToggleIsEnabled
			};

			var parent = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					instructions,
					color,
					disabled,
					result,
					grid,
					contentView,
					stackLayout,
					relativeLayout
				}
			};

			Content = parent;
		}

		void AddTapGesture(Label result, View view)
		{
			var tapGestureRecognizer = new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					result.Text = view.AutomationId;
				})
			};
			view.GestureRecognizers.Add(tapGestureRecognizer);
		}

#if UITEST && (__WINDOWS__ || __ANDROID__)

		[Test]
		public void TestGrid()
		{
			TestControl(Grid,GridButton);
		}

		[Test]
		public void TestContentView()
		{
			TestControl(ContentView, ContentViewButton);
		}

		[Test]
		public void TestStackLayout()
		{
			TestControl(StackLayout, StackLayoutButton);
		}

		[Test]
		public void TestRelativeLayout()
		{
			TestControl(RelativeLayout,RelativeLayoutButton);
		}

		void TestControl(string control, string controlButton=null)
		{
			//Tap the button inside the control and check it worked
			TapControlAndAssert(controlButton, controlButton);

			//Toggle the background color on the controls. Without color, tap is not triggered on UWP
			RunningApp.WaitForElement(q => q.Marked(ToggleColor));
			RunningApp.Tap(q => q.Marked(ToggleColor));

			//Tap the control and check it worked
			TapControlAndAssert(control, control);
			//Tap the button inside the control and check it worked
			TapControlAndAssert(controlButton, controlButton);

			//Disable tap gestures on controls
			RunningApp.WaitForElement(q => q.Marked(ToggleIsEnabled));
			RunningApp.Tap(q => q.Marked(ToggleIsEnabled));

			//Toggle the background color on the controls.
			RunningApp.WaitForElement(q => q.Marked(ToggleColor));
			RunningApp.Tap(q => q.Marked(ToggleColor));

			//Tap the control and check it didn't trigger
			TapControlAndAssert(control, Original);
			//Tap the button inside the control and check it didn't trigger
			TapControlAndAssert(controlButton, Original);
		}

		void TapControlAndAssert(string control, string expectedResult)
		{
			var element= RunningApp.WaitForElement(q => q.Marked(control))[0];
			RunningApp.TapCoordinates(element.Rect.X + element.Rect.Width -5, element.Rect.Y + element.Rect.Height-5);

			var label = RunningApp.WaitForElement(q => q.Marked(ResultLabel))[0];
			Assert.AreEqual(expectedResult, label.Description);
		}
#endif
	}
}
