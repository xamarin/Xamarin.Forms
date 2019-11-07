using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SafeShellNavBarAppearanceTracker : IShellNavBarAppearanceTracker
	{
		UIColor _defaultBarTint;
		UIColor _defaultTint;
		UIStringAttributes _defaultTitleAttributes;

		public void UpdateLayout(UINavigationController controller)
		{
		}

		public void ResetAppearance(UINavigationController controller)
		{
			if (_defaultTint != null)
			{
				var navBar = controller.NavigationBar;
				navBar.TintColor = _defaultTint;
				navBar.TitleTextAttributes = _defaultTitleAttributes;
			}
		}

		public void SetAppearance(UINavigationController controller, ShellAppearance appearance)
		{
			var background = appearance.BackgroundColor;
			var foreground = appearance.ForegroundColor;
			var titleColor = appearance.TitleColor;

			var navBar = controller.NavigationBar;

			if (_defaultTint == null)
			{
				_defaultBarTint = navBar.BarTintColor;
				_defaultTint = navBar.TintColor;
				_defaultTitleAttributes = navBar.TitleTextAttributes;
			}

			if (!background.IsDefault)
				navBar.BarTintColor = background.ToUIColor();
			if (!foreground.IsDefault)
				navBar.TintColor = foreground.ToUIColor();
			if (!titleColor.IsDefault)
			{
				navBar.TitleTextAttributes = new UIStringAttributes
				{
					ForegroundColor = titleColor.ToUIColor()
				};
			}
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing)
		{
		}

		public void Dispose()
		{
			Dispose(true);
		}

		nfloat shadowRadius = float.MinValue;
		float shadowOpacity = float.MinValue;
		CGColor shadowColor;

		public virtual void SetNavigationBarHasShadow(UINavigationController controller, bool hasShadow)
		{
			var navigationBar = controller.NavigationBar;

			if (shadowRadius == float.MinValue)
			{
				shadowRadius = navigationBar.Layer.ShadowRadius;
				shadowOpacity = navigationBar.Layer.ShadowOpacity;
				shadowColor = navigationBar.Layer.ShadowColor;
			}

			if(hasShadow)
			{
				navigationBar.Layer.ShadowColor = UIColor.Black.CGColor;
				//navigationBar.Layer.ShadowOffset = new CGSize(0, 2);
				//navigationBar.Layer.ShadowRadius = 4.0f;
				//navigationBar.Layer.ShadowOpacity = 1.0f;
				//navigationBar.Layer.MasksToBounds = false;

				navigationBar.Layer.ShadowRadius = 3f;
				navigationBar.Layer.ShadowOpacity = 1.0f;
			}
			else
			{
				navigationBar.Layer.ShadowColor = shadowColor;
				//navigationBar.Layer.ShadowOffset = new CGSize(0, 0);
				navigationBar.Layer.ShadowRadius = shadowRadius;
				navigationBar.Layer.ShadowOpacity = shadowOpacity;
				navigationBar.Layer.MasksToBounds = false;

			}
		}
		#endregion
	}
}