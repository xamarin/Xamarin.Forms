using CoreAnimation;
using CoreGraphics;
using System;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellTableViewController : UITableViewController
	{
		readonly IShellContext _context;
		readonly UIContainerView _headerView;
		readonly ShellTableViewSource _source;
		double _headerMin = 56;
		double _headerOffset = 0;
		double _headerSize;

		public ShellTableViewController(IShellContext context, UIContainerView headerView, Action<Element> onElementSelected)
		{
			_context = context;
			_headerView = headerView;
			_source = new ShellTableViewSource(context, onElementSelected);
			_source.ScrolledEvent += OnScrolled;
			if (_headerView != null)
				_headerView.HeaderSizeChanged += OnHeaderSizeChanged;
			((IShellController)_context.Shell).StructureChanged += OnStructureChanged;
		}

		void OnHeaderSizeChanged(object sender, EventArgs e)
		{
			_headerSize = HeaderMax;
			TableView.ContentInset = new UIEdgeInsets((nfloat)HeaderMax, 0, 0, 0);
			LayoutParallax();
		}

		void OnStructureChanged(object sender, EventArgs e)
		{
			_source.ClearCache();
			TableView.ReloadData();
		}

		public void LayoutParallax()
		{
			if (TableView?.Superview == null)
				return;

			var parent = TableView.Superview;

			if (_headerView != null && IsFlyoutFixed)
				TableView.Frame =
					new CGRect(0, HeaderTopMargin, parent.Bounds.Width, parent.Bounds.Height);
			else
				TableView.Frame = parent.Bounds;

			if (_headerView != null)
			{
				var margin = _headerView.Margin;
				var leftMargin = margin.Left - margin.Right;

				_headerView.Frame = new CGRect(leftMargin, _headerOffset + HeaderTopMargin, parent.Frame.Width, _headerSize);
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			_headerView?.MeasureIfNeeded();

			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			if (Forms.IsiOS11OrNewer)
				TableView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;

			if (_headerView != null)
			{
				if(IsFlyoutFixed)
					TableView.ContentInset = new UIEdgeInsets((nfloat)HeaderMax, 0, 0, 0);
				else
					TableView.ContentInset = new UIEdgeInsets((nfloat)HeaderMax + (nfloat)HeaderTopMargin, 0, 0, 0);
			}
			else
				TableView.ContentInset = new UIEdgeInsets(Platform.SafeAreaInsetsForWindow.Top, 0, 0, 0);

			TableView.Source = _source;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if ((_context?.Shell as IShellController) != null)
					((IShellController)_context.Shell).StructureChanged -= OnStructureChanged;

				if (_source != null)
					_source.ScrolledEvent -= OnScrolled;

				if (_headerView != null)
					_headerView.HeaderSizeChanged -= OnHeaderSizeChanged;
			}

			base.Dispose(disposing);
		}


		void OnScrolled(object sender, UIScrollView e)
		{
			if (_headerView == null)
				return;

			var headerBehavior = _context.Shell.FlyoutHeaderBehavior;

			switch (headerBehavior)
			{
				case FlyoutHeaderBehavior.Default:
				case FlyoutHeaderBehavior.Fixed:
					_headerSize = HeaderMax;
					break;

				case FlyoutHeaderBehavior.Scroll:
					_headerSize = HeaderMax;
					_headerOffset = Math.Min(0, -(HeaderMax + e.ContentOffset.Y + HeaderTopMargin));
					break;

				case FlyoutHeaderBehavior.CollapseOnScroll:
					_headerSize = Math.Max(_headerMin, Math.Min(HeaderMax, HeaderMax - (e.ContentOffset.Y + HeaderTopMargin) - HeaderMax));
					break;
			}

			LayoutParallax();
		}

		double HeaderMax => _headerView?.MeasuredHeight ?? 0;
		double HeaderTopMargin => (_headerView != null) ? _headerView.Margin.Top - _headerView.Margin.Bottom : 0;
		bool IsFlyoutFixed => _context.Shell.FlyoutHeaderBehavior == FlyoutHeaderBehavior.Default || _context.Shell.FlyoutHeaderBehavior == FlyoutHeaderBehavior.Fixed;
	}
}