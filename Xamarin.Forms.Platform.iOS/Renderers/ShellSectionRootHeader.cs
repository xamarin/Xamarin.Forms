using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Core;

namespace Xamarin.Forms.Platform.iOS
{
	public class ShellSectionRootHeader : UICollectionViewController, IAppearanceObserver, IShellSectionRootHeader
	{
		#region IAppearanceObserver

		Color _defaultBackgroundColor = new Color(0.964);
		Color _defaultForegroundColor = Color.Black;
		Color _defaultUnselectedColor = Color.Black.MultiplyAlpha(0.7);

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			if (appearance == null)
				ResetAppearance();
			else
				SetAppearance(appearance);
		}

		protected virtual void ResetAppearance()
		{
			SetValues(_defaultBackgroundColor, _defaultForegroundColor, _defaultUnselectedColor);
		}

		protected virtual void SetAppearance(ShellAppearance appearance)
		{
			SetValues(appearance.BackgroundColor.IsDefault ? _defaultBackgroundColor : appearance.BackgroundColor,
				appearance.ForegroundColor.IsDefault ? _defaultForegroundColor : appearance.ForegroundColor,
				appearance.UnselectedColor.IsDefault ? _defaultUnselectedColor : appearance.UnselectedColor);
		}

		void SetValues(Color backgroundColor, Color foregroundColor, Color unselectedColor)
		{
			CollectionView.BackgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, .863).ToUIColor();

			bool reloadData = _selectedColor != foregroundColor || _unselectedColor != unselectedColor;

			_selectedColor = foregroundColor;
			_unselectedColor = unselectedColor;

			if (reloadData)
				ReloadData();
		}

		#endregion IAppearanceObserver

		static readonly NSString CellId = new NSString("HeaderCell");

		readonly IShellContext _shellContext;
		UIView _bar;
		UIView _bottomShadow;
		Color _selectedColor;
		Color _unselectedColor;
		bool _isDisposed;
		IDictionary<ShellContent, ShellSectionHeaderCell> _shellContentToCellMapping;

		[Internals.Preserve(Conditional = true)]
		public ShellSectionRootHeader()
		{

		}

		[Internals.Preserve(Conditional = true)]
		public ShellSectionRootHeader(IShellContext shellContext) : base(new UICollectionViewFlowLayout())
		{
			_shellContext = shellContext;
			_shellContentToCellMapping = new Dictionary<ShellContent, ShellSectionHeaderCell>();
		}

		public double SelectedIndex { get; set; }
		public ShellSection ShellSection { get; set; }
		IShellSectionController ShellSectionController => ShellSection;

		public UIViewController ViewController => this;

		public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return false;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var reusedCell = (UICollectionViewCell)collectionView.DequeueReusableCell(CellId, indexPath);
			var headerCell = reusedCell as ShellSectionHeaderCell;

			if (headerCell == null)
				return reusedCell;

			var selectedItems = collectionView.GetIndexPathsForSelectedItems();

			var shellContent = ShellSectionController.GetItems()[indexPath.Row];
			headerCell.Label.Text = shellContent.Title;
			headerCell.Label.SetNeedsDisplay();

			headerCell.SelectedColor = _selectedColor.ToUIColor();
			headerCell.UnSelectedColor = _unselectedColor.ToUIColor();

			if (selectedItems.Length > 0 && selectedItems[0].Row == indexPath.Row)
				headerCell.Selected = true;
			else
				headerCell.Selected = false;

			headerCell.ApplyBadge(Badge.GetBadgeBackground(shellContent), Badge.GetBadgeText(shellContent), Badge.GetBadgeTextColor(shellContent));
			_shellContentToCellMapping[shellContent] = headerCell;

			return headerCell;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return ShellSectionController.GetItems().Count;
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			if(CollectionView.CellForItem(indexPath) is ShellSectionHeaderCell cell)
				cell.Label.TextColor = _unselectedColor.ToUIColor();
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var row = indexPath.Row;

			var item = ShellSectionController.GetItems()[row];

			if (item != ShellSection.CurrentItem)
				ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, item);

			if (CollectionView.CellForItem(indexPath) is ShellSectionHeaderCell cell)
				cell.Label.TextColor = _selectedColor.ToUIColor();
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var row = indexPath.Row;
			var item = ShellSectionController.GetItems()[row];
			IShellController shellController = _shellContext.Shell;

			if (item == ShellSection.CurrentItem)
				return true;
			return shellController.ProposeNavigation(ShellNavigationSource.ShellContentChanged, (ShellItem)ShellSection.Parent, ShellSection, item, ShellSection.Stack, true);
		}

		public override void ViewDidLayoutSubviews()
		{
			if (_isDisposed)
				return;

			base.ViewDidLayoutSubviews();

			LayoutBar();

			_bottomShadow.Frame = new CGRect(0, CollectionView.Frame.Bottom, CollectionView.Frame.Width, 0.5);
		}

		public override void ViewDidLoad()
		{
			if (_isDisposed)
				return;

			base.ViewDidLoad();

			CollectionView.ScrollsToTop = false;
			CollectionView.Bounces = false;
			CollectionView.AlwaysBounceHorizontal = false;
			CollectionView.ShowsHorizontalScrollIndicator = false;
			CollectionView.ClipsToBounds = false;

			_bar = new UIView(new CGRect(0, 0, 20, 20));
			_bar.BackgroundColor = UIColor.White;
			_bar.Layer.ZPosition = 9001; //its over 9000!
			CollectionView.AddSubview(_bar);

			_bottomShadow = new UIView(new CGRect(0, 0, 10, 1));
			_bottomShadow.BackgroundColor = Color.Black.MultiplyAlpha(0.3).ToUIColor();
			_bottomShadow.Layer.ZPosition = 9002;
			CollectionView.AddSubview(_bottomShadow);

			var flowLayout = Layout as UICollectionViewFlowLayout;
			flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			flowLayout.MinimumInteritemSpacing = 0;
			flowLayout.MinimumLineSpacing = 0;
			flowLayout.EstimatedItemSize = new CGSize(70, 35);

			CollectionView.RegisterClassForCell(GetCellType(), CellId);

			// Move to HookEvents?
			((IShellController)_shellContext.Shell).AddAppearanceObserver(this, ShellSection);
			ShellSectionController.ItemsCollectionChanged += OnShellSectionItemsChanged;

			UpdateSelectedIndex();

			HookEvents();
			ShellSection.PropertyChanged += OnShellSectionPropertyChanged;
		}

		protected virtual Type GetCellType()
		{
			return typeof(ShellSectionHeaderCell);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				// Move to UnhookEvents?
				((IShellController)_shellContext.Shell).RemoveAppearanceObserver(this);
				ShellSectionController.ItemsCollectionChanged -= OnShellSectionItemsChanged;
				ShellSection.PropertyChanged -= OnShellSectionPropertyChanged;

				UnhookEvents();

				ShellSection = null;

				_shellContentToCellMapping.Clear();
				_shellContentToCellMapping = null;

				_bar.RemoveFromSuperview();
				this.RemoveFromParentViewController();
				_bar.Dispose();
				_bar = null;
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		protected void LayoutBar()
		{
			if (SelectedIndex < 0)
				return;

			if (ShellSectionController.GetItems().IndexOf(ShellSection.CurrentItem) != SelectedIndex)
				return;

			var layout = CollectionView.GetLayoutAttributesForItem(NSIndexPath.FromItemSection((int)SelectedIndex, 0));

			if (layout == null)
				return;

			var frame = layout.Frame;

			if (_bar.Frame.Height != 2)
			{
				_bar.Frame = new CGRect(frame.X, frame.Bottom - 2, frame.Width, 2);
			}
			else
			{
				UIView.Animate(.25, () => _bar.Frame = new CGRect(frame.X, frame.Bottom - 2, frame.Width, 2));
			}
		}

		protected virtual void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ShellSection.CurrentItemProperty.PropertyName)
			{
				UpdateSelectedIndex();
			}
		}

		protected virtual void UpdateSelectedIndex(bool animated = false)
		{
			if (ShellSection.CurrentItem == null)
				return;

			SelectedIndex = ShellSectionController.GetItems().IndexOf(ShellSection.CurrentItem);

			if (SelectedIndex < 0)
				return;

			LayoutBar();

			CollectionView.SelectItem(NSIndexPath.FromItemSection((int)SelectedIndex, 0), false, UICollectionViewScrollPosition.CenteredHorizontally);
		}

		void OnShellSectionItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ReloadData();

			if (e.OldItems != null)
			{
				foreach (ShellContent shellContent in e.OldItems)
				{
					UnhookChildEvents(shellContent);
				}
			}

			if (e.NewItems != null)
			{
				foreach (ShellContent shellContent in e.NewItems)
				{
					HookChildEvents(shellContent);
				}
			}
		}

		void ReloadData()
		{
			if (_isDisposed)
				return;

			CollectionView.ReloadData();
			CollectionView.CollectionViewLayout.InvalidateLayout();
		}

		void HookEvents()
		{
			foreach (var shellContent in ShellSection.Items)
			{
				HookChildEvents(shellContent);
			}
		}

		protected virtual void HookChildEvents(ShellContent shellContent)
		{
			shellContent.PropertyChanged += OnShellContentPropertyChanged;
		}

		void UnhookEvents()
		{
			foreach (var shellContent in ShellSection.Items)
			{
				UnhookChildEvents(shellContent);
			}
		}

		protected virtual void UnhookChildEvents(ShellContent shellContent)
		{
			shellContent.PropertyChanged -= OnShellContentPropertyChanged;
		}

		void OnShellContentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Badge.BadgeTextProperty.PropertyName ||
				e.PropertyName == Badge.BadgeTextColorProperty.PropertyName ||
				e.PropertyName == Badge.BadgeBackgroundProperty.PropertyName)
			{
				var shellContent = (ShellContent)sender;
				var headerCell = _shellContentToCellMapping[shellContent];
				headerCell.ApplyBadge(Badge.GetBadgeBackground(shellContent), Badge.GetBadgeText(shellContent), Badge.GetBadgeTextColor(shellContent));
				headerCell.SetNeedsLayout();
			}
		}

		public class ShellSectionHeaderCell : UICollectionViewCell
		{
			public UIColor SelectedColor { get; set; }
			public UIColor UnSelectedColor { get; set; }

			[Internals.Preserve(Conditional = true)]
			public ShellSectionHeaderCell()
			{

			}

			[Export("initWithFrame:")]
			[Internals.Preserve(Conditional = true)]
			public ShellSectionHeaderCell(CGRect frame) : base(frame)
			{
				Label = new UILabel();
				Label.TextAlignment = UITextAlignment.Center;
				Label.Font = UIFont.BoldSystemFontOfSize(14);

				ContentView.AddSubview(Label);

				BadgeLabelContainer = new UIView();
				BadgeLabelContainer.ClipsToBounds = true;
				BadgeLabelContainer.Layer.CornerRadius = 9;

				BadgeLabel = new UILabel();
				BadgeLabel.TextAlignment = UITextAlignment.Center;
				BadgeLabel.Font = UIFont.SystemFontOfSize(13);
				BadgeLabel.TextColor = UIColor.White;

				BadgeLabelContainer.AddSubview(BadgeLabel);
				ContentView.AddSubview(BadgeLabelContainer);
			}

			public override bool Selected
			{
				get => base.Selected;
				set
				{
					base.Selected = value;
					Label.TextColor = value ? SelectedColor : UnSelectedColor;
				}
			}

			public UILabel Label { get; }

			protected UILabel BadgeLabel { get; }

			protected UIView BadgeLabelContainer { get; }

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();

				Label.Frame = Bounds;

				nfloat height = BadgeLabel.IntrinsicContentSize.Height + 2;
				nfloat width = BadgeLabel.IntrinsicContentSize.Width + 12;

				BadgeLabel.Frame = new CGRect(0, 0, width > height ? width : height, height);

				var bounds = new CGRect(0, 0, width > height ? width : height, height);
				bounds.Offset(Label.IntrinsicContentSize.Width + ((Bounds.Width - Label.IntrinsicContentSize.Width) / 2) - 6, (Bounds.Height - Label.IntrinsicContentSize.Height) / 2.0f - 8);

				BadgeLabelContainer.Frame = bounds;
				BadgeLabelContainer.UpdateBackgroundLayer();
			}

			public override CGSize SizeThatFits(CGSize size)
			{
				return new CGSize(Label.SizeThatFits(size).Width + 30, 35);
			}

			public void ApplyBadge(Brush color, string text, Color textColor)
			{
				BadgeLabel.UpdateBackground(Brush.IsNullOrEmpty(color) ? new SolidColorBrush(Color.FromRgb(255, 59, 48)) : color);

				BadgeLabel.Text = text;

				if (textColor == Color.Default)
				{
					BadgeLabel.TextColor = UIColor.White;
				}
				else
				{
					BadgeLabel.TextColor = textColor.ToUIColor();
				}

				BadgeLabelContainer.Hidden = string.IsNullOrEmpty(text);
			}
		}
	}
}
