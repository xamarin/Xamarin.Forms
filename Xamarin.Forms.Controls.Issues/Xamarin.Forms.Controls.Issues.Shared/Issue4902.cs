using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4902, "Slider converts Value to minimum when minimum is greater than zero", PlatformAffected.WPF)]
	public class Issue4902 : TestContentPage
	{
		readonly Slider _slider = new Slider(10, 20, 18);
		readonly Label _valueLabel = new Label
		{
			Text = "Min: 10, Max: 20, Value: 18"
		};

		readonly Button _setValueButton = new Button { Text = "Set value to 15" };

		protected override void Init()
		{
			// Initialize ui here instead of ctor

			Content = new StackLayout { Children = { _valueLabel, _slider, _setValueButton } };
			//new Label { AutomationId = "Issue1Label", Text = "See if I'm here" };}

			_slider.ValueChanged += (sender, args) =>
			{
				_valueLabel.Text = $"Min: 10, Max: 20, Value: {_slider.Value}";
			};
			_setValueButton.Clicked += (sender, args) =>
			{
				_slider.Value = 15;
			};

			BindingContext = new ViewModelIssue4902();
		}

#if UITEST
		[Test]
		public void Issue1Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Issue1");
			RunningApp.WaitForElement(q => q.Marked("Issue1Label"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue4902
	{
		public ViewModelIssue4902()
		{

		}
	}
}