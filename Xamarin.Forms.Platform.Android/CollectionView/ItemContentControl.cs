using Android.Content;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	internal sealed class ItemContentControl : ViewGroup
	{
		readonly IVisualElementRenderer _content;

		public ItemContentControl(IVisualElementRenderer content, Context context) : base(context)
		{
			_content = content;
			AddView(content.View);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var size = Context.FromPixels(r - l, b - t);

			_content.Element.Layout(new Rectangle(Point.Zero, size));

			_content.UpdateLayout();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int width = MeasureSpec.GetSize(widthMeasureSpec);
			int height = MeasureSpec.GetSize(heightMeasureSpec);

			var pixelWidth = Context.FromPixels(width);
			var pixelHeight = Context.FromPixels(height);

			SizeRequest measure = _content.Element.Measure(pixelWidth, pixelHeight, MeasureFlags.IncludeMargins);
			
			width = (int)Context.ToPixels(_content.Element.Width > 0 
				? _content.Element.Width : measure.Request.Width);

			height = (int)Context.ToPixels(_content.Element.Height > 0 
				? _content.Element.Height : measure.Request.Height);

			SetMeasuredDimension(width, height);
		}
	}
}