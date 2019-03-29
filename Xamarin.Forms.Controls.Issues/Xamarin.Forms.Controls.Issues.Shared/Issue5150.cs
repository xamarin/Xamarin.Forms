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

		public static readonly BindableProperty NameAndHelpTextProperty = BindableProperty.CreateAttached(
			"NameAndHelpText",
			typeof(string),
			typeof(ContentDescriptionEffectProperties),
			null);

		public static string GetContentDescription(BindableObject view)
		{
			return (string)view.GetValue(ContentDescriptionProperty);
		}

		public static string GetNameAndHelpText(BindableObject view)
		{
			return (string)view.GetValue(NameAndHelpTextProperty);
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
		void AddButton(StackLayout layout, string automationId, string buttonText, string buttonName = null, string buttonHelp = null)
		{
			var button = new Button();
			var automationIdLabel = new Label();
			var contentDescriptionLabel = new Label();
			var nameAndHelpTextLabel = new Label();
			automationIdLabel.Text = $"AutomationId = {automationId}";
			button.AutomationId = automationId;
			button.Text = buttonText;
			button.Effects.Add(new ContentDescriptionEffect());
			button.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == ContentDescriptionEffectProperties.ContentDescriptionProperty.PropertyName)
				{
					contentDescriptionLabel.Text = $"ContentDescription = {ContentDescriptionEffectProperties.GetContentDescription(button)}";
				}

				if (e.PropertyName == ContentDescriptionEffectProperties.NameAndHelpTextProperty.PropertyName)
				{
					nameAndHelpTextLabel.Text = $"Name + Help Text = {ContentDescriptionEffectProperties.GetNameAndHelpText(button)}";
				}
			};
			layout.Children.Add(button);
			layout.Children.Add(automationIdLabel);
			layout.Children.Add(contentDescriptionLabel);
			layout.Children.Add(nameAndHelpTextLabel);

			button.SetValue(AutomationProperties.NameProperty, buttonName);
			button.SetValue(AutomationProperties.HelpTextProperty, buttonHelp);
		}

		protected override void Init()
		{
			var layout = new StackLayout();
			layout.Children.Add(new Label
			{
				Text = "On the Android platform, the 'Name + Help Text' " +
					"labels below each button should match the text read by " +
					"TalkBack without interferring with the AutomationId and " +
					"ContentDescription."
			});

			AddButton(layout, "button1", "Button 1", buttonName: "Name 1");
			AddButton(layout, "button2", "Button 2", buttonHelp: "Help 2.");
			AddButton(layout, "button3", "Button 3", "Name 3", "Help 3.");
			AddButton(layout, "button4", null , buttonHelp: "Help 4.");

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
			RunningApp.WaitForElement(q => q.Marked("button1"));
			RunningApp.WaitForElement(q => q.Marked("button2"));
			RunningApp.WaitForElement(q => q.Marked("button3"));
			RunningApp.WaitForElement(q => q.Marked("button4"));

			RunningApp.WaitForElement (q => q.Text("Name + HelpText = Name 1"));
			RunningApp.Screenshot ("I see the label Name + HelpText = Name 1");

			RunningApp.WaitForElement(q => q.Text("Name + HelpText = Button 2. Help 2."));
			RunningApp.Screenshot("I see the label Name + HelpText = Button 2. Help 2.");

			RunningApp.WaitForElement(q => q.Text("Name + HelpText = Name 3. Help 3."));
			RunningApp.Screenshot("I see the label Name + HelpText = Name 3. Help 3.");

			RunningApp.WaitForElement(q => q.Text("Name + HelpText = Help 4."));
			RunningApp.Screenshot("I see the label Name + HelpText = Help 4.");
		}
#endif
	}
}