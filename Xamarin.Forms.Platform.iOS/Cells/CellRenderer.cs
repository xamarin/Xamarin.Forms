using System;
using Xamarin.Forms.Internals;
#if __UNIFIED__
using UIKit;
using Foundation;

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public class CellRenderer : IRegisterable
	{
		static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(UITableViewCell), typeof(Cell), null);

		EventHandler _onForceUpdateSizeRequested;

		public virtual UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var tvc = reusableCell as CellTableViewCell ?? new CellTableViewCell(UITableViewCellStyle.Default, item.GetType().FullName);

			tvc.Cell = item;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			tvc.TextLabel.Text = item.ToString();

			UpdateBackground(tvc, item);
			return tvc;
		}

		protected void UpdateBackground(UITableViewCell tableViewCell, Cell cell)
		{
			if (cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
					tableViewCell.BackgroundColor = new UIColor(247f / 255f, 247f / 255f, 247f / 255f, 1);
			}
			else
			{
				// Must be set to a solid color or blending issues will occur
				var bgColor = UIColor.White;

				var element = cell.RealParent as VisualElement;
				if (element != null)
					bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToUIColor();

				tableViewCell.BackgroundColor = bgColor;
			}
		}

		protected void WireUpForceUpdateSizeRequested(ICellController cell, UITableViewCell nativeCell, UITableView tableView)
		{
			cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

			_onForceUpdateSizeRequested = (sender, e) => 
			{
				var index = tableView.IndexPathForCell(nativeCell);
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