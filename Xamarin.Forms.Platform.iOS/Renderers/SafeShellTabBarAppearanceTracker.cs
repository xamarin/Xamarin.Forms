using System.ComponentModel;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SafeShellTabBarAppearanceTracker : IShellTabBarAppearanceTracker
	{
		UIColor _defaultBarTint;
		UIColor _defaultTint;
		UIColor _defaultUnselectedTint;

		public virtual void ResetAppearance(UITabBarController controller)
		{
			if (_defaultTint == null)
				return;

			var tabBar = controller.TabBar;
			tabBar.BarTintColor = _defaultBarTint;
			tabBar.TintColor = _defaultTint;
			tabBar.UnselectedItemTintColor = _defaultUnselectedTint;
		}

		public virtual void SetAppearance(UITabBarController controller, ShellAppearance appearance)
		{
			var tabBar = controller.TabBar;

			bool operatingSystemSupportsUnselectedTint = Forms.IsiOS10OrNewer;

			if (_defaultTint == null)
			{
				_defaultBarTint = tabBar.BarTintColor;
				_defaultTint = tabBar.TintColor;

				if (operatingSystemSupportsUnselectedTint)
				{
					_defaultUnselectedTint = tabBar.UnselectedItemTintColor;
				}
			}

			if (Forms.IsiOS15OrNewer)
				UpdateiOS15TabBarAppearance(controller, appearance);
			else
				UpdateTabBarAppearance(controller, appearance);
		}

		public virtual void UpdateLayout(UITabBarController controller)
		{
		}

		void UpdateiOS15TabBarAppearance(UITabBarController controller, ShellAppearance appearance)
		{
			IShellAppearanceElement appearanceElement = appearance;

			var tabBar = controller.TabBar;

			var tabBarAppearance = new UITabBarAppearance();
			tabBarAppearance.ConfigureWithOpaqueBackground();

			// Set TabBarBackgroundColor
			var tabBarBackgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;

			if (!tabBarBackgroundColor.IsDefault)
				tabBarAppearance.BackgroundColor = tabBarBackgroundColor.ToUIColor();

			// Set TabBarTitleColor
			var tabBarTitleColor = appearanceElement.EffectiveTabBarTitleColor;

			if (!tabBarTitleColor.IsDefault)
			{
				tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = tabBarTitleColor.ToUIColor() };
				tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = tabBarAppearance.StackedLayoutAppearance.Selected.IconColor = tabBarTitleColor.ToUIColor();
			}
			
			//Set TabBarUnselectedColor
			var tabBarUnselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;

			if (!tabBarUnselectedColor.IsDefault)
			{
				tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = tabBarUnselectedColor.ToUIColor() };
				tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = tabBarUnselectedColor.ToUIColor();
			}

			// Set TabBarDisabledColor
			var tabBarDisabledColor = appearanceElement.EffectiveTabBarDisabledColor;

			if (!tabBarDisabledColor.IsDefault)
			{
				tabBarAppearance.StackedLayoutAppearance.Disabled.TitleTextAttributes = new UIStringAttributes { ForegroundColor = tabBarDisabledColor.ToUIColor() };
				tabBarAppearance.StackedLayoutAppearance.Disabled.IconColor = tabBarDisabledColor.ToUIColor();
			}
			
			tabBar.StandardAppearance = tabBar.ScrollEdgeAppearance = tabBarAppearance;
		}

		void UpdateTabBarAppearance(UITabBarController controller, ShellAppearance appearance)
		{
			IShellAppearanceElement appearanceElement = appearance;
			var backgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;
			var unselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;
			var titleColor = appearanceElement.EffectiveTabBarTitleColor;

			var tabBar = controller.TabBar;

			if (!backgroundColor.IsDefault)
				tabBar.BarTintColor = backgroundColor.ToUIColor();
			if (!titleColor.IsDefault)
				tabBar.TintColor = titleColor.ToUIColor();

			bool operatingSystemSupportsUnselectedTint = Forms.IsiOS10OrNewer;

			if (operatingSystemSupportsUnselectedTint)
			{
				if (!unselectedColor.IsDefault)
					tabBar.UnselectedItemTintColor = unselectedColor.ToUIColor();
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

		#endregion
	}
}