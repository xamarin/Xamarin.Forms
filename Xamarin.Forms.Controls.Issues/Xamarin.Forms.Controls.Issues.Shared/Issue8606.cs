using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8606, "[Bug] [UWP] FontIcons are not aligned correctly", PlatformAffected.UWP)]
#if UITEST
	[Category(UITestCategories.Github5000)]
	[Category(UITestCategories.ManualReview)]
#endif
	public class Issue8606 : TestContentPage
	{
		protected override void Init()
		{
			var iconColor = Color.White;
			List<(string fontFamily, string glyph, string familyShortName)> fontFamilyGlyphs = new List<(string, string, string)>
			{
				(GetFontFamily("fa-solid-900.ttf","Font Awesome 5 Free"), GetGlyph("f059"),"FaSolid"),
				(GetFontFamily("ionicons.ttf","Ionicons"), GetGlyph("f142"),"Ionicons"),
				(GetFontFamily("materialdesignicons-webfont.ttf","Material Design Icons"),GetGlyph("f625"),"Material old"),
				(GetFontFamily("MaterialIconsOutlined-Regular.otf","Material Icons Outlined"), GetGlyph("e8fd"),"Material"),
			};

			List<Func<FontImageSource, View>> affectedViewsCreators = new List<Func<FontImageSource, View>>
			{
				(fontImageSource) => new Button { ImageSource = fontImageSource },
				(fontImageSource) => new ImageButton { WidthRequest=39,HeightRequest=39, Source = fontImageSource, BackgroundColor = Color.FromHex("#333333")},
				(fontImageSource) => new Frame{Content = new Image {  Source = fontImageSource}, BorderColor=iconColor, Padding=0,  }
			};

			var content = new StackLayout { };
			content.Children.Add(new Label { BackgroundColor = Color.Black, Padding = 12, TextColor = iconColor, Text = "Button, ImageButton and Image should use the same FontImageSourceHandler which should render centered." });

			foreach (var (fontFamily, glyph, familyShortName) in fontFamilyGlyphs)
			{
				var fontImageSource = new FontImageSource { Size = 24, Color = iconColor, FontFamily = fontFamily, Glyph = glyph };
				var fontStackLayout = new StackLayout { };
				fontStackLayout.Children.Add(new Label { FontSize = 24, Text = familyShortName });
				var fontsStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
				fontStackLayout.Children.Add(fontsStackLayout);
				foreach (var affectedViewCreator in affectedViewsCreators)
				{
					var affectedView = affectedViewCreator(fontImageSource);
					affectedView.VerticalOptions = LayoutOptions.Center;
					fontsStackLayout.Children.Add(affectedView);
				}
				content.Children.Add(fontStackLayout);
			}

			Content = content;

		}

		private string GetFontFamily(string fileName, string fontName)
		{
			return $"Assets/Fonts/{fileName}#{fontName}";
		}

		private static string GetGlyph(string codePoint)
		{
			int p = int.Parse(codePoint, System.Globalization.NumberStyles.HexNumber);
			return char.ConvertFromUtf32(p);
		}
	}
}
