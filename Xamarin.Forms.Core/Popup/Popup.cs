using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public abstract class Popup<T>
	{
		protected internal Popup(View view, string title = null, bool isLightDismissEnabled = true, View anchor = null,
			Size size = default(Size))
		{
			View = view;
			Title = title;
			IsLightDismissEnabled = isLightDismissEnabled;
			Anchor = anchor;
			Size = size;

			_tcs = new TaskCompletionSource<T>();
		}

		protected Popup(IPopupView<T> popupView, string title = null, bool isLightDismissEnabled = true, View anchor = null,
			Size size = default(Size))
			: this(popupView.View, title, isLightDismissEnabled, anchor, size)
		{
			popupView.SetDismissDelegate(Dismiss);
		}

		public string Title { get; private set; }

		public bool IsLightDismissEnabled { get; private set; }

		public View View { get; protected set; }

		public View Anchor { get; private set; }

		public Size Size { get; private set; }

		public void Reset()
		{
			_tcs = new TaskCompletionSource<T>();
		}

		public void Dismiss(T result)
		{
			_tcs.TrySetResult(result);
		}

		public Task<T> Result => _tcs.Task;

		public void LightDismiss()
		{
			_tcs.TrySetResult(OnLightDismissed());
		}

		protected abstract T OnLightDismissed();

		TaskCompletionSource<T> _tcs;
	}

	public class Popup : Popup<PopupDismissed>
	{
		public Popup(View view, string title = null, View anchor = null)
			: base(view, title, true, anchor)
		{
		}

		protected override PopupDismissed OnLightDismissed()
		{
			return Dismissed;
		}

		internal static PopupDismissed Dismissed = new PopupDismissed();
	}

	public struct PopupDismissed
	{
	}
}