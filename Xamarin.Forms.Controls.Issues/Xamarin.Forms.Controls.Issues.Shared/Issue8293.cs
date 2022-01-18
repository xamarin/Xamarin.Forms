using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System;
using System.Threading;

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
	[Issue(IssueTracker.Github, 8293, "[Bug] Font Icon Disappears when Minimizing Window in UWP", PlatformAffected.UWP)]
	public class Issue8293 : TestContentPage
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout
			{
				AutomationId = "Issue8293Label"
			};

			var iconColor = Color.White;
			var fontImageSource = new FontImageSource { Size = 24, Color = Color.White, FontFamily = GetFontFamily("materialdesignicons-webfont.ttf", "Material Design Icons"), Glyph = GetGlyph("f625") };

			List<Func<FontImageSource, View>> affectedViewsCreators = new List<Func<FontImageSource, View>>
			{
				(fontImageSource) => new Button { ImageSource = fontImageSource },
				(fontImageSource) => new ImageButton { WidthRequest=39,HeightRequest=39, Source = fontImageSource, BackgroundColor = Color.FromHex("#333333")},
				(fontImageSource) => new Frame{Content = new Image {  Source = fontImageSource}, BorderColor=iconColor, Padding=0,  }
			};

			foreach (var affectedViewCreator in affectedViewsCreators)
			{
				var affectedView = affectedViewCreator(fontImageSource);
				stackLayout.Children.Add(affectedView);
			}

			Content = stackLayout;

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