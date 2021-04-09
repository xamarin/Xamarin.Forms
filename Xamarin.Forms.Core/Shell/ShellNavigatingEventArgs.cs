using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class ShellNavigatingEventArgs : DeferrableEventArgs
	{
		public ShellNavigatingEventArgs(ShellNavigationState current, ShellNavigationState target,
			ShellNavigationSource source, bool canCancel)
			: base(canCancel)
		{
			Current = current;
			Target = target;
			Source = source;
			Animate = true;
		}

		public ShellNavigationState Current { get; }

		public ShellNavigationState Target { get; }

		public ShellNavigationSource Source { get; }
	}
}