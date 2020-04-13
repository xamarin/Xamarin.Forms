﻿using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

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

			if (renderer is Xamarin.Forms.Platform.Android.FastRenderers.LabelRenderer fastRenderer)
			{
				return fastRenderer;
			}			
			
			var viewRenderer = renderer.View as ViewRenderer<Label, TextView>;
			return viewRenderer.Control;
		}

		protected global::Android.Widget.Button GetNativeControl(Button button)
		{
			var renderer = GetRenderer(button);

			if (renderer is Xamarin.Forms.Platform.Android.FastRenderers.ButtonRenderer fastRenderer)
			{
				return fastRenderer;
			}

			var viewRenderer = renderer.View as ViewRenderer<Button, global::Android.Widget.Button>;
			return viewRenderer.Control;
		}

		protected FormsEditText GetNativeControl(Entry entry)
		{
			var renderer = GetRenderer(entry);
			var viewRenderer = renderer.View as ViewRenderer<Entry, FormsEditText>;
			return viewRenderer.Control;
		}
	}
}