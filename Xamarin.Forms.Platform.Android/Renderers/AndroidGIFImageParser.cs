using System;
using Xamarin.Forms.Internals;
using Android.OS;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;

namespace Xamarin.Forms.Platform.Android
{
	public class FormsAnimationDrawableStateEventArgs : EventArgs
	{
		public FormsAnimationDrawableStateEventArgs(bool finished)
		{
			Finished = finished;
		}

		public bool Finished { get; set; }
	}

	public class FormsAnimationDrawable : AnimationDrawable
	{
		int _repeatCounter = 0;
		int _frameCount = 0;
		bool _finished = false;

		public FormsAnimationDrawable()
		{
			RepeatCount = int.MaxValue;
		}

		public int RepeatCount { get; set; }

		public event EventHandler AnimationStarted;
		public event EventHandler<FormsAnimationDrawableStateEventArgs> AnimationStopped;

		public override void Start()
		{
			_repeatCounter = 0;
			_frameCount = NumberOfFrames;
			_finished = false;

			base.Start();
			AnimationStarted?.Invoke(this, null);
		}

		public override void Stop()
		{
			base.Stop();
			AnimationStopped?.Invoke(this, new FormsAnimationDrawableStateEventArgs(_finished));
		}

		public override bool SelectDrawable(int index)
		{
			// Restarted animation, reached max number of repeats?
			if (_repeatCounter >= RepeatCount)
			{
				_finished = true;

				// Stop can't be done from within this method.
				new Handler(Looper.MainLooper).Post(() => {
					if (this.IsRunning)
						this.Stop();
				});

				// Until stopped, show first image.
				return base.SelectDrawable(0);
			}

			// Hitting last frame?
			if (index != 0 && index == _frameCount - 1)
				_repeatCounter++;

			return base.SelectDrawable(index);
		}
	}

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
			Animation = new FormsAnimationDrawable();
		}

		public FormsAnimationDrawable Animation { get; private set; }

		protected override void StartParsing()
		{
			System.Diagnostics.Debug.Assert(!Animation.IsRunning);
			System.Diagnostics.Debug.Assert(Animation.NumberOfFrames == 0);
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

					System.Diagnostics.Debug.Assert(!originalBitmap.Equals(bitmap));

					originalBitmap.Recycle();
					originalBitmap.Dispose();
					originalBitmap = null;
				}

				// Frame delay compability adjustment in milliseconds.
				int delay = gifBitmap.Delay;
				if (delay <= 20)
					delay = 100;

				Animation.AddFrame(new BitmapDrawable(_context.Resources, bitmap), delay);

				if (gifBitmap.LoopCount != 0)
					Animation.RepeatCount = gifBitmap.LoopCount;
			}
		}

		protected override void FinishedParsing()
		{
			System.Diagnostics.Debug.Assert(!Animation.IsRunning);
		}
	}
}