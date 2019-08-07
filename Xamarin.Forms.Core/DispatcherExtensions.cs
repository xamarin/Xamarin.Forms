using System;

namespace Xamarin.Forms
{
	internal static class DispatcherExtensions
	{
		static IDispatcherProvider s_current;

		public static IDispatcher GetDispatcher(this BindableObject bindableObject)
		{
			s_current = s_current ?? DependencyService.Get<IDispatcherProvider>();



			return s_current.GetDispatcher(bindableObject);
		}

		public static void Dispatch(this IDispatcher dispatcher, Action action)
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
	}
}
