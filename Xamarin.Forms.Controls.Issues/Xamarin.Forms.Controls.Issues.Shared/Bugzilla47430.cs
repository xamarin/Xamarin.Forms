using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.Bugzilla, 47430, "[A] ButtonRenderer does not set TextFormatted property on override to OnElementChanged", PlatformAffected.Android)]
    public class Bugzilla47430 : TestContentPage
    {
        protected override void Init()
        {
            var button = new Bugzilla47430Button
            {
                BackgroundColor = Color.Red,
                AutomationId = "Bugzilla47430Button"
            };

            var formattedString = new FormattedString();
            formattedString.Spans.Add(new Span
            {
                Text = "Testing",
                FontAttributes = FontAttributes.Italic,
                ForegroundColor = Color.Aqua,
                BackgroundColor = Color.Black
            });
            formattedString.Spans.Add(new Span
            {
                Text = "FormattedText",
                FontAttributes = FontAttributes.Bold,
                ForegroundColor = Color.Green,
                BackgroundColor = Color.Aqua
            });
            button.FormattedText = formattedString;

            Content = new StackLayout
            {
                Children =
                {
                    button,
                    new Label
                    {
                        Text = "The above button should show spans on its text due to the use of a custom renderer."
                    },
                    new Button
                    {
                        AutomationId = "Bugzilla47430ChangeTextButton",
                        Text = "Click to change text",
                        Command = new Command(() => button.Text = "Updated text!")
                    },
                    new Button
                    {
                        AutomationId = "Bugzilla47430ChangeTextToEmptyStringButton",
                        Text = "Click to change text",
                        Command = new Command(() => button.Text = "")
                    }
                }
            };
        }

        public class Bugzilla47430Button : Button
        {
            public static readonly BindableProperty FormattedTextProperty =
                BindableProperty.Create(propertyName: nameof(FormattedText), returnType: typeof(FormattedString), declaringType: typeof(Bugzilla47430Button), defaultValue: null);

            public FormattedString FormattedText
            {
                get { return (FormattedString)GetValue(FormattedTextProperty); }
                set { SetValue(FormattedTextProperty, value); }
            }
        }

#if UITEST

#if __ANDROID__
		[Test]
		public void Bugzilla47430Test()
		{
			RunningApp.WaitForElement(q => q.Marked("Bugzilla47430Button"));
			var button = RunningApp.Query(q => q.Marked("Bugzilla47430Button"))[0];
			Assert.AreEqual("TestingFormattedText", button.Text);
			RunningApp.Tap("Bugzilla47430ChangeTextButton");
			RunningApp.WaitForElement(q => q.Marked("Updated Text!"));
			Assert.AreEqual("Updated text!", button.Text);
			RunningApp.Tap("Bugzilla47430ChangeTextToEmptyStringButton");
			Assert.AreEqual("", button.Text);
		}
#endif

#endif
	}
}