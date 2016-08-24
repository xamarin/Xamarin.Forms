using System;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CellRenderer : IRegisterable
	{
		static readonly BindableProperty RealCellProperty = BindableProperty.CreateAttached("RealCell", typeof(NSView), typeof(Cell), null);

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
			var bgColor = NSColor.White;
			var element = cell.RealParent as VisualElement;
			if (element != null)
				bgColor = element.BackgroundColor == Color.Default ? bgColor : element.BackgroundColor.ToNSColor();

			UpdateBackgroundChild(cell, bgColor);

			var formsNSView = tableViewCell as FormsNSView;
			if (formsNSView == null)
				return;

			formsNSView.BackgroundColor = bgColor;
		}

		internal virtual void UpdateBackgroundChild(Cell cell, NSColor backgroundColor)
		{

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

		internal static NSView GetRealCell(BindableObject cell)
		{
			return (NSView)cell.GetValue(RealCellProperty);
		}

		internal static void SetRealCell(BindableObject cell, NSView renderer)
		{
			cell.SetValue(RealCellProperty, renderer);
		}
	}
}

