using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	internal class Tweener
	{
		long _lastMilliseconds;

		int _timer;

		public Tweener(uint length)
		{
			Value = 0.0f;
			Length = length;
			Loop = false;
		}

		public AnimatableKey Handle { get; set; }

		public uint Length { get; }

		public bool Loop { get; set; }

		public double Value { get; private set; }

		public event EventHandler Finished;

		public void Pause()
		{
			if (_timer != 0)
			{
				Ticker.Default.Remove(_timer);
				_timer = 0;
			}
		}

		public void Start()
		{
			Pause();

			_lastMilliseconds = 0;
			_timer = Ticker.Default.Insert(step =>
			{
				long ms = step + _lastMilliseconds;

				Value = Math.Min(1.0f, ms / (double)Length);

				_lastMilliseconds = ms;

				if (ValueUpdated != null)
					ValueUpdated(this, EventArgs.Empty);

				if (Value >= 1.0f)
				{
					if (Loop)
					{
						_lastMilliseconds = 0;
						Value = 0.0f;
						return true;
					}

					if (Finished != null)
						Finished(this, EventArgs.Empty);
					Value = 0.0f;
					_timer = 0;
					return false;
				}
				return true;
			});
		}

		public void Stop()
		{
			Pause();
			Value = 1.0f;
			if (Finished != null)
				Finished(this, EventArgs.Empty);
			Value = 0.0f;
		}

		public event EventHandler ValueUpdated;

		~Tweener()
		{
			if (_timer != 0)
			{
				try
				{
					Ticker.Default.Remove(_timer);
				}
				catch (InvalidOperationException)
				{
				}
			}
			_timer = 0;
		}
	}
}