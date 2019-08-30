using System;
using Xamarin.Forms.Internals;
using Android.OS;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Xamarin.Forms.Platform.Android
{
	// all this animation code will go away if/once we pull in GlideX
	internal class FormsAnimationDrawableStateEventArgs : EventArgs
	{
		public FormsAnimationDrawableStateEventArgs(bool finished)
		{
			Finished = finished;
		}

		public bool Finished { get; set; }
	}

	internal interface IFormsAnimationDrawable : IDisposable
	{
		event EventHandler AnimationStarted;
		event EventHandler<FormsAnimationDrawableStateEventArgs> AnimationStopped;

		int RepeatCount { get; set; }

		bool IsRunning { get; }

		Drawable ImageDrawable { get; }

		void Start();
		void Stop();

	}

	internal class FormsAnimationDrawable : AnimationDrawable, IFormsAnimationDrawable
	{
		int _repeatCounter = 0;
		int _frameCount = 0;
		bool _finished = false;
		bool _isRunning = false;

		public FormsAnimationDrawable()
		{
			RepeatCount = int.MaxValue;

			if (!Forms.IsLollipopOrNewer)
				base.SetVisible(false, true);
		}

		public int RepeatCount { get; set; }

		public event EventHandler AnimationStarted;
		public event EventHandler<FormsAnimationDrawableStateEventArgs> AnimationStopped;

		public override bool IsRunning
		{
			get { return _isRunning; }
		}

		public Drawable ImageDrawable
		{
			get { return this; }
		}

		public override void Start()
		{
			_repeatCounter = 0;
			_frameCount = NumberOfFrames;
			_finished = false;

			base.OneShot = RepeatCount == 1 ? true : false;

			base.Start();

			if (!Forms.IsLollipopOrNewer)
				base.SetVisible(true, true);

			_isRunning = true;
			AnimationStarted?.Invoke(this, null);
		}

		public override void Stop()
		{
			base.Stop();

			if (!Forms.IsLollipopOrNewer)
				base.SetVisible(false, true);

			_isRunning = false;
			AnimationStopped?.Invoke(this, new FormsAnimationDrawableStateEventArgs(_finished));
		}

		public override bool SelectDrawable(int index)
		{
			if (!_isRunning)
				return base.SelectDrawable(0);

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


		public static Task<IFormsAnimationDrawable> LoadImageAnimationAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			switch (imagesource)
			{
				case FileImageSource fis:
					return LoadImageAnimationAsync(fis, context, cancelationToken);
				case StreamImageSource sis:
					return LoadImageAnimationAsync(sis, context, cancelationToken);
			}

			return Task.FromResult<IFormsAnimationDrawable>(null);
		}



		public async Task<IFormsAnimationDrawable> LoadImageAnimationAsync(StreamImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			var streamSource = imagesource as StreamImageSource;
			FormsAnimationDrawable animation = null;
			if (streamSource?.Stream != null)
			{
				using (Stream stream = await ((IStreamImageSource)streamSource).GetStreamAsync(cancelationToken).ConfigureAwait(false))
				{
					int sourceDensity = 1;
					int targetDensity = 1;

					if (stream.CanSeek)
					{
						BitmapFactory.Options options = new BitmapFactory.Options();
						options.InJustDecodeBounds = true;
						await BitmapFactory.DecodeStreamAsync(stream, null, options);
						sourceDensity = options.InDensity;
						targetDensity = options.InTargetDensity;
						stream.Seek(0, SeekOrigin.Begin);
					}

					using (var decoder = new AndroidGIFImageParser(context, sourceDensity, targetDensity))
					{
						try
						{
							await decoder.ParseAsync(stream).ConfigureAwait(false);
							animation = decoder.Animation;
						}
						catch (GIFDecoderFormatException)
						{
							animation = null;
						}
					}
				}
			}

			if (animation == null)
			{
				Internals.Log.Warning(nameof(ImageLoaderSourceHandler), "Image data was invalid: {0}", streamSource);
			}

			return animation;
		}

		public static async Task<IFormsAnimationDrawable> LoadImageAnimationAsync(FileImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			string file = ((FileImageSource)imagesource).File;
			FormsAnimationDrawable animation = null;

			BitmapFactory.Options options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true
			};

			if (!FileImageSourceHandler.DecodeSynchronously)
				await BitmapFactory.DecodeResourceAsync(context.Resources, ResourceManager.GetDrawableByName(file), options);
			else
				BitmapFactory.DecodeResource(context.Resources, ResourceManager.GetDrawableByName(file), options);

			using (var stream = context.Resources.OpenRawResource(ResourceManager.GetDrawableByName(file)))
			using (var decoder = new AndroidGIFImageParser(context, options.InDensity, options.InTargetDensity))
			{
				try
				{
					if (!FileImageSourceHandler.DecodeSynchronously)
						await decoder.ParseAsync(stream).ConfigureAwait(false);
					else
						decoder.ParseAsync(stream).Wait();

					animation = decoder.Animation;
				}
				catch (GIFDecoderFormatException)
				{
					animation = null;
				}
			}

			if (animation == null)
			{
				Internals.Log.Warning(nameof(FileImageSourceHandler), "Could not retrieve image or image data was invalid: {0}", imagesource);
			}

			return animation;
		}

	}

	class AndroidGIFImageParser : GIFImageParser, IDisposable
	{
		readonly DisplayMetrics _metrics = Resources.System.DisplayMetrics;
		Context _context;
		int _sourceDensity;
		int _targetDensity;
		Bitmap _currentBitmap;
		bool _disposed;

		public AndroidGIFImageParser(Context context, int sourceDensity, int targetDensity)
		{
			_context = context;
			_sourceDensity = sourceDensity;
			_targetDensity = targetDensity;
			Animation = new FormsAnimationDrawable();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public FormsAnimationDrawable Animation { get; private set; }

		protected override void StartParsing()
		{
			System.Diagnostics.Debug.Assert(!Animation.IsRunning);
			System.Diagnostics.Debug.Assert(Animation.NumberOfFrames == 0);
			System.Diagnostics.Debug.Assert(_currentBitmap == null);
		}

		protected override void AddBitmap(GIFHeader header, GIFBitmap gifBitmap, bool ignoreImageData)
		{
			if (!ignoreImageData)
			{
				Bitmap bitmap;

				if (_sourceDensity < _targetDensity)
				{
					if (_currentBitmap == null)
						_currentBitmap = Bitmap.CreateBitmap(header.Width, header.Height, Bitmap.Config.Argb8888);

					System.Diagnostics.Debug.Assert(_currentBitmap.Width == header.Width);
					System.Diagnostics.Debug.Assert(_currentBitmap.Height == header.Height);

					_currentBitmap.SetPixels(gifBitmap.Data, 0, header.Width, 0, 0, header.Width, header.Height);

					float scaleFactor = (float)_targetDensity / (float)_sourceDensity;
					int scaledWidth = (int)(scaleFactor * header.Width);
					int scaledHeight = (int)(scaleFactor * header.Height);

					bitmap = Bitmap.CreateScaledBitmap(_currentBitmap, scaledWidth, scaledHeight, true);

					System.Diagnostics.Debug.Assert(!_currentBitmap.Equals(bitmap));
				}
				else
				{
					bitmap = Bitmap.CreateBitmap(gifBitmap.Data, header.Width, header.Height, Bitmap.Config.Argb8888);
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
			if (_currentBitmap != null)
			{
				_currentBitmap.Recycle();
				_currentBitmap.Dispose();
				_currentBitmap = null;
			}

			System.Diagnostics.Debug.Assert(!Animation.IsRunning);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (!disposing)
				return;

			if (_currentBitmap != null)
			{
				_currentBitmap.Recycle();
				_currentBitmap.Dispose();
				_currentBitmap = null;
			}

			_disposed = true;
		}
	}
}