using System.ComponentModel;
using Android.Content;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	public class ImageCellRenderer : TextCellRenderer
	{
		protected override global::Android.Views.View GetCellCore(Cell item, global::Android.Views.View convertView, ViewGroup parent, Context context)
		{
			var result = (BaseCellView)base.GetCellCore(item, convertView, parent, context);

			UpdateImage();
			UpdateFlowDirection();
			UpdateBackgroundColor();

			return result;
		}

		protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnCellPropertyChanged(sender, args);
			if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
				UpdateImage();
			else if (args.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
			else if (args.PropertyName == Cell.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
		}

		void UpdateImage()
		{
			var cell = (ImageCell)Cell;
			if (cell.ImageSource != null)
			{
				View.SetImageVisible(true);
				View.SetImageSource(cell.ImageSource);
			}
			else
				View.SetImageVisible(false);
		}

		void UpdateFlowDirection()
		{
			View.UpdateFlowDirection(ParentView);
		}

		void UpdateBackgroundColor()
		{
			if (View == null)
				return;

			var cell = (ImageCell)Cell;

			if (cell.IsSet(Cell.BackgroundColorProperty))
				View.SetBackgroundColor(cell.BackgroundColor.ToAndroid());
			else
				View.Background?.ClearColorFilter();
		}
	}
}