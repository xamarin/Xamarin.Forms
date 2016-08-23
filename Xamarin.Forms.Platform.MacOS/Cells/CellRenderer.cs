using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CellRenderer : IRegisterable
	{
		static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(CellTableViewCell), typeof(Cell), null);

		EventHandler _onForceUpdateSizeRequested;

		public virtual NSView GetCell(Cell item, NSView reusableView, NSTableView tv)
		{
			var tvc = reusableView as CellTableViewCell ?? new CellTableViewCell(NSTableViewCellStyle.Default, item.GetType().FullName);

			tvc.Cell = item;

			WireUpForceUpdateSizeRequested(item, tvc, tv);

			tvc.TextLabel.StringValue = item.ToString();

			UpdateBackground(tvc, item);

			return tvc;
		}

		protected void UpdateBackground(NSView tableViewCell, Cell cell)
		{
			//if (cell.GetIsGroupHeader<ItemsView<Cell>, Cell>())
			//{
			//	if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
			//		tableViewCell.BackgroundColor = new UIColor(247f / 255f, 247f / 255f, 247f / 255f, 1);
			//}
			//else
			//{
			//	// Must be set to a solid color or blending issues will occur
			//	var bgColor = UIColor.White;

			//	var element = cell.RealParent as VisualElement;
			//	if (element != null)
			//		bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToNSColor();

			//	tableViewCell.BackgroundColor = bgColor;
			//}
		}

		protected void WireUpForceUpdateSizeRequested(ICellController cell, NSView nativeCell, NSTableView tableView)
		{
			cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

			_onForceUpdateSizeRequested = (sender, e) =>
			{
				//var index = tableView.IndexPathForCell(nativeCell);
				//if (index != null)
				//	tableView.ReloadRows(new[] { index }, UITableViewRowAnimation.None);
			};

			cell.ForceUpdateSizeRequested += _onForceUpdateSizeRequested;
		}

		internal static CellTableViewCell GetRealCell(BindableObject cell)
		{
			return (CellTableViewCell)cell.GetValue(RealCellProperty);
		}

		internal static void SetRealCell(BindableObject cell, CellTableViewCell renderer)
		{
			cell.SetValue(RealCellProperty, renderer);
		}
	}
}

