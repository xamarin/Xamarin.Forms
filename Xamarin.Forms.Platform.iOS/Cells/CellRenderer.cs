using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
{
	public class CellRenderer : IRegisterable
	{
		static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(UITableViewCell), typeof(Cell), null);

		EventHandler _onForceUpdateSizeRequested;

		public virtual UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			Performance.Start(out string reference);

			var tvc = reusableCell as CellTableViewCell;

			if (tvc == null)
				tvc = new CellTableViewCell(UITableViewCellStyle.Default, item.GetType().FullName);
			else
				tvc.PropertyChanged -= HandlePropertyChanged;

			tvc.Cell = item;
			tvc.PropertyChanged += HandlePropertyChanged;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			tvc.TextLabel.Text = item.ToString();

			UpdateBackground(tvc, item);

			SetAccessibility (tvc, item);

			Performance.Stop(reference);
			return tvc;
		}

		public virtual void SetAccessibility (UITableViewCell tableViewCell, Cell cell)
		{
			if (cell.IsSet (AutomationProperties.IsInAccessibleTreeProperty))
				tableViewCell.IsAccessibilityElement = cell.GetValue (AutomationProperties.IsInAccessibleTreeProperty).Equals (true);
			else
				tableViewCell.IsAccessibilityElement = false;

			if (cell.IsSet (AutomationProperties.NameProperty))
				tableViewCell.AccessibilityLabel = cell.GetValue (AutomationProperties.NameProperty).ToString ();
			else
				tableViewCell.AccessibilityLabel = null;

			if (cell.IsSet (AutomationProperties.HelpTextProperty))
				tableViewCell.AccessibilityHint = cell.GetValue (AutomationProperties.HelpTextProperty).ToString ();
			else
				tableViewCell.AccessibilityHint = null;
		}

		public virtual void SetBackgroundColor(UITableViewCell tableViewCell, Cell cell, UIColor color)
		{
			tableViewCell.TextLabel.BackgroundColor = color;
			tableViewCell.ContentView.BackgroundColor = color;
			tableViewCell.BackgroundColor = color;
		}

		protected virtual void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		protected void UpdateBackground(UITableViewCell tableViewCell, Cell cell)
		{
			var uiBgColor = UITableView.Appearance.BackgroundColor ?? UIColor.White; // Must be set to a solid color or blending issues will occur
#if __MOBILE__
			var defaultBgColor = cell.On<PlatformConfiguration.iOS>().DefaultBackgroundColor();
#else
			var defaultBgColor = cell.On<PlatformConfiguration.macOS>().DefaultBackgroundColor();
#endif
			if (defaultBgColor != Color.Default)
			{
				uiBgColor = defaultBgColor.ToUIColor();
			}
			else if (cell.IsSet(Cell.BackgroundColorProperty))
			{
				uiBgColor = cell.BackgroundColor.ToUIColor();
			}
			else
			{
				if (cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
				{
					if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
						return;

					uiBgColor = new UIColor(247f / 255f, 247f / 255f, 247f / 255f, 1);
				}
				else
				{
					if (cell.RealParent is VisualElement element && element.BackgroundColor != Color.Default)
						uiBgColor = element.BackgroundColor.ToUIColor();
				}
			}

			SetBackgroundColor(tableViewCell, cell, uiBgColor);
		}

		protected void WireUpForceUpdateSizeRequested(ICellController cell, UITableViewCell nativeCell, UITableView tableView)
		{
			cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

			_onForceUpdateSizeRequested = (sender, e) =>
			{
				var index = tableView?.IndexPathForCell(nativeCell) ?? (sender as Cell)?.GetIndexPath();
				if (index != null)
					tableView.ReloadRows(new[] { index }, UITableViewRowAnimation.None);
			};

			cell.ForceUpdateSizeRequested += _onForceUpdateSizeRequested;
		}

		internal static UITableViewCell GetRealCell(BindableObject cell)
		{
			return (UITableViewCell)cell.GetValue(RealCellProperty);
		}

		internal static void SetRealCell(BindableObject cell, UITableViewCell renderer)
		{
			cell.SetValue(RealCellProperty, renderer);
		}
	}
}
