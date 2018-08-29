using System;
using Android.Content;
using Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	internal class ItemContentControl : ViewGroup
	{
		protected readonly IVisualElementRenderer Content;

		public ItemContentControl(IVisualElementRenderer content, Context context) : base(context)
		{
			Content = content;
			AddContent();
		}

		void AddContent()
		{
			AddView(Content.View);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var size = Context.FromPixels(r - l, b - t);

			Content.Element.Layout(new Rectangle(Point.Zero, size));

			Content.UpdateLayout();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int width = MeasureSpec.GetSize(widthMeasureSpec);
			int height = MeasureSpec.GetSize(heightMeasureSpec);

			var pixelWidth = Context.FromPixels(width);
			var pixelHeight = Context.FromPixels(height);

			SizeRequest measure = Content.Element.Measure(pixelWidth, pixelHeight, MeasureFlags.IncludeMargins);
			
			width = (int)Context.ToPixels(Content.Element.Width > 0 
				? Content.Element.Width : measure.Request.Width);

			height = (int)Context.ToPixels(Content.Element.Height > 0 
				? Content.Element.Height : measure.Request.Height);

			SetMeasuredDimension(width, height);
		}
	}

	internal class SizedItemContentControl : ItemContentControl
	{
		readonly Func<int> _width;
		readonly Func<int> _height;

		public SizedItemContentControl(IVisualElementRenderer content, Context context, Func<int> width, Func<int> height) 
			: base(content, context)
		{
			_width = width;
			_height = height;
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			// TODO hartez 2018/08/29 15:43:47 Instead of caching width/height, perhaps it should come from the recyclerview directly	
			// as two Func<int>
			//var pixelWidth = Context.FromPixels(_width());
			//var pixelHeight = Context.FromPixels(_height());

			var pixelWidth = _width();
			var pixelHeight =_height();


			Content.Element.Measure(pixelWidth, pixelHeight, MeasureFlags.IncludeMargins);

			SetMeasuredDimension((int)pixelWidth, (int)pixelHeight);
		}
	}
}