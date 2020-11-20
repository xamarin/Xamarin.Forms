using System;
using System.Runtime.InteropServices;
using ElmSharp;
using SkiaSharp.Views.Tizen;

namespace Xamarin.Forms.Platform.Tizen.SkiaSharp
{
	public class SKCliperView : SKCanvasView
	{
		public SKCliperView(EvasObject parent) : base(parent) { }

		public new void Invalidate()
		{
			OnDrawFrame();
		}

	}

	public static class CliperExtension
	{
		public static void SetCliperCanvas(this VisualElement target, SKCliperView cliper)
		{
			if (target != null)
			{
				var nativeView = Platform.GetOrCreateRenderer(target)?.NativeView;
				var realHandle = elm_object_part_content_get(cliper, "elm.swallow.content");

				nativeView?.SetClip(null); // To restore original image
				evas_object_clip_set(nativeView, realHandle);
			}
		}

		[DllImport("libevas.so.1")]
		internal static extern void evas_object_clip_set(IntPtr obj, IntPtr clip);

		[DllImport("libelementary.so.1")]
		internal static extern IntPtr elm_object_part_content_get(IntPtr obj, string part);
	}
}
