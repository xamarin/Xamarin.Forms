using CoreGraphics;
using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellTableViewController : UITableViewController
	{
		private readonly IShellContext _context;
		private readonly UIView _headerView;
		private readonly ShellTableViewSource _source;
		private double _headerMax = 200;
		private double _headerMin = 44;
		private double _headerOffset = 0;
		private double _headerSize;

		public ShellTableViewController(IShellContext context, UIView headerView, Action<Element> onElementSelected)
		{
			if (headerView == null)
			{
				_headerMax = 20;
				_headerMin = 0;
			}

			_headerSize = _headerMax;
			_context = context;
			_headerView = headerView;
			_source = new ShellTableViewSource(context, onElementSelected);
			_source.ScrolledEvent += OnScrolled;

			((IShellController)_context.Shell).StructureChanged += OnStructureChanged;
		}

		private void OnStructureChanged(object sender, EventArgs e)
		{
			_source.ClearCache();
			TableView.ReloadData();
		}

		public void LayoutParallax()
		{
			var parent = TableView.Superview;

			TableView.Frame = parent.Bounds;
			if (_headerView != null)
				_headerView.Frame = new CGRect(0, _headerOffset, parent.Frame.Width, _headerSize);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			if (Forms.IsiOS11OrNewer)
				TableView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
			TableView.ContentInset = new UIEdgeInsets((nfloat)_headerMax, 0, 0, 0);
			TableView.Source = _source;
		}

		private void OnScrolled(object sender, UIScrollView e)
		{
			var headerBehavior = _context.Shell.FlyoutHeaderBehavior;

			switch (headerBehavior)
			{
				case FlyoutHeaderBehavior.Default:
				case FlyoutHeaderBehavior.Fixed:
					_headerSize = _headerMax;
					break;

				case FlyoutHeaderBehavior.Scroll:
					_headerSize = _headerMax;
					_headerOffset = Math.Min (0, -(_headerMax + e.ContentOffset.Y));
					break;

				case FlyoutHeaderBehavior.CollapseOnScroll:
					_headerSize = Math.Max(_headerMin, Math.Min(_headerMax, _headerMax - e.ContentOffset.Y - _headerMax));
					break;
			}

			LayoutParallax();
		}
	}
}