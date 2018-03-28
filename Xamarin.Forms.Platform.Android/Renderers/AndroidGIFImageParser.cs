using System.Diagnostics;
using Xamarin.Forms.Internals;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;

namespace Xamarin.Forms.Platform.Android
{
	class AndroidGIFImageParser : GIFImageParser
	{
		readonly DisplayMetrics _metrics = Resources.System.DisplayMetrics;
		Context _context;
		int _sourceDensity;
		int _targetDensity;

		public AndroidGIFImageParser(Context context, int sourceDensity, int targetDensity)
		{
			_context = context;
			_sourceDensity = sourceDensity;
			_targetDensity = targetDensity;
			Animation = new AnimationDrawable();
		}

		public AnimationDrawable Animation { get; private set; }

		protected override void StartParsing()
		{
			Debug.Assert(!Animation.IsRunning);
			Debug.Assert(Animation.NumberOfFrames == 0);
		}

		protected override void AddBitmap(GIFHeader header, GIFBitmap gifBitmap, bool ignoreImageData)
		{
			if (!ignoreImageData)
			{
				Bitmap bitmap;
				bitmap = Bitmap.CreateBitmap(gifBitmap.Data, header.Width, header.Height, Bitmap.Config.Argb4444);

				if (_sourceDensity < _targetDensity)
				{
					var originalBitmap = bitmap;

					float scaleFactor = (float)_targetDensity / (float)_sourceDensity;

					int scaledWidth = (int)(scaleFactor * header.Width);
					int scaledHeight = (int)(scaleFactor * header.Height);
					bitmap = Bitmap.CreateScaledBitmap(originalBitmap, scaledWidth, scaledHeight, true);

					Debug.Assert(!originalBitmap.Equals(bitmap));

					originalBitmap.Recycle();
					originalBitmap.Dispose();
					originalBitmap = null;
				}

				// Frame delay compability adjustment in milliseconds.
				int delay = gifBitmap.Delay;
				if (delay <= 20)
					delay = 100;

				Animation.AddFrame(new BitmapDrawable(_context.Resources, bitmap), delay);
			}
		}

		protected override void FinishedParsing()
		{
			Debug.Assert(!Animation.IsRunning);
		}
	}
}