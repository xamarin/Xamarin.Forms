using System;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public class CellRenderer : IRegisterable
	{
		static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(UITableViewCell), typeof(Cell), null);

		EventHandler _onForceUpdateSizeRequested;

		public virtual UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			Performance.Start(out string reference);

			var tvc = reusableCell as CellTableViewCell ?? new CellTableViewCell(UITableViewCellStyle.Default, item.GetType().FullName);

			tvc.Cell = item;

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
			tableViewCell.BackgroundColor = color;
		}

		protected void UpdateBackground(UITableViewCell tableViewCell, Cell cell)
		{
			if (cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
					SetBackgroundColor(tableViewCell, cell, new UIColor(247f / 255f, 247f / 255f, 247f / 255f, 1));
			}
			else
			{
				// Must be set to a solid color or blending issues will occur
				var bgColor = UIColor.White;

				var element = cell.RealParent as VisualElement;
				if (element != null)
					bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToUIColor();

				SetBackgroundColor(tableViewCell, cell, bgColor);
			}
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