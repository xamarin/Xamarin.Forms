using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class LabelExtensionsTests : MarkupBaseTestFixture<Label>
	{
		Label Label => Bindable;

		[Test]
		public void TextLeft()
			=> TestPropertySet(l => l.TextLeft(), (Label.HorizontalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void TextCenterH()
			=> TestPropertySet(l => l.TextCenterH(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void TextRight()
			=> TestPropertySet(l => l.TextRight(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

		[Test]
		public void TextTop()
			=> TestPropertySet(l => l.TextTop(), (Label.VerticalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void TextCenterV()
			=> TestPropertySet(l => l.TextCenterV(), (Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void TextBottom()
			=> TestPropertySet(l => l.TextBottom(), (Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

		[Test]
		public void TextCenter()
			=> TestPropertySet(
					l => l.TextCenter(),
					(Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center),
					(Label.VerticalTextAlignmentProperty, TextAlignment.Start, TextAlignment.Center));

		[Test]
		public void FontSize()
			=> TestPropertySet(l => l.FontSize(8.0), (Label.FontSizeProperty, 6.0, 8.0));

		[Test]
		public void Bold()
			=> TestPropertySet(l => l.Bold(), (Label.FontAttributesProperty, FontAttributes.None, FontAttributes.Bold));

		[Test]
		public void Italic()
			=> TestPropertySet(l => l.Italic(), (Label.FontAttributesProperty, FontAttributes.None, FontAttributes.Italic));

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
				.TextLeft()
				.TextCenterH()
				.TextRight()
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