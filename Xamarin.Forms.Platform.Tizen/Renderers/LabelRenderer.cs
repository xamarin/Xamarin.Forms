using EColor = ElmSharp.Color;
using Specific = Xamarin.Forms.PlatformConfiguration.TizenSpecific.Label;

namespace Xamarin.Forms.Platform.Tizen
{

	public class LabelRenderer : ViewRenderer<Label, Native.Label>
	{
		static readonly EColor s_defaultBackgroundColor = EColor.Transparent;
		static readonly EColor s_defaultForegroundColor = EColor.Black;
		static readonly EColor s_defaultTextColor = s_defaultForegroundColor;

		public LabelRenderer()
		{
			RegisterPropertyHandler(Label.TextProperty, () => Control.Text = Element.Text);
			RegisterPropertyHandler(Label.TextColorProperty, UpdateTextColor);
			// FontProperty change is called also for FontSizeProperty, FontFamilyProperty and FontAttributesProperty change
			RegisterPropertyHandler(Label.FontProperty, UpdateFontProperties);
			RegisterPropertyHandler(Label.LineBreakModeProperty, UpdateLineBreakMode);
			RegisterPropertyHandler(Label.HorizontalTextAlignmentProperty, UpdateTextAlignment);
			RegisterPropertyHandler(Label.VerticalTextAlignmentProperty, UpdateTextAlignment);
			RegisterPropertyHandler(Label.FormattedTextProperty, UpdateFormattedText);
			RegisterPropertyHandler(Specific.FontWeightProperty, UpdateFontWeight);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			if (Control == null)
			{
				var label = new Native.Label(Forms.Context.MainWindow);
				base.SetNativeControl(label);
			}

			if (e.OldElement != null)
			{
			}

			if (e.NewElement != null)
			{
			}

			base.OnElementChanged(e);
		}

		protected override Size MinimumSize()
		{
			return Control.Measure(Control.MinimumWidth, Control.MinimumHeight).ToDP();
		}

		Native.FormattedString ConvertFormattedText(FormattedString formattedString)
		{
			if (formattedString == null)
			{
				return null;
			}

			Native.FormattedString nativeString = new Native.FormattedString();

			foreach (var span in formattedString.Spans)
			{
				Native.Span nativeSpan = new Native.Span();
				nativeSpan.Text = span.Text;
				nativeSpan.FontAttributes = span.FontAttributes;
				nativeSpan.FontFamily = span.FontFamily;
				nativeSpan.FontSize = span.FontSize;
				nativeSpan.ForegroundColor = span.ForegroundColor.ToNative();
				nativeSpan.BackgroundColor = span.BackgroundColor.ToNative();
				nativeString.Spans.Add(nativeSpan);
			}

			return nativeString;
		}

		void UpdateFormattedText()
		{
			if (Element.FormattedText != null)
				Control.FormattedText = ConvertFormattedText(Element.FormattedText);
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.IsDefault ? s_defaultTextColor : Element.TextColor.ToNative();
		}

		void UpdateTextAlignment()
		{
			Control.HorizontalTextAlignment = Element.HorizontalTextAlignment.ToNative();
			Control.VerticalTextAlignment = Element.VerticalTextAlignment.ToNative();
		}

		void UpdateFontProperties()
		{
			Control.FontSize = Element.FontSize;
			Control.FontAttributes = Element.FontAttributes;
			Control.FontFamily = Element.FontFamily;
		}

		void UpdateLineBreakMode()
		{
			Control.LineBreakMode = ConvertToNativeLineBreakMode(Element.LineBreakMode);
		}

		void UpdateFontWeight()
		{
			Control.FontWeight = Specific.GetFontWeight(Element);
		}

		Native.LineBreakMode ConvertToNativeLineBreakMode(LineBreakMode mode)
		{
			switch (mode)
			{
				case LineBreakMode.CharacterWrap:
					return Native.LineBreakMode.CharacterWrap;
				case LineBreakMode.HeadTruncation:
					return Native.LineBreakMode.HeadTruncation;
				case LineBreakMode.MiddleTruncation:
					return Native.LineBreakMode.MiddleTruncation;
				case LineBreakMode.NoWrap:
					return Native.LineBreakMode.NoWrap;
				case LineBreakMode.TailTruncation:
					return Native.LineBreakMode.TailTruncation;
				case LineBreakMode.WordWrap:
				default:
					return Native.LineBreakMode.WordWrap;
			}
		}
	}
}
