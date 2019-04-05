using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellFlyoutTemplatedContentRenderer : Windows.UI.Xaml.Controls.Grid
	{
		private ShellRenderer _shellContext;

		public ShellFlyoutTemplatedContentRenderer(ShellRenderer shellContext)
		{
			_shellContext = shellContext;
			_shellContext.Pane = this;
			LoadView(shellContext);
		}

		protected virtual void LoadView(ShellRenderer shellContext)
		{
			// TODO: Render something a lot prettier
			RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
			RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			//var text = new Windows.UI.Xaml.Controls.ContentControl()
			//{
			//	Content = _shellContext.Element.FlyoutHeader,
			//	MinHeight = 56
			//};
			//Children.Add(text);
			var ic = new Windows.UI.Xaml.Controls.ListView()
			{
				ItemsSource = _shellContext.Element.Items,
				Padding = new Windows.UI.Xaml.Thickness(0, 20, 0, 0),
				VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
			};
			ic.SelectionChanged += ShellItem_SelectionChanged;
			SetRow(ic, 1);
			Children.Add(ic);

			UpdateFlyoutHeaderBehavior();
			_shellContext.Element.PropertyChanged += OnShellPropertyChanged;
			UpdateFlyoutBackgroundColor();
		}

		private void ShellItem_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			var element = e.AddedItems?.OfType<Element>().FirstOrDefault();
			if (element != null)
				((IShellController)_shellContext.Element).OnFlyoutItemSelected(element);
		}

		protected virtual void OnShellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Shell.FlyoutHeaderBehaviorProperty.PropertyName)
				UpdateFlyoutHeaderBehavior();
			else if (e.PropertyName == Shell.FlyoutBackgroundColorProperty.PropertyName)
				UpdateFlyoutBackgroundColor();
		}

		protected virtual void UpdateFlyoutBackgroundColor()
		{
			var color = _shellContext.Element.FlyoutBackgroundColor;

			if (color.IsDefault)
				_shellContext.ClearValue(Windows.UI.Xaml.Controls.SplitView.PaneBackgroundProperty);
			else
				_shellContext.PaneBackground = new Windows.UI.Xaml.Media.SolidColorBrush(color.ToWindowsColor());
		}

		protected virtual void UpdateFlyoutHeaderBehavior()
		{
			switch (_shellContext.Element.FlyoutHeaderBehavior)
			{
				case FlyoutHeaderBehavior.Default:
				case FlyoutHeaderBehavior.Fixed:
					//Disable scroll
					break;
				case FlyoutHeaderBehavior.Scroll:
					//Enable scroll
					break;
				case FlyoutHeaderBehavior.CollapseOnScroll:
					// ??
					break;
			}
		}
	}
}
