using System;

namespace Xamarin.Forms
{
	/// <summary>
	/// Specifies a View to be used as popup content for popups which return a result
	/// </summary>
	/// <typeparam name="T">The type of result returned by the popup</typeparam>
	public interface IPopupView<out T> : IPopup
	{
		void SetDismissDelegate(Action<T> dismissDelegate);
	}

	public interface IPopup
	{
		View View { get; }
	}
}