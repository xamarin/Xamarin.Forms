using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Layout)]
#endif
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 1758, "LayoutTo needs to be smarted about using layout specific API calls", PlatformAffected.Android | PlatformAffected.iOS | PlatformAffected.WinPhone)]
	public class Issue1758 : TestContentPage
	{
		ListView _list;
		Button _button;
		const string ButtonAutomationId = "Button";

		protected override void Init()
		{
			_list = new ListView { ItemsSource = new[] { "hello", "world", "from", "xamarin", "forms" } };

			_button = new Button { Text = "Button", AutomationId = ButtonAutomationId };

			// The same behavior happens for both Absolute and Relative layout.
			//var layout = true ? Relative() : Absolute();
			var layout = Relative();

			Content = layout;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Animate();
		}

		Layout Relative()
		{
			var layout = new RelativeLayout();
                   
			layout.Children.Add(_list,
				Forms.Constraint.RelativeToParent(p => p.X),
				Forms.Constraint.RelativeToParent(p => p.Y),
				Forms.Constraint.RelativeToParent(p => p.Width),
				Forms.Constraint.RelativeToParent(p => p.Height)
			);
        
			layout.Children.Add(_button, 
				Forms.Constraint.Constant(0),
				Forms.Constraint.Constant(0));
 
			return layout;
		}

		Layout Absolute()
		{
			var layout = new AbsoluteLayout { Children = { _list, _button } };

			AbsoluteLayout.SetLayoutBounds(_list, new Rectangle(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
			AbsoluteLayout.SetLayoutBounds(_button, new Rectangle(0, 300, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

			return layout;
		}

		async void Animate()
		{
			await Task.Delay(2000).ContinueWith(t => 
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					await _button.LayoutTo(new Rectangle(100, 100, 100, 100), 1000);
				});
			});
		}

#if UITEST
		[Test]
		public void CheckButtonPosition()
		{
			RunningApp.WaitForElement(ButtonAutomationId);

			var oldRect = RunningApp.Query(ButtonAutomationId).Single().Rect;

			try
			{
				RunningApp.WaitForNoElement(ButtonAutomationId, timeout: TimeSpan.FromSeconds(5));
			}
			catch
			{
				// swallow exception
			}

			var button = RunningApp.Query(ButtonAutomationId).Single();

			Assert.IsTrue(button.Rect.Width == button.Rect.Height);
			Assert.IsTrue(button.Rect.X == button.Rect.Width);
			Assert.IsTrue(button.Rect.Y == oldRect.Y + button.Rect.Height);
		}
#endif
	}
}