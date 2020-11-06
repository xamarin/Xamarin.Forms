using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public static class DispatcherExtensions
	{
		static IDispatcherProvider s_current;
		static IDispatcher s_default;

		public static IDispatcher GetDispatcher(this BindableObject bindableObject)
		{
			if (s_default != null)
			{
				// If we're already using the fallback dispatcher, keep using it
				return s_default;
			}

			// See if the current platform has a DispatcherProvider for us
			s_current = s_current ?? DependencyService.Get<IDispatcherProvider>();

			if (s_current == null)
			{
				// No DispatcherProvider available, use the fallback dispatcher
				s_default = new FallbackDispatcher();
				return s_default;
			}

			// Use the DispatcherProvider to retrieve an appropriate dispatcher for this BindableObject
			return s_current.GetDispatcher(bindableObject) ?? new FallbackDispatcher();
		}

		internal static void Dispatch(this IDispatcher dispatcher, Action action)
		{
			if (dispatcher != null)
			{
				if (dispatcher.IsInvokeRequired)
				{
					dispatcher.BeginInvokeOnMainThread(action);
				}
				else
				{
					action();
				}
			}
			else
			{
				if (Device.IsInvokeRequired)
				{
					Device.BeginInvokeOnMainThread(action);
				}
				else
				{
					action();
				}
			}
		}

		internal static Task<int> GetThreadIdAsync(this IDispatcher dispatcher)
		{
			var tcs = new TaskCompletionSource<int>();

			if (!dispatcher.IsInvokeRequired)
			{
#if NETSTANDARD2_0
				tcs.SetResult(Thread.CurrentThread.ManagedThreadId);
#else
				tcs.SetResult(0);
#endif
				return tcs.Task;
			}

			dispatcher.BeginInvokeOnMainThread(() =>
			{
				try
				{

#if NETSTANDARD2_0
					tcs.SetResult(Thread.CurrentThread.ManagedThreadId);
#else
					tcs.SetResult(0);
#endif
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			});

			return tcs.Task;
		}

		static ConditionalWeakTable<IDispatcher, object> ThreadIds = new ConditionalWeakTable<IDispatcher, object>();

		public static int GetThreadId(this IDispatcher dispatcher)
		{
			if (ThreadIds.TryGetValue(dispatcher, out object threadId))
			{
				return (int)threadId;
			}

			var t = dispatcher.GetThreadIdAsync();
			var result = t.ConfigureAwait(false).GetAwaiter().GetResult();

			ThreadIds.Add(dispatcher, result);

			return result;
		}
	}

	internal class FallbackDispatcher : IDispatcher
	{
		public bool IsInvokeRequired => Device.IsInvokeRequired;

		public void BeginInvokeOnMainThread(Action action)
		{
			Device.BeginInvokeOnMainThread(action);
		}
	}
}
