using System;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using Android.OS;
using AColor = Android.Graphics.Color;

namespace Xamarin.Forms.Platform.Android
{
	internal class FormsImageView : ImageView
	{
		Movie _movie;
		bool _play;
		long _startTime;
		bool _skipInvalidate;

		public FormsImageView(Context context) : base(context)
		{
		}

		protected FormsImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void Invalidate()
		{
			if (_skipInvalidate)
			{
				_skipInvalidate = false;
				return;
			}

			base.Invalidate();
		}

		public void SkipInvalidate()
		{
			_skipInvalidate = true;
		}

		public void SetAnimatedImage(Movie movie, bool autoStart)
		{
			_movie = movie;
			_play = autoStart;
		}

		public void InitializeSource()
		{
			_movie?.Dispose();
			_movie = null;
		}

		public void PlayAnimation()
		{
			_play = true;
			Invalidate();
		}

		public void StopAnimation()
		{
			_play = false;
		}

		protected override void OnDraw(Canvas canvas)
		{
			canvas.DrawColor(AColor.Transparent);
			base.OnDraw(canvas);

			if (_movie != null && _play)
				DrawGifImage(canvas);
		}

		void DrawGifImage(Canvas canvas)
		{
			long now = SystemClock.UptimeMillis();
			if (_startTime == 0)
				_startTime = now;
			int duration = _movie.Duration();
			int relTime = (int)((now - _startTime) % duration);
			_movie.SetTime(relTime);
			_movie.Draw(canvas, 0, 0);
			if (_play)
				Invalidate();
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
			if (_movie != null)
				SetMeasuredDimension(_movie.Width(), _movie.Height());
		}
	}
}