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
			double width = Context.FromPixels(r - l);
			double height = Context.FromPixels(b - t);

			// TODO hartez 2018/07/25 08:51:24 Can this just be element.Layout?	
			Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(_content.Element, new Rectangle(0, 0, width, height));

			_content.UpdateLayout();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int width = MeasureSpec.GetSize(widthMeasureSpec);

			// TODO hartez 2018/07/25 08:50:19 This only correctly measures for vertical right now	
			SizeRequest measure = _content.Element.Measure(Context.FromPixels(width), double.PositiveInfinity, MeasureFlags.IncludeMargins);
			var height = (int)Context.ToPixels(_content.Element.Height > 0 ? _content.Element.Height : measure.Request.Height);

			SetMeasuredDimension(width, height);
		}
	}
}