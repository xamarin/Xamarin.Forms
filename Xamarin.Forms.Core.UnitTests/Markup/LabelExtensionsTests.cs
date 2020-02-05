using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class LabelExtensionsTests : MarkupBaseTestFixture<Label>
	{
		Label Label => Bindable;

		[Test]
		public void TextLeft()
			=> TestPropertiesSet(l => l.TextStart(), (Label.HorizontalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void TextCenterH()
			=> TestPropertiesSet(l => l.TextCenterH(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void TextRight()
			=> TestPropertiesSet(l => l.TextEnd(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

		[Test]
		public void TextTop()
			=> TestPropertiesSet(l => l.TextTop(), (Label.VerticalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void TextCenterV()
			=> TestPropertiesSet(l => l.TextCenterV(), (Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void TextBottom()
			=> TestPropertiesSet(l => l.TextBottom(), (Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

		[Test]
		public void TextCenter()
			=> TestPropertiesSet(
					l => l.TextCenter(),
					(Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center),
					(Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void FontSize()
			=> TestPropertiesSet(l => l.FontSize(8.0), (Label.FontSizeProperty, 6.0, 8.0));

		[Test]
		public void Bold()
			=> TestPropertiesSet(l => l.Bold(), (Label.FontAttributesProperty, FontAttributes.None, FontAttributes.Bold));

		[Test]
		public void Italic()
			=> TestPropertiesSet(l => l.Italic(), (Label.FontAttributesProperty, FontAttributes.None, FontAttributes.Italic));

		[Test]
		public void FormattedTextSingleSpan()
		{
			Label.FormattedText = null;
			Label.FormattedText(
				new Span { BackgroundColor = Color.Blue }
			);

			var spans = Label.FormattedText?.Spans;
			Assert.That(spans?.Count == 1 && spans[0].BackgroundColor == Color.Blue);
		}

		[Test]
		public void FormattedTextMultipleSpans()
		{
			Label.FormattedText = null;
			Label.FormattedText(
				new Span { BackgroundColor = Color.Blue },
				new Span { BackgroundColor = Color.Green }
			);

			var spans = Label.FormattedText?.Spans;
			Assert.That(spans?.Count == 2 && spans[0].BackgroundColor == Color.Blue && spans[1].BackgroundColor == Color.Green);
		}

		[Test]
		public void SupportDerivedFromLabel()
		{
			DerivedFromLabel _ =
				new DerivedFromLabel()
				.TextStart()
				.TextCenterH()
				.TextEnd()
				.TextTop()
				.TextCenterV()
				.TextBottom()
				.TextCenter()
				.FontSize(8.0)
				.Bold()
				.Italic()
				.FormattedText();
		}

		class DerivedFromLabel : Label { }
	}
}