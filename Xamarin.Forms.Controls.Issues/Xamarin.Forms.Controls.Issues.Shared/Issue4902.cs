﻿using System.Linq;
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
	[Category(UITestCategories.Slider)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4902, "Slider converts Value to minimum when minimum is greater than zero", PlatformAffected.WPF)]
	public class Issue4902 : TestContentPage
	{
		readonly Slider _slider = new Slider(10, 20, 18);
		readonly Label _valueLabel = new Label
		{
			Text = "Min: 10, Max: 20, Value: 18",
			AutomationId = "outputLabel"
		};

		readonly Button _setValueButton = new Button { Text = "Set value to 15", AutomationId = "testbutton"};

		protected override void Init()
		{
			// Initialize ui here instead of ctor

			Content = new StackLayout { Children = { _valueLabel, _slider, _setValueButton } }; 

			_slider.ValueChanged += (sender, args) =>
			{
				_valueLabel.Text = $"Min: 10, Max: 20, Value: {_slider.Value}";
			};
			_setValueButton.Clicked += (sender, args) =>
			{
				_slider.Value = 15;
			}; 
		}

#if UITEST
		[Test]
		public void Issue4902Test() 
		{
			RunningApp.Screenshot("I am at Issue 4902");
			var value = RunningApp.Query(x => x.Marked("outputLabel")).First().Text; 
		    Assert.IsTrue(value.EndsWith("18"));
			RunningApp.Screenshot("The value is initially set to the correct value");
			RunningApp.Tap(q => q.Marked("testbutton"));
			value = RunningApp.Query(x => x.Marked("outputLabel")).First().Text;
			Assert.IsTrue(value.EndsWith("15"));
			RunningApp.Screenshot("The value changed to the correct value");
		}
#endif
	} 
}