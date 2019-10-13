using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest.Queries;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7176, "[Enhancement] Label Autofit Font", PlatformAffected.Android | PlatformAffected.iOS)]
#if UITEST
	[Category(UITestCategories.Label)]
#endif
	public class Issue7176 : TestContentPage
	{
		const string GetFontSize = "getTextSize";
		const string GetAutoSizeMode = "getAutoSizeTextType";
		const string AutoFitLabel = "Autofit Label";
		const string AutoFitModeButton = "Autofit Mode Button";

		protected override void Init()
		{
			var autofitLabel = new Label
			{
				AutomationId = AutoFitLabel,
				AutoFit = true,
				Text = "Welcome to xamarin forms!!",
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.LightBlue
			};

			Content = new StackLayout()
			{
				Children =
				{
					new ApiLabel(),
					autofitLabel,
					new Button
					{
						Text = "Change autofit mode",
						AutomationId = AutoFitModeButton,
						Command = new Command(() => autofitLabel.AutoFit = !autofitLabel.AutoFit)
					}
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		[UiTest(typeof(Label))]
		public void Issue7176TestAutoFitMode()
		{
			RunningApp.WaitForElement(AutoFitLabel);
			if (!RunningApp.IsApiHigherThan(13)) return;

			var autoSizeMode = RunningApp.Query(
					c => c.Marked(AutoFitLabel).Invoke(GetAutoSizeMode).Value<int>())[0];

			Assert.AreEqual(autoSizeMode, 1);//check autosize mode are "Uniform (1)"

			var withAutofit = GetLabelFontSize();
			RunningApp.Tap(AutoFitModeButton);
			var withoutAutofit = GetLabelFontSize();

			Assert.AreNotEqual(withAutofit, withoutAutofit);
			Assert.IsTrue(withoutAutofit < withAutofit);
		}

		float GetLabelFontSize() => RunningApp.Query(
			c => c.Marked(AutoFitLabel).Invoke(GetFontSize).Value<float>())[0];
#endif
	}
}
