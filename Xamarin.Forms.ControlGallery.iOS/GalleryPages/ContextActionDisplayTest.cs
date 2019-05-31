using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls
{
	public class ContextActionDisplayTest : ContentPage
	{
		public ContextActionDisplayTest()
		{
			var template = new DataTemplate(() =>
			{
				var cell = new TextCell();
				cell.SetBinding(TextCell.TextProperty, ".");
				cell.Height = 200;

				var textContext = new MenuItem { Text = "Text", IconImageSource = new FileImageSource { File = "about.png" }, IsDestructive = true };
				var iconContext = new MenuItem { Text = "Icon", IconImageSource = new FileImageSource { File = "about.png" } };
				iconContext.On<iOS>().SetContextActionDisplay(ContextActionDisplay.Icon);

				cell.ContextActions.Add(textContext);
				cell.ContextActions.Add(iconContext);

				return cell;
			});

			var data = new List<string>();
			for (var i = 0; i < 10; i++)
				data.Add($"{i+1:D3}");

			var list = new ListView();
			list.ItemTemplate = template;
			list.ItemsSource = data;

			Content = list;
		}
	}
}
