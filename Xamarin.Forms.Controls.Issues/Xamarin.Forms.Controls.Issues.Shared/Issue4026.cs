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
	[Issue(IssueTracker.Github, 4026, "Bindable Span height Issue", PlatformAffected.All)]
	public class Issue4026 : TestContentPage
	{
		public Issue4026()
		{
			BindingContext = this;
		}
		protected override void Init()
		{
			BackgroundColor = Color.Aquamarine;
			var layout = new StackLayout { VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.CadetBlue, Padding = new Thickness(10) };
			var btn = new Button { Text = "Add More Text to Span", Command = new Command(() => Title += " More Text") };
			var spanBindable = new Span { TextColor = Color.Blue };
			spanBindable.SetBinding(Span.TextProperty, new Binding(nameof(Title), BindingMode.OneWay));
			var label = new Label { BackgroundColor = Color.Red, FormattedText = new FormattedString { Spans = { new Span { Text = "Span Test Span Test Span Test Span Test" }, spanBindable } } };
			layout.Children.Add(btn);
			layout.Children.Add(label);
			Content = layout;
		}
	}
}
