using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7534, "Span with tail truncation and paragraph breaks with Java.Lang.IndexOutOfBoundsException", PlatformAffected.Android)]
	public partial class Issue7534 : TestContentPage
	{
		protected override void Init()
		{
			var span = new Span
			{
				Text =
				" Mi augue molestie ligula lobortis enim Velit, in. \n Imperdiet eu dignissim odio. Massa erat Hac inceptos facilisis nibh " +
				" Interdum massa Consectetuer risus sociis molestie facilisi enim. Class gravida. \n Gravida sociosqu cras Quam velit, suspendisse" +
				"  leo auctor odio integer primis dui potenti dolor faucibus augue justo morbi ornare sem. "
			};

			var formattedString = new FormattedString();
			formattedString.Spans.Add(span);

			var label = new Xamarin.Forms.Label
			{
				LineBreakMode = LineBreakMode.TailTruncation,
				FormattedText = formattedString,
				MaxLines = 3 
				//max line is less than the text reproduce and textViewExtensions couldn't identify when
			};

			var layout = new Xamarin.Forms.StackLayout();
			layout.Children.Add(label);

			Content = layout;
		}
	}
}
