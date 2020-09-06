using System;
using System.Threading;

namespace Xamarin.Forms
{
	public class ShellNavigatingEventArgs : EventArgs
	{
		int _deferalCount;
		Action _deferralFinishedCallback;
		bool _cancelled;

		public ShellNavigatingEventArgs(ShellNavigationState current, ShellNavigationState target, ShellNavigationSource source, bool canCancel)
		{
			Current = current;
			Target = target;
			Source = source;
			CanCancel = canCancel;
			Animate = true;
		}

		public ShellNavigationState Current { get; }

		public ShellNavigationState Target { get; }

		public ShellNavigationSource Source { get; }

		public bool CanCancel { get; }

		public bool Cancel()
		{
			if (!CanCancel)
				return false;

			Cancelled = true;
			return true;
		}

		public bool Cancelled 
		{ 
			get => _cancelled || _deferalCount > 0;
			private set => _cancelled = value; 
		}

		public ShellNavigatingDeferral GetDeferral()
		{
			if (!CanCancel)
				return null;

			DeferredEventArgs = true;
			Interlocked.Increment(ref _deferalCount);
			return new ShellNavigatingDeferral(DecrementDeferral);
		}

		void DecrementDeferral()
		{
			if (Interlocked.Decrement(ref _deferalCount) == 0)
			{
				_deferralFinishedCallback?.Invoke();
				_deferralFinishedCallback = null;
			}
		}

		internal bool Animate { get; set; }
		internal bool DeferredEventArgs { get; set; }

		internal int DeferalCount => _deferalCount;

		internal void RegisterDeferalCallback(Action callback)
		{
			_deferralFinishedCallback = callback;
		}
	}
}