using CoreAnimation;
using CoreGraphics;
using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellTableViewController : UITableViewController
	{
		readonly IShellContext _context;
		readonly UIView _headerView;
		readonly ShellTableViewSource _source;
		double _headerMax = -1;
		double _headerMin = 56;
		double _headerOffset = 0;
		double _headerSize;

		public ShellTableViewController(IShellContext context, UIView headerView, Action<Element> onElementSelected)
		{
			_context = context;
			_headerView = headerView;
			_source = new ShellTableViewSource(context, onElementSelected);
			_source.ScrolledEvent += OnScrolled;

			((IShellController)_context.Shell).StructureChanged += OnStructureChanged;
		}

		void OnStructureChanged(object sender, EventArgs e)
		{
			_source.ClearCache();
			TableView.ReloadData();
		}

		public void LayoutParallax()
		{
			var parent = TableView.Superview;
			TableView.Frame = parent.Bounds.Inset(0, SafeAreaOffset);
			SetHeaderSize();
			if (_headerView != null)
			{
				_headerView.Frame = new CGRect(0, _headerOffset + SafeAreaOffset, parent.Frame.Width, _headerSize);

				var headerHeight = _headerSize + _headerOffset;
				if (headerHeight < 0)
					headerHeight = 0;

				if (_headerOffset < 0)
				{
					CAShapeLayer shapeLayer = new CAShapeLayer();
					CGRect rect = new CGRect(0, _headerOffset * -1, parent.Frame.Width, headerHeight);
					var path = CGPath.FromRect(rect);
					shapeLayer.Path = path;
					_headerView.Layer.Mask = shapeLayer;
				}
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			SetHeaderSize();

			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			if (Forms.IsiOS11OrNewer)
				TableView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
			TableView.ContentInset = new UIEdgeInsets((nfloat)_headerMax + SafeAreaOffset, 0, 0, 0);
			TableView.Source = _source;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if ((_context?.Shell as IShellController) != null)
					((IShellController)_context.Shell).StructureChanged -= OnStructureChanged;
			}

			base.Dispose(disposing);
		}

		bool _headerSizeSet = false;
		void SetHeaderSize()
		{
			if(_headerView == null)
			{
				_headerMax = 20;
				_headerMin = 0;
				return;
			}
			if (_headerSizeSet)
				return;

			_headerView.LayoutIfNeeded();
			_headerMax = _headerView.Bounds.Height;
			_headerSize = _headerMax;
			_headerSizeSet = true;
		}

		void OnScrolled(object sender, UIScrollView e)
		{
			SetHeaderSize();
			var headerBehavior = _context.Shell.FlyoutHeaderBehavior;

			switch (headerBehavior)
			{
				case FlyoutHeaderBehavior.Default:
				case FlyoutHeaderBehavior.Fixed:
					_headerSize = _headerMax;
					break;

				case FlyoutHeaderBehavior.Scroll:
					_headerSize = _headerMax;
					_headerOffset = Math.Min(0, -(_headerMax + e.ContentOffset.Y));
					break;

				case FlyoutHeaderBehavior.CollapseOnScroll:
					_headerSize = Math.Max(_headerMin, Math.Min(_headerMax, _headerMax - e.ContentOffset.Y - _headerMax));
					break;
			}

			LayoutParallax();
		}

		float SafeAreaOffset => (float)Platform.SafeAreaInsetsForWindow.Top;
	}
}