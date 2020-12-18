using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	class ShellFlyoutContentManager
	{
		double _headerMin = 56;
		double _headerOffset = 0;
		UIView _contentView;
		UIScrollView ScrollView => _contentView as UIScrollView;
		UIContainerView _headerView;
		UIView _footerView;
		double _headerSize;
		readonly IShellContext _context;

		IShellController ShellController => _context.Shell;
		public ShellFlyoutContentManager(IShellContext context)
		{
			_context = context;
			_context.Shell.PropertyChanged += OnShellPropertyChanged;
			ShellController.StructureChanged += OnStructureChanged;
		}

		public View Content
		{
			get;
			set;
		}

		public UIView ContentView
		{
			get
			{
				return _contentView;
			}
			set
			{
				if (ScrollView != null && !(ScrollView is UITableView))
					ScrollView.Scrolled -= ScrollViewScrolled;

				_contentView = value;

				if (ScrollView != null && !(ScrollView is UITableView))
					ScrollView.Scrolled += ScrollViewScrolled;

				if (ScrollView != null && Forms.IsiOS11OrNewer)
					ScrollView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
			}
		}

		void ScrollViewScrolled(object sender, EventArgs e) =>
			OnScrolled();

		public virtual UIContainerView HeaderView
		{
			get => _headerView;
			set
			{
				if (_headerView == value)
					return;

				if (_headerView != null)
					_headerView.HeaderSizeChanged -= OnHeaderFooterSizeChanged;

				_headerView = value;

				if (_headerView != null)
					_headerView.HeaderSizeChanged += OnHeaderFooterSizeChanged;
			}
		}

		public virtual UIView FooterView
		{
			get => _footerView;
			set
			{
				if (_footerView == value)
					return;

				_footerView = value;
			}
		}

		void OnHeaderFooterSizeChanged(object sender, EventArgs e)
		{
			_headerSize = HeaderMax;
			SetHeaderContentInset();
			LayoutParallax();
		}

		internal void SetHeaderContentInset()
		{
			if (ScrollView == null)
				return;

			var offset = ScrollView.ContentInset.Top;

			if (HeaderView != null)
				ScrollView.ContentInset = new UIEdgeInsets((nfloat)HeaderMax, 0, 0, 0);
			else
				ScrollView.ContentInset = new UIEdgeInsets(Platform.SafeAreaInsetsForWindow.Top, 0, 0, 0);
			
			offset -= ScrollView.ContentInset.Top;

			ScrollView.ContentOffset =
				new CGPoint(ScrollView.ContentOffset.X, ScrollView.ContentOffset.Y + offset);

			UpdateVerticalScrollMode();
		}


		public void UpdateVerticalScrollMode()
		{
			switch (_context.Shell.FlyoutVerticalScrollMode)
			{
				case ScrollMode.Auto:
					ScrollView.ScrollEnabled = true;
					ScrollView.AlwaysBounceVertical = false;
					break;
				case ScrollMode.Enabled:
					ScrollView.ScrollEnabled = true;
					ScrollView.AlwaysBounceVertical = true;
					break;
				case ScrollMode.Disabled:
					ScrollView.ScrollEnabled = false;
					ScrollView.AlwaysBounceVertical = false;
					break;
			}
		}

		public void LayoutParallax()
		{
			var parent = ContentView?.Superview;
			if (parent == null)
				return;

			nfloat footerHeight = 0;

			if (FooterView != null)
				footerHeight = FooterView.Frame.Height;

			var contentViewYOffset = HeaderView?.Frame.Height ?? 0;
			if (ScrollView != null)
			{
				if (Content == null)
				{
					ContentView.Frame =
							new CGRect(parent.Bounds.X, HeaderTopMargin, parent.Bounds.Width, parent.Bounds.Height - HeaderTopMargin - footerHeight);
				}
				else
				{
					ContentView.Frame =
							new CGRect(parent.Bounds.X, HeaderTopMargin, parent.Bounds.Width, parent.Bounds.Height - HeaderTopMargin - footerHeight);

					if (Content != null)
						Layout.LayoutChildIntoBoundingRegion(Content, new Rectangle(0, 0, ContentView.Frame.Width, ContentView.Frame.Height - contentViewYOffset));
				}
			}
			else
			{
				ContentView.Frame =
						new CGRect(parent.Bounds.X, HeaderTopMargin + contentViewYOffset, parent.Bounds.Width, parent.Bounds.Height - HeaderTopMargin - footerHeight - contentViewYOffset);

				if (Content != null)
					Layout.LayoutChildIntoBoundingRegion(Content, new Rectangle(0, 0, ContentView.Frame.Width, ContentView.Frame.Height));
			}

			if (HeaderView != null && !double.IsNaN(_headerSize))
			{
				var margin = HeaderView.Margin;
				var leftMargin = margin.Left - margin.Right;

				HeaderView.Frame = new CGRect(leftMargin, _headerOffset, parent.Frame.Width, _headerSize + HeaderTopMargin);

				if (_context.Shell.FlyoutHeaderBehavior == FlyoutHeaderBehavior.Scroll && HeaderTopMargin > 0 && _headerOffset < 0)
				{
					var headerHeight = Math.Max(_headerMin, _headerSize + _headerOffset);
					CAShapeLayer shapeLayer = new CAShapeLayer();
					CGRect rect = new CGRect(0, _headerOffset * -1, parent.Frame.Width, headerHeight);
					var path = CGPath.FromRect(rect);
					shapeLayer.Path = path;
					HeaderView.Layer.Mask = shapeLayer;
				}
				else if (HeaderView.Layer.Mask != null)
					HeaderView.Layer.Mask = null;
			}
		}

		void OnStructureChanged(object sender, EventArgs e) => UpdateVerticalScrollMode();

		void OnShellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Is(Shell.FlyoutHeaderBehaviorProperty))
			{
				SetHeaderContentInset();
				LayoutParallax();
			}
			else if (e.Is(Shell.FlyoutVerticalScrollModeProperty))
				UpdateVerticalScrollMode();
		}

		public void ViewDidLoad()
		{
			HeaderView?.MeasureIfNeeded();
			SetHeaderContentInset();
		}

		public void OnScrolled()
		{
			if (HeaderView == null)
				return;

			var headerBehavior = _context.Shell.FlyoutHeaderBehavior;

			switch (headerBehavior)
			{
				case FlyoutHeaderBehavior.Default:
				case FlyoutHeaderBehavior.Fixed:
					_headerSize = HeaderMax;
					_headerOffset = 0;
					break;

				case FlyoutHeaderBehavior.Scroll:
					_headerSize = HeaderMax;
					_headerOffset = Math.Min(0, -(HeaderMax + ScrollView.ContentOffset.Y));
					break;

				case FlyoutHeaderBehavior.CollapseOnScroll:
					_headerSize = Math.Max(_headerMin, -ScrollView.ContentOffset.Y);
					_headerOffset = 0;
					break;
			}

			LayoutParallax();
		}

		double HeaderMax => HeaderView?.MeasuredHeight ?? 0;
		double HeaderTopMargin => (HeaderView != null) ? HeaderView.Margin.Top - HeaderView.Margin.Bottom : 0;

		public void TearDown()
		{
			if (HeaderView != null)
				HeaderView.HeaderSizeChanged -= OnHeaderFooterSizeChanged;

			_context.Shell.PropertyChanged -= OnShellPropertyChanged;
			ShellController.StructureChanged -= OnStructureChanged;
			ContentView = null;
		}
	}
}