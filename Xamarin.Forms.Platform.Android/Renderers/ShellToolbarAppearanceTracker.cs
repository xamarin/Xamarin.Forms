using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class ShellToolbarAppearanceTracker : IShellToolbarAppearanceTracker
	{
		bool _disposed;
		IShellContext _shellContext;
		int _titleTextColor = -1;

		public ShellToolbarAppearanceTracker(IShellContext shellContext)
		{
			_shellContext = shellContext;
		}

		public virtual void SetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
		{
			var foreground = appearance.ForegroundColor;
			var backgroundColor = appearance.BackgroundColor;
			var background = appearance.Background;
			var titleColor = appearance.TitleColor;

			SetColors(toolbar, toolbarTracker, foreground, backgroundColor, titleColor);
			SetBrushes(toolbar, background);
		}

		public virtual void ResetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker)
		{
			SetColors(toolbar, toolbarTracker, ShellRenderer.DefaultForegroundColor, ShellRenderer.DefaultBackgroundColor, ShellRenderer.DefaultTitleColor);
			SetBrushes(toolbar, ShellRenderer.DefaultBackground);
		}

		protected virtual void SetColors(Toolbar toolbar, IShellToolbarTracker toolbarTracker, Color foreground, Color background, Color title)
		{
			var titleArgb = title.ToAndroid(ShellRenderer.DefaultTitleColor).ToArgb();

			if (_titleTextColor != titleArgb)
			{
				toolbar.SetTitleTextColor(titleArgb);
				_titleTextColor = titleArgb;
			}

			var newColor = background.ToAndroid(ShellRenderer.DefaultBackgroundColor);
			if (!(toolbar.Background is ColorDrawable cd) || cd.Color != newColor)
			{
				using (var colorDrawable = new ColorDrawable(background.ToAndroid(ShellRenderer.DefaultBackgroundColor)))
					toolbar.SetBackground(colorDrawable);
			}

			var newTintColor = foreground.IsDefault ? ShellRenderer.DefaultForegroundColor : foreground;

			if (toolbarTracker.TintColor != newTintColor)
				toolbarTracker.TintColor = newTintColor;
		}

		protected virtual void SetBrushes(Toolbar toolbar, Brush background)
		{
			bool isDefaultBackground = Brush.IsNullOrEmpty(background);
			var newBackground = isDefaultBackground ? ShellRenderer.DefaultBackground : background;

			if (!isDefaultBackground || (isDefaultBackground && (!(toolbar.Background is ColorDrawable))))
				toolbar.UpdateBackground(newBackground);
		}

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_shellContext = null;
			}
		}

		#endregion IDisposable
	}
}