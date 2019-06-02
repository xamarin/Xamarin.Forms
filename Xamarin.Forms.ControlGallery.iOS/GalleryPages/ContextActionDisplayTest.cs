using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static Xamarin.Forms.PlatformConfiguration.iOSSpecific.Cell;

namespace Xamarin.Forms.Controls
{
	public abstract class ContextActionDisplayTest : ContentPage
	{
		public ContextActionDisplayTest(ContextActionDisplay contextActionDisplay)
		{
			var template = new DataTemplate(() =>
			{
				var cell = new TextCell();
				cell.On<PlatformConfiguration.iOS>().SetContextActionDisplay(contextActionDisplay);

				cell.SetBinding(TextCell.TextProperty, ".");
				cell.Height = 200;

				cell.ContextActions.Add(new MenuItem { Text = "Entry #1", IconImageSource = new FileImageSource { File = "Entry1.png" }, IsDestructive = true });
				cell.ContextActions.Add(new MenuItem { Text = "Entry #2", IconImageSource = new FileImageSource { File = "Entry2.png" } });
				cell.ContextActions.Add(new MenuItem { Text = "Entry #3", IconImageSource = new FileImageSource { File = "Entry3.png" } });
				cell.ContextActions.Add(new MenuItem { Text = "About", IconImageSource = new FileImageSource { File = "about.png" } });

				return cell;
			});

			var data = new List<string>();
			for (var i = 0; i < 10; i++)
				data.Add($"{i + 1:D3}");

			var list = new ListView();
			list.ItemTemplate = template;
			list.ItemsSource = data;

			Content = list;
		}
	}

	public class ContextActionDisplayIcon : ContextActionDisplayTest
	{
		public ContextActionDisplayIcon() 
			: base(ContextActionDisplay.Icon)
		{ }
	}

	public class ContextActionDisplayText : ContextActionDisplayTest
	{
		public ContextActionDisplayText() 
			: base(ContextActionDisplay.Text)
		{ }
	}
}
