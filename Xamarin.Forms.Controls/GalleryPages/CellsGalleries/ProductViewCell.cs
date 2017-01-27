using System.Linq;

namespace Xamarin.Forms.Controls
{
	public class ProductCellView : StackLayout
	{
		Label _brandLabel;
		StackLayout _stack;
		Label _timeLabel;

		public ProductCellView(string text)
		{
			_stack = new StackLayout();
			_brandLabel = new Label { Text = "BrandLabel", HorizontalTextAlignment = TextAlignment.Center };
			_stack.Children.Add(_brandLabel);

			var frame = new Frame
			{
				Content = _stack,
				BackgroundColor =
					new[] { Device.Android, Device.Windows, Device.WinPhone }.Contains(Device.RuntimePlatform)
						? new Color(0.2)
						: new Color(1)
			};
			_timeLabel = new Label
			{
				Text = text
			};
			Children.Add(_timeLabel);
			Children.Add(frame);
			Padding = new Size(20, 20);
		}
	}
}