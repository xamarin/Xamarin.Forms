using System;
using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{

	public static class ContentDescriptionEffectProperties
	{
		public static readonly BindableProperty ContentDescriptionProperty = BindableProperty.CreateAttached(
			"ContentDescription",
			typeof(string),
			typeof(ContentDescriptionEffectProperties),
			null,
			propertyChanged: OnContentDescriptionChanged );

		public static string GetContentDescription(BindableObject view)
		{
			return (string)view.GetValue(ContentDescriptionProperty);
		}

		public static void SetContentDescription(BindableObject view, string value)
		{
			view.SetValue(ContentDescriptionProperty, value);
		}

		static void OnContentDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			System.Diagnostics.Debug.WriteLine($"Old value = {oldValue}, new value = {newValue}");
		}
	}

	public class ContentDescriptionEffect : RoutingEffect
	{
		public const string EffectName = "ContentDescriptionEffect";

		public ContentDescriptionEffect() : base($"{Issues.Effects.ResolutionGroupName}.{EffectName}")
		{
		}
	}

#if UITEST
	[Category(UITestCategories.Button)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5150, "AutomationProperties.Name, AutomationProperties.HelpText on Button not read by Android TalkBack", PlatformAffected.Android)]
	public class Issue5150 : TestContentPage // or TestMasterDetailPage, etc ...
	{

		private void Configure(Button button, Label label, StackLayout layout, string buttonText, string buttonName = null, string buttonHelp = null)
		{
			button.Text = buttonText;
			button.Effects.Add(new ContentDescriptionEffect());
			button.SetValue(AutomationProperties.NameProperty, buttonName);
			button.SetValue(AutomationProperties.HelpTextProperty, buttonHelp);
			button.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == ContentDescriptionEffectProperties.ContentDescriptionProperty.PropertyName)
				{
					var element = sender as Button;
					var desc = (string)element.GetValue(ContentDescriptionEffectProperties.ContentDescriptionProperty);
					label.Text = desc;
				}
			};
			layout.Children.Add(button);
			layout.Children.Add(label);
		}

		protected override void Init()
		{
			var layout = new StackLayout();

			var ButtonWithTextAndName = new Button();
			var ButtonWithTextAndNameLabel = new Label();
			Configure(ButtonWithTextAndName, ButtonWithTextAndNameLabel, layout, "Button 1", buttonName: "Name 1");

			var ButtonWithTextAndHelp = new Button();
			var ButtonWithTextAndHelpLabel = new Label();
			Configure(ButtonWithTextAndHelp, ButtonWithTextAndHelpLabel, layout, "Button 2", buttonHelp: "Help 2.");

			var ButtonWithTextAndNameAndHelp = new Button();
			var ButtonWithTextAndNameAndHelpLabel = new Label();
			Configure(ButtonWithTextAndNameAndHelp, ButtonWithTextAndNameAndHelpLabel, layout, "Button 3", "Name 3", "Help 3.");

			var ButtonWithHelp = new Button();
			var ButtonWithHelpLabel = new Label();
			Configure(ButtonWithHelp, ButtonWithHelpLabel, layout, null , null, "Help 4.");

			Content = layout;
		}

#if UITEST
		[Test]
		public void Issue5150Test() 
		{
			RunningApp.Screenshot ("I am at Issue 5150");

			RunningApp.WaitForElement (q => q.Text("Name 1"));
			RunningApp.Screenshot ("I see the label Name 1");

			RunningApp.WaitForElement(q => q.Text("Button 2. Help 2."));
			RunningApp.Screenshot("I see the label Button 2. Help 2.");

			RunningApp.WaitForElement(q => q.Text("Name 3. Help 3."));
			RunningApp.Screenshot("I see the label Name 3. Help 3.");

			RunningApp.WaitForElement(q => q.Text("Help 4."));
			RunningApp.Screenshot("I see the label Help 4.");
		}
#endif
	}
}