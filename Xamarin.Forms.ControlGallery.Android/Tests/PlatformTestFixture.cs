using Android.Content;
using Android.Content.PM;
using Android.Support.V7.Widget;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using Android.Graphics;
using Android.Views;
using System;
using AProgressBar = Android.Widget.ProgressBar;
using ASearchView = Android.Widget.SearchView;

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
			return element.GetRenderer() ?? Platform.Android.Platform.CreateRendererWithContext(element, Context);
		}

		protected AView GetNativeControl(VisualElement element)
		{
			switch (element)
			{
				case Button button:
					return GetNativeControl(button);
				case CheckBox checkBox:
					return GetNativeControl(checkBox);
				case DatePicker datePicker:
					return GetNativeControl(datePicker);
				case Editor editor:
					return GetNativeControl(editor);
				case Entry entry:
					return GetNativeControl(entry);
				case Image image:
					return GetNativeControl(image);
				case Label label:
					return GetNativeControl(label);
				case Picker picker:
					return GetNativeControl(picker);
				case ProgressBar progressBar:
					return GetNativeControl(progressBar);
				case SearchBar searchBar:
					return GetNativeControl(searchBar);
				case Stepper stepper:
					return GetNativeControl(stepper);
				case Switch @switch:
					return GetNativeControl(@switch);
				case TimePicker timePicker:
					return GetNativeControl(timePicker);
			}

			throw new NotImplementedException($"Don't know how to get the native control for {element}");
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

		protected AppCompatCheckBox GetNativeControl(CheckBox checkbox)
		{
			var renderer = GetRenderer(checkbox);
			return renderer as AppCompatCheckBox;
		}

		protected EditText GetNativeControl(DatePicker datePicker)
		{
			var renderer = GetRenderer(datePicker);
			var viewRenderer = renderer.View as ViewRenderer<DatePicker, EditText>;
			return viewRenderer.Control;
		}

		protected FormsEditText GetNativeControl(Editor editor)
		{
			var renderer = GetRenderer(editor);
			var viewRenderer = renderer.View as ViewRenderer<Editor, FormsEditText>;
			return viewRenderer.Control;
		}

		protected FormsEditText GetNativeControl(Entry entry)
		{
			var renderer = GetRenderer(entry);
			var viewRenderer = renderer.View as ViewRenderer<Entry, FormsEditText>;
			return viewRenderer.Control;
		}

		protected ImageView GetNativeControl(Image image)
		{
			var renderer = GetRenderer(image);

			if (renderer is ImageView fastImage)
			{
				return fastImage;
			}

			var viewRenderer = renderer.View as ViewRenderer<Image, ImageView>;
			return viewRenderer.Control;
		}

		protected TextView GetNativeControl(Label label) 
		{
			var renderer = GetRenderer(label);
			var viewRenderer = renderer.View as ViewRenderer<Label, TextView>;
			return viewRenderer.Control;
		}

		protected EditText GetNativeControl(Picker picker)
		{
			var renderer = GetRenderer(picker);
			var viewRenderer = renderer.View as ViewRenderer<Picker, EditText>;
			return viewRenderer.Control;
		}

		protected AProgressBar GetNativeControl(ProgressBar progressBar)
		{
			var renderer = GetRenderer(progressBar);
			var viewRenderer = renderer.View as ViewRenderer<ProgressBar, AProgressBar>;
			return viewRenderer.Control;
		}

		protected ASearchView GetNativeControl(SearchBar searchBar)
		{
			var renderer = GetRenderer(searchBar);
			var viewRenderer = renderer.View as ViewRenderer<SearchBar, ASearchView>;
			return viewRenderer.Control;
		}

		protected LinearLayout GetNativeControl(Stepper stepper)
		{
			var renderer = GetRenderer(stepper);
			var viewRenderer = renderer.View as ViewRenderer<Stepper, LinearLayout>;
			return viewRenderer.Control;
		}

		protected SwitchCompat GetNativeControl(Switch @switch)
		{
			var renderer = GetRenderer(@switch);
			var viewRenderer = renderer.View as ViewRenderer<Switch, SwitchCompat>;
			return viewRenderer.Control;
		}

		protected EditText GetNativeControl(TimePicker timePicker)
		{
			var renderer = GetRenderer(timePicker);
			var viewRenderer = renderer.View as ViewRenderer<TimePicker, EditText>;
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

		// Some of the renderer properties aren't set until the renderer is actually 
		// attached to the view hierarchy; to test them, we need to add the, run the test,
		// then remove them
		protected void ParentView(AView view) 
		{
			((ViewGroup)Application.Current.MainPage.GetRenderer().View).AddView(view);
		}

		protected void UnparentView(AView view)
		{
			((ViewGroup)Application.Current.MainPage.GetRenderer().View).RemoveView(view);
		}
	}
}