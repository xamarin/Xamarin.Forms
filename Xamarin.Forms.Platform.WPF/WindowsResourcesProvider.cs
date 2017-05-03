using System;
using System.Windows;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	internal sealed class WindowsResourcesProvider : ISystemResourcesProvider
	{
		public IResourceDictionary GetSystemResources()
		{
			System.Windows.ResourceDictionary windowsResources = System.Windows.Application.Current.Resources;

			var resources = new ResourceDictionary();
			resources[Device.Styles.TitleStyleKey] = GetStyle("HeaderTextBlockStyle");
			resources[Device.Styles.SubtitleStyleKey] = GetStyle("SubheaderTextBlockStyle");
			resources[Device.Styles.BodyStyleKey] = GetStyle("BodyTextBlockStyle");
			resources[Device.Styles.CaptionStyleKey] = GetStyle("CaptionTextBlockStyle");
#if WINDOWS_UWP
			resources[Device.Styles.ListItemTextStyleKey] = GetStyle("BaseTextBlockStyle");
#else
			resources[Device.Styles.ListItemTextStyleKey] = GetStyle("TitleTextBlockStyle");
#endif
			resources[Device.Styles.ListItemDetailTextStyleKey] = GetStyle("BodyTextBlockStyle");
			return resources;
		}

		Style GetStyle(object nativeKey)
		{
			var style = (System.Windows.Style)System.Windows.Application.Current.Resources[nativeKey];

			var formsStyle = new Style(typeof(Label));
			foreach (var b in style.Setters)
			{
			    var setter = b as System.Windows.Setter;
				if (setter == null)
					continue;
				
				try
				{
					if (setter.Property == TextBlock.FontSizeProperty)
						formsStyle.Setters.Add(Label.FontSizeProperty, setter.Value);
					else if (setter.Property == TextBlock.FontFamilyProperty)
						formsStyle.Setters.Add(Label.FontFamilyProperty, setter.Value);
					else if (setter.Property == TextBlock.FontWeightProperty)
						formsStyle.Setters.Add(Label.FontAttributesProperty, setter.Value);
					else if (setter.Property == TextBlock.TextWrappingProperty)
						formsStyle.Setters.Add(Label.LineBreakModeProperty, ToLineBreakMode((TextWrapping)setter.Value));
				}
				catch (NotImplementedException)
				{
					// see https://bugzilla.xamarin.com/show_bug.cgi?id=33135
					// WinRT implementation of Windows.UI.Xaml.Setter.get_Value is not implemented.
				}
			}

			return formsStyle;
		}
		
		static LineBreakMode ToLineBreakMode(TextWrapping value)
		{
			switch (value)
			{
				case TextWrapping.Wrap:
					return LineBreakMode.CharacterWrap;
				case TextWrapping.WrapWithOverflow:
					return LineBreakMode.WordWrap;
				default:
				case TextWrapping.NoWrap:
					return LineBreakMode.NoWrap;
			}
		}
	}
}