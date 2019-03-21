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
			null);

		public static string GetContentDescription(BindableObject view)
		{
			return (string)view.GetValue(ContentDescriptionProperty);
		}
	}

	public class ContentDescriptionEffect : RoutingEffect
	{
		public const string EffectName = "ContentDescriptionEffect";

		public ContentDescriptionEffect() : base($"{Effects.ResolutionGroupName}.{EffectName}")
		{
		}
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5150, "AutomationProperties.Name, AutomationProperties.HelpText on Button not read by Android TalkBack", PlatformAffected.Android)]
	public class Issue5150 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		void AddButton(StackLayout layout, string buttonText, string buttonName = null, string buttonHelp = null)
		{
			var button = new Button();
			var label = new Label();
			button.Text = buttonText;
			button.Effects.Add(new ContentDescriptionEffect());
			button.SetValue(AutomationProperties.NameProperty, buttonName);
			button.SetValue(AutomationProperties.HelpTextProperty, buttonHelp);
			button.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == ContentDescriptionEffectProperties.ContentDescriptionProperty.PropertyName)
				{
					label.Text = ContentDescriptionEffectProperties.GetContentDescription(button);
				}
			};
			layout.Children.Add(button);
			layout.Children.Add(label);
		}

		protected override void Init()
		{
			var layout = new StackLayout();
			layout.Children.Add(new Label
			{
				Text = "On the Android platform, the labels below each button should match the text read by TalkBack."
			});

			AddButton(layout, "Button 1", buttonName: "Name 1");
			AddButton(layout, "Button 2", buttonHelp: "Help 2.");
			AddButton(layout, "Button 3", "Name 3", "Help 3.");
			AddButton(layout, null , buttonHelp: "Help 4.");

			Content = layout;
		}


#if UITEST
		[Test]
		[Category(UITestCategories.Button)]
#if !__ANDROID__
		[Ignore("This test verifies ContentDescription is set on the Android platform.")]
#endif
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