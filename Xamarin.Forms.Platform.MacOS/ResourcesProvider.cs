using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class ResourcesProvider : ISystemResourcesProvider
	{
		ResourceDictionary _dictionary;

		public ResourcesProvider()
		{

		}

		public IResourceDictionary GetSystemResources()
		{
			_dictionary = new ResourceDictionary();
			//UpdateStyles();

			return _dictionary;
		}

		//void UpdateStyles()
		//{

		//	_dictionary[Device.Styles.TitleStyleKey] = GenerateStyle(NSFont.BoldSystemFontOfSize(17));
		//	_dictionary[Device.Styles.SubtitleStyleKey] = GenerateStyle(NSFont.SystemFontOfSize(15));
		//	_dictionary[Device.Styles.BodyStyleKey] = GenerateStyle(NSFont.SystemFontOfSize(17));
		//	_dictionary[Device.Styles.CaptionStyleKey] = GenerateStyle(NSFont.SystemFontOfSize(12));

		//	_dictionary[Device.Styles.ListItemTextStyleKey] = GenerateListItemTextStyle();
		//	_dictionary[Device.Styles.ListItemDetailTextStyleKey] = GenerateListItemDetailTextStyle();
		//}

		//Style GenerateListItemDetailTextStyle()
		//{
		//	var font = new NSTableViewCell()NSTableViewCellStyle.Subtitle, "Foobar").DetailTextLabel.Font;
		//	return GenerateStyle(font);
		//}

		//Style GenerateListItemTextStyle()
		//{
		//	var font = new UITableViewCell(UITableViewCellStyle.Subtitle, "Foobar").TextLabel.Font;
		//	return GenerateStyle(font);
		//}

		//Style GenerateStyle(NSFont font)
		//{
		//	var result = new Style(typeof(Label));

		//	result.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = (double)font.PointSize });

		//	result.Setters.Add(new Setter { Property = Label.FontFamilyProperty, Value = font.FontName });

		//	return result;
		//}
	}
}

