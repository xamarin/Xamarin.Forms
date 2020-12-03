﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Internals;
using IOPath = System.IO.Path;
using WApplication = Windows.UI.Xaml.Application;

namespace Xamarin.Forms.Platform.UWP
{
	public static class FontExtensions
	{
		static Dictionary<string, FontFamily> FontFamilies = new Dictionary<string, FontFamily>();
		static double DefaultFontSize = double.NegativeInfinity;

		public static void ApplyFont(this Control self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = font.ToFontFamily();
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		public static void ApplyFont(this TextBlock self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = font.ToFontFamily();
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		public static void ApplyFont(this Windows.UI.Xaml.Documents.TextElement self, Font font)
		{
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = font.ToFontFamily();
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static void ApplyFont(this Control self, IFontElement element)
		{
			self.FontSize = element.FontSize;
			self.FontFamily = element.FontFamily.ToFontFamily();
			self.FontStyle = element.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = element.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static double GetFontSize(this NamedSize size)
		{
			// TODO: Hmm, maybe we need to revisit this, since we no longer support Windows Phone OR WinRT.
			// These are values pulled from the mapped sizes on Windows Phone, WinRT has no equivalent sizes, only intents.
			switch (size)
			{
				case NamedSize.Default:
					if(DefaultFontSize == double.NegativeInfinity)
					{
						DefaultFontSize = (double)WApplication.Current.Resources["ControlContentThemeFontSize"];
					}
					return DefaultFontSize;
				case NamedSize.Micro:
					return 15.667;
				case NamedSize.Small:
					return 18.667;
				case NamedSize.Medium:
					return 22.667;
				case NamedSize.Large:
					return 32;
				case NamedSize.Body:
					return 14;
				case NamedSize.Caption:
					return 12;
				case NamedSize.Header:
					return 46;
				case NamedSize.Subtitle:
					return 20;
				case NamedSize.Title:
					return 24;
				default:
					throw new ArgumentOutOfRangeException("size");
			}
		}

		internal static bool IsDefault(this IFontElement self)
		{
			return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
		}

		public static FontFamily ToFontFamily(this Font font) => font.FontFamily.ToFontFamily();
		public static FontFamily ToFontFamily(this string fontFamily)
		{
			if (string.IsNullOrWhiteSpace(fontFamily))
				return (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];

			//Return from Cache!
			if (FontFamilies.TryGetValue(fontFamily, out var f))
			{
				return f;
			}

			//Cache this puppy!
			var formatted = string.Join(", ", GetAllFontPossibilities(fontFamily));
			var font = new FontFamily(formatted);
			FontFamilies[fontFamily] = font;
			return font;
		}

		static string FindFontFamilyName(string fontFile)
		{
			try
			{
				var fontUri = new Uri(fontFile, UriKind.RelativeOrAbsolute);

				// CanvasFontSet only supports ms-appx:// and ms-appdata:// font URIs
				if (fontUri.IsAbsoluteUri && (fontUri.Scheme == "ms-appx" || fontUri.Scheme == "ms-appdata"))
				{
					using (var fontSet = new CanvasFontSet(fontUri))
					{
						if (fontSet.Fonts.Count != 0) 
							return fontSet.GetPropertyValues(CanvasFontPropertyIdentifier.FamilyName).FirstOrDefault().Value;
					}
				}

				return null;
			}
			catch(Exception ex)
			{
				// the CanvasFontSet constructor can throw an exception in case something's wrong with the font. It should not crash the app
				Internals.Log.Warning("Font",$"Error loading font {fontFile}: {ex.Message}");
				return null;
			}
		}

		static IEnumerable<string> GetAllFontPossibilities(string fontFamily)
		{
			//First check Alias
			var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontFamily);
			if (hasFontAlias)
			{
				var familyName = FindFontFamilyName(fontPostScriptName);
				var file = FontFile.FromString(IOPath.GetFileName(fontPostScriptName));
				var formatted = $"{fontPostScriptName}#{familyName ?? file.GetPostScriptNameWithSpaces()}";
				yield return formatted;
				yield break;
			}

			const string path = "Assets/Fonts/";
			string[] extensions = new[]
			{
				".ttf",
				".otf",
			};

			var fontFile = FontFile.FromString(fontFamily);
			//If the extension is provided, they know what they want!
			var hasExtension = !string.IsNullOrWhiteSpace(fontFile.Extension);
			if (hasExtension)
			{
				var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont)
				{
					var familyName = FindFontFamilyName(filePath);
					var formatted = $"{filePath}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";
					yield return formatted;
					yield break;
				}
				else
				{
					yield return $"{path}{fontFile.FileNameWithExtension()}";
				}
			}
			foreach (var ext in extensions)
			{
				var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension(ext));
				if (hasFont)
				{
					var familyName = FindFontFamilyName(filePath);
					var formatted = $"{filePath}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";
					yield return formatted;
					yield break;
				}
			}

			//Always send the base back
			yield return fontFamily;

			foreach (var ext in extensions)
			{
				var fileName = $"{path}{fontFile.FileNameWithExtension(ext)}";
				var familyName = FindFontFamilyName(fileName);
				var formatted = $"{fileName}#{familyName ?? fontFile.GetPostScriptNameWithSpaces()}";
				yield return formatted;
			}
		}
	}
}