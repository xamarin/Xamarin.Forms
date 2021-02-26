using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using Xunit;
using AColor = Android.Graphics.Color;
using AView = Android.Views.View;

namespace Microsoft.Maui.DeviceTests
{
	internal static class AssertionExtensions
	{
		public static string CreateColorAtPointError(this Bitmap bitmap, AColor expectedColor, int x, int y)
		{
			return CreateColorError(bitmap, $"Expected {expectedColor} at point {x},{y} in renderered view.");
		}

		public static string CreateColorError(this Bitmap bitmap, string message)
		{
			using (var ms = new MemoryStream())
			{
				bitmap.Compress(Bitmap.CompressFormat.Png, 0, ms);
				var imageAsString = Convert.ToBase64String(ms.ToArray());
				return $"{message}. This is what it looked like:<img>{imageAsString}</img>";
			}
		}

		public static AColor ColorAtPoint(this Bitmap bitmap, int x, int y, bool includeAlpha = false)
		{
			int pixel = bitmap.GetPixel(x, y);

			int red = AColor.GetRedComponent(pixel);
			int blue = AColor.GetBlueComponent(pixel);
			int green = AColor.GetGreenComponent(pixel);

			if (includeAlpha)
			{
				int alpha = AColor.GetAlphaComponent(pixel);
				return AColor.Argb(alpha, red, green, blue);
			}
			else
			{
				return AColor.Rgb(red, green, blue);
			}
		}

		public static Bitmap ToBitmap(this AView view)
		{
			var layout = new FrameLayout(view.Context);
			layout.LayoutParameters = new FrameLayout.LayoutParams(500, 500);
			view.LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent)
			{
				Gravity = GravityFlags.Center
			};

			layout.AddView(view);
			layout.Measure(500, 500);
			layout.Layout(0, 0, 500, 500);

			var act = view.Context.GetActivity();
			var rootView = act.FindViewById<FrameLayout>(Android.Resource.Id.Content);

			rootView.AddView(layout);

			var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(bitmap);
			view.Layout(0, 0, view.Width, view.Height);
			view.Draw(canvas);

			rootView.RemoveView(layout);

			return bitmap;
		}

		public static Bitmap AssertColorAtPoint(this Bitmap bitmap, AColor expectedColor, int x, int y)
		{
			Assert.Equal(bitmap.ColorAtPoint(x, y), expectedColor);

			return bitmap;
		}

		public static Bitmap AssertColorAtCenter(this Bitmap bitmap, AColor expectedColor)
		{
			return bitmap.AssertColorAtPoint(expectedColor, bitmap.Width / 2, bitmap.Height / 2);
		}

		public static Bitmap AssertColorAtBottomLeft(this Bitmap bitmap, AColor expectedColor)
		{
			return bitmap.AssertColorAtPoint(expectedColor, 0, 0);
		}

		public static Bitmap AssertColorAtBottomRight(this Bitmap bitmap, AColor expectedColor)
		{
			return bitmap.AssertColorAtPoint(expectedColor, bitmap.Width - 1, 0);
		}

		public static Bitmap AssertColorAtTopLeft(this Bitmap bitmap, AColor expectedColor)
		{
			return bitmap.AssertColorAtPoint(expectedColor, 0, bitmap.Height - 1);
		}

		public static Bitmap AssertColorAtTopRight(this Bitmap bitmap, AColor expectedColor)
		{
			return bitmap.AssertColorAtPoint(expectedColor, bitmap.Width - 1, bitmap.Height - 1);
		}

		public static Bitmap AssertContainsColor(this AView view,  Maui.Color expectedColor) =>
			AssertContainsColor(view, expectedColor.ToNative());

		public static Bitmap AssertContainsColor(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();

			for (int x = 1; x < view.Width; x++)
			{
				for (int y = 1; y < view.Height; y++)
				{
					if (bitmap.ColorAtPoint(x, y, true) == expectedColor)
					{
						return bitmap;
					}
				}
			}

			Assert.True(false, CreateColorError(bitmap, $"Color {expectedColor} not found."));
			return bitmap;
		}

		public static Bitmap AssertColorAtPoint(this AView view, AColor expectedColor, int x, int y)
		{
			var bitmap = view.ToBitmap();
			Assert.Equal(bitmap.ColorAtPoint(x, y), expectedColor);

			return bitmap;
		}

		public static Bitmap AssertColorAtCenter(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();
			return bitmap.AssertColorAtCenter(expectedColor);
		}

		public static Bitmap AssertColorAtBottomLeft(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();
			return bitmap.AssertColorAtBottomLeft(expectedColor);
		}

		public static Bitmap AssertColorAtBottomRight(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();
			return bitmap.AssertColorAtBottomRight(expectedColor);
		}

		public static Bitmap AssertColorAtTopLeft(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();
			return bitmap.AssertColorAtTopLeft(expectedColor);
		}

		public static Bitmap AssertColorAtTopRight(this AView view, AColor expectedColor)
		{
			var bitmap = view.ToBitmap();
			return bitmap.AssertColorAtTopRight(expectedColor);
		}
	}
}