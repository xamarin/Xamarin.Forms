using Android.Content;
using Android.Content.PM;
using Android.Support.V7.Widget;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using Android.Graphics;
using Android.Views;

namespace Xamarin.Forms.ControlGallery.Android.Tests
{
	public class PlatformTestFixture
	{
		Context _context;

		protected Context Context
		{
			get
			{
				if (_context == null)
				{
					_context = DependencyService.Resolve<Context>();
				}

				return _context;
			}
		}

		protected static void ToggleRTLSupport(Context context, bool enabled)
		{
			context.ApplicationInfo.Flags = enabled
				? context.ApplicationInfo.Flags | ApplicationInfoFlags.SupportsRtl
				: context.ApplicationInfo.Flags & ~ApplicationInfoFlags.SupportsRtl;
		}

		protected IVisualElementRenderer GetRenderer(VisualElement element) 
		{
			return Platform.Android.Platform.CreateRendererWithContext(element, Context);
		}

		protected TextView GetNativeControl(Label label) 
		{
			var renderer = GetRenderer(label);
			var viewRenderer = renderer.View as ViewRenderer<Label, TextView>;
			return viewRenderer.Control;
		}

		protected FormsEditText GetNativeControl(Entry entry)
		{
			var renderer = GetRenderer(entry);
			var viewRenderer = renderer.View as ViewRenderer<Entry, FormsEditText>;
			return viewRenderer.Control;
		}

		protected AppCompatButton GetNativeControl(Button button)
		{
			var renderer = GetRenderer(button);

			if (renderer is AppCompatButton fastButton)
			{
				return fastButton;
			}

			var viewRenderer = renderer.View as ViewRenderer<Button, AppCompatButton>;
			return viewRenderer.Control;
		}

		protected AColor GetColorAtCenter(AView view) 
		{
			var rect = new Rect();
			rect.Set(0, 0, view.Width, view.Height);

			var bitmap = Bitmap.CreateBitmap(rect.Width(), rect.Height(), Bitmap.Config.Argb8888);
			var canvas = new Canvas(bitmap);
			canvas.Save();
			canvas.Translate(rect.Left, rect.Top);
			view.Draw(canvas);
			canvas.Restore();

			int pixel = bitmap.GetPixel(rect.CenterX(), rect.CenterY());

			int red = AColor.GetRedComponent(pixel);
			int blue = AColor.GetBlueComponent(pixel);
			int green = AColor.GetGreenComponent(pixel);

			return AColor.Rgb(red, green, blue);
		}

		protected void Layout(VisualElement element, AView nativeView) 
		{
			var size = element.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);
			var width = size.Request.Width;
			var height = size.Request.Height;
			element.Layout(new Rectangle(0, 0, width, height));

			int widthSpec = AView.MeasureSpec.MakeMeasureSpec((int)width, MeasureSpecMode.Exactly);
			int heightSpec = AView.MeasureSpec.MakeMeasureSpec((int)height, MeasureSpecMode.Exactly);
			nativeView.Measure(widthSpec, heightSpec);
			nativeView.Layout(0, 0, (int)width, (int)height);
		}
	}
}