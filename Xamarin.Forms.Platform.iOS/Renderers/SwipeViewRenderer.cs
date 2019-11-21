using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static Xamarin.Forms.SwipeView;
using Specifics = Xamarin.Forms.PlatformConfiguration.iOSSpecific.SwipeView;

namespace Xamarin.Forms.Platform.iOS
{
	public class SwipeViewRenderer : ViewRenderer<SwipeView, UIView>
	{
		const double SwipeThreshold = 250;
		const int SwipeThresholdMargin = 6;
		const double SwipeItemWidth = 100;
		const double SwipeAnimationDuration = 0.2;

		View _scrollParent;
		UIView _contentView;
		UIStackView _actionView;
		UITapGestureRecognizer _tapGestureRecognizer;
		SwipeTransitionMode _swipeTransitionMode;
		SwipeDirection? _swipeDirection;
		CGPoint _initialPoint;
		bool _isTouchDown;
		bool _isSwiping;
		double _swipeOffset;
		double _swipeThreshold;
		CGRect _originalBounds;
		List<CGRect> _swipeItemsRect;
		bool _isDisposed;

		public SwipeViewRenderer()
		{
			_tapGestureRecognizer = new UITapGestureRecognizer(OnTap)
			{
				CancelsTouchesInView = true
			};
			AddGestureRecognizer(_tapGestureRecognizer);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<SwipeView> e)
		{
			if (e.NewElement != null)
			{
				e.NewElement.CloseRequested += OnCloseRequested;

				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());
				}

				UpdateContent();
				UpdateSwipeTransitionMode();
				SetBackgroundColor(Element.BackgroundColor);
			}

			base.OnElementChanged(e);
		}

		protected override UIView CreateNativeControl()
		{
			return new UIView();
		}

		public override void WillMoveToWindow(UIWindow window)
		{
			base.WillMoveToWindow(window);

			if (window != null && _scrollParent == null)
			{
				_scrollParent = Element.FindParentOfType<ScrollView>();

				if (_scrollParent is ScrollView scrollView)
				{
					scrollView.Scrolled += OnParentScrolled;
					return;
				}

				_scrollParent = Element.FindParentOfType<ListView>();

				if (_scrollParent is ListView listView)
				{
					listView.Scrolled += OnParentScrolled;
					return;
				}

				_scrollParent = Element.FindParentOfType<CollectionView>();

				if (_scrollParent is CollectionView collectionView)
				{
					collectionView.Scrolled += OnParentScrolled;
				}
			}
		}
  
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (Element.Content != null)
				Element.Content.Layout(Bounds.ToRectangle());

			if (_contentView != null)
				_contentView.Frame = Bounds;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ContentView.ContentProperty.PropertyName)
				UpdateContent();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				SetBackgroundColor(Element.BackgroundColor);
			else if (e.PropertyName == Specifics.SwipeTransitionModeProperty.PropertyName)
				UpdateSwipeTransitionMode();
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Element.BackgroundColor != Color.Default)
			{
				var backgroundColor = Element.BackgroundColor.ToUIColor();

				BackgroundColor = Element.BackgroundColor.ToUIColor();

				if (_contentView != null && Element.Content == null && HasSwipeItems())
					_contentView.BackgroundColor = backgroundColor;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Element != null)
				{
					Element.CloseRequested -= OnCloseRequested;
				}

				if (_scrollParent != null)
				{
					if(_scrollParent is ScrollView scrollView)
						scrollView.Scrolled -= OnParentScrolled;

					if (_scrollParent is ListView listView)
						listView.Scrolled -= OnParentScrolled;

					if (_scrollParent is CollectionView collectionView)
						collectionView.Scrolled -= OnParentScrolled;
				}

				if (_tapGestureRecognizer != null)
				{
					Control.RemoveGestureRecognizer(_tapGestureRecognizer);
					_tapGestureRecognizer.Dispose();
					_tapGestureRecognizer = null;
				}

				if (_contentView != null)
				{
					_contentView.Dispose();
					_contentView = null;
				}

				if (_actionView != null)
				{
					_actionView.Dispose();
					_actionView = null;
				}

				if (_swipeItemsRect != null)
				{
					_swipeItemsRect.Clear();
					_swipeItemsRect = null;
				}
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			var navigationController = GetUINavigationController(GetViewController());

			if (navigationController != null)
				navigationController.InteractivePopGestureRecognizer.Enabled = false;

			HandleTouchInteractions(touches, GestureStatus.Started);

			base.TouchesBegan(touches, evt);
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			HandleTouchInteractions(touches, GestureStatus.Running);
			base.TouchesMoved(touches, evt);
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			var navigationController = GetUINavigationController(GetViewController());

			if (navigationController != null)
				navigationController.InteractivePopGestureRecognizer.Enabled = true;

			HandleTouchInteractions(touches, GestureStatus.Completed);

			base.TouchesEnded(touches, evt);
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			var navigationController = GetUINavigationController(GetViewController());

			if (navigationController != null)
				navigationController.InteractivePopGestureRecognizer.Enabled = true;

			HandleTouchInteractions(touches, GestureStatus.Canceled);

			base.TouchesCancelled(touches, evt);
		}

		void UpdateContent()
		{
			ClipsToBounds = true;

			if (Element.Content == null)
				_contentView = CreateEmptyContent();
			else
				_contentView = CreateContent();

			AddSubview(_contentView);
		}

		UIView CreateEmptyContent()
		{
			var emptyContentView = new UIView
			{
				BackgroundColor = Color.Default.ToUIColor()
			};

			return emptyContentView;
		}

		UIView CreateContent()
		{
			var formsElement = Element.Content;
			var renderer = Platform.CreateRenderer(formsElement);
			Platform.SetRenderer(formsElement, renderer);

			return renderer?.NativeView;
		}

		bool HasSwipeItems()
		{
			return Element != null && (IsValidSwipeItems(Element.LeftItems) || IsValidSwipeItems(Element.RightItems) || IsValidSwipeItems(Element.TopItems) || IsValidSwipeItems(Element.BottomItems));
		}

		bool IsValidSwipeItems(SwipeItems swipeItems)
		{
			return swipeItems != null && swipeItems.Count > 0;
		}

		void UpdateSwipeItems()
		{
			if (_contentView == null || _actionView != null)
				return;

			_swipeItemsRect = new List<CGRect>();

			SwipeItems items = GetSwipeItemsByDirection();
			double swipeItemsWidth;

			if (_swipeDirection == SwipeDirection.Left || _swipeDirection == SwipeDirection.Right)
				swipeItemsWidth = (items != null ? items.Count : 0) * SwipeItemWidth;
			else
				swipeItemsWidth = _contentView.Frame.Width;

			if (items == null)
				return;

			_actionView = new UIStackView
			{
				Axis = UILayoutConstraintAxis.Horizontal,
				Frame = new CGRect(0, 0, swipeItemsWidth, _contentView.Frame.Height)
			};

			int i = 0;
			foreach (var item in items)
			{
				var swipeItemWidth = GetSwipeItemWidthByDirection();
				var swipeItem = new UIView();

				if (item is SwipeItem formsSwipeItem)
				{
					swipeItem = new UIButton(UIButtonType.Custom)
					{
						BackgroundColor = formsSwipeItem.BackgroundColor.ToUIColor()
					};

					((UIButton)swipeItem).SetTitle(formsSwipeItem.Text, UIControlState.Normal);

					UpdateSwipeItemIconImage(((UIButton)swipeItem), formsSwipeItem);

					var textColor = GetSwipeItemColor(formsSwipeItem.BackgroundColor);
					((UIButton)swipeItem).SetTitleColor(textColor.ToUIColor(), UIControlState.Normal);
					swipeItem.UserInteractionEnabled = false;
					UpdateSwipeItemInsets(((UIButton)swipeItem));
				}

				if (item is SwipeItemView formsSwipeItemView)
				{
					var formsElement = formsSwipeItemView;
					var renderer = Platform.CreateRenderer(formsSwipeItemView);
					Platform.SetRenderer(formsSwipeItemView, renderer);
					formsSwipeItemView.Layout(new Rectangle(0, 0, swipeItemWidth, _contentView.Frame.Height));
					swipeItem = renderer?.NativeView;
				}

				switch (_swipeDirection)
				{
					case SwipeDirection.Left:
						swipeItem.Frame = new CGRect(_contentView.Frame.Width - (i + 1 * swipeItemWidth), 0, i + 1 * swipeItemWidth, _contentView.Frame.Height);
						break;
					case SwipeDirection.Right:
						swipeItem.Frame = new CGRect(i * swipeItemWidth, 0, i + 1 * swipeItemWidth, _contentView.Frame.Height);
						break;
					case SwipeDirection.Up:
					case SwipeDirection.Down:
						swipeItem.Frame = new CGRect(i * swipeItemWidth, 0, i + 1 * swipeItemWidth, _contentView.Frame.Height);
						break;
				}

				_actionView.AddSubview(swipeItem);
				_swipeItemsRect.Add(swipeItem.Frame);

				i++;
			}

			AddSubview(_actionView);
			BringSubviewToFront(_contentView);
		}

		void UpdateSwipeTransitionMode()
		{
			if (Element.IsSet(Specifics.SwipeTransitionModeProperty))
				_swipeTransitionMode = Element.OnThisPlatform().GetSwipeTransitionMode();
			else
				_swipeTransitionMode = SwipeTransitionMode.Reveal;
		}

		void UpdateSwipeItemInsets(UIButton button, float spacing = 0.0f)
		{
			if (button.ImageView?.Image == null)
				return;

			var imageSize = button.ImageView.Image.Size;

			var titleEdgeInsets = new UIEdgeInsets(spacing, -imageSize.Width, -imageSize.Height, 0.0f);
			button.TitleEdgeInsets = titleEdgeInsets;

			var labelString = button.TitleLabel.Text;
			var titleSize = labelString.StringSize(button.TitleLabel.Font);
			var imageEdgeInsets = new UIEdgeInsets(-(titleSize.Height + spacing), 0.0f, 0.0f, -titleSize.Width);
			button.ImageEdgeInsets = imageEdgeInsets;
		}

		double GetSwipeItemWidthByDirection()
		{
			var swipeItemWidth = SwipeItemWidth;
			var items = GetSwipeItemsByDirection();

			switch (_swipeDirection)
			{
				case SwipeDirection.Left:
					swipeItemWidth = items.Mode == SwipeMode.Execute ? _contentView.Frame.Width / items.Count : SwipeItemWidth;
					break;
				case SwipeDirection.Right:
					swipeItemWidth = items.Mode == SwipeMode.Execute ? _contentView.Frame.Width / items.Count : SwipeItemWidth;
					break;
				case SwipeDirection.Up:
				case SwipeDirection.Down:
					swipeItemWidth = _contentView.Frame.Width / items.Count;
					break;
			}

			return swipeItemWidth;
		}

		Color GetSwipeItemColor(Color backgroundColor)
		{
			var luminosity = 0.2126 * backgroundColor.R + 0.7152 * backgroundColor.G + 0.0722 * backgroundColor.B;

			return luminosity < 0.75 ? Color.White : Color.Black;
		}

		async void UpdateSwipeItemIconImage(UIButton swipeButton, SwipeItem swipeItem)
		{
			if (swipeButton == null)
				return;

			if (swipeItem.IconImageSource == null || swipeItem.IconImageSource.IsEmpty)
			{
				swipeButton.SetImage(null, UIControlState.Normal);
			}
			else
			{
				var image = await swipeItem.IconImageSource.GetNativeImageAsync();

				try
				{
					swipeButton.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
					var tintColor = GetSwipeItemColor(swipeItem.BackgroundColor);
					swipeButton.TintColor = tintColor.ToUIColor();
				}
				catch (Exception)
				{
					// UIImage ctor throws on file not found if MonoTouch.ObjCRuntime.Class.ThrowOnInitFailure is true;
					Log.Warning("SwipeView", "Can not load SwipeItem Icon.");
				}
			}
		}

		void OnTap()
		{
			var state = _tapGestureRecognizer.State;

			if (state != UIGestureRecognizerState.Cancelled)
			{
				if (_contentView == null)
					return;

				var point = _tapGestureRecognizer.LocationInView(this);

				bool touchContent = TouchInsideContent(_contentView.Frame.X, _contentView.Frame.Y, _contentView.Frame.Width, _contentView.Frame.Height, point.X, point.Y);

				if (!touchContent)
					ProcessTouchSwipeItems(point);
				else
					ResetSwipe();
			}
		}

		void HandleTouchInteractions(NSSet touches, GestureStatus gestureStatus)
		{
			var anyObject = touches.AnyObject as UITouch;
			nfloat x = anyObject.LocationInView(this).X;
			nfloat y = anyObject.LocationInView(this).Y;

			HandleTouchInteractions(gestureStatus, new CGPoint(x, y));
		}

		void HandleTouchInteractions(GestureStatus status, CGPoint point)
		{
			switch (status)
			{
				case GestureStatus.Started:
					ProcessTouchDown(point);
					break;
				case GestureStatus.Running:
					ProcessTouchMove(point);
					break;
				case GestureStatus.Canceled:
				case GestureStatus.Completed:
					ProcessTouchUp();
					break;
			}

			_isTouchDown = false;
		}

		void ProcessTouchDown(CGPoint point)
		{
			if (_isSwiping || _isTouchDown || _contentView == null)
				return;

			_initialPoint = point;
			_isTouchDown = true;
		}

		bool TouchInsideContent(double x1, double y1, double x2, double y2, double x, double y)
		{
			if (x > x1 && x < (x1 + x2) && y > y1 && y < (y1 + y2))
				return true;

			return false;
		}

		void ProcessTouchMove(CGPoint point)
		{
			if (!_isSwiping)
			{
				_swipeDirection = SwipeDirectionHelper.GetSwipeDirection(new Point(_initialPoint.X, _initialPoint.Y), new Point(point.X, point.Y));
				RaiseSwipeStarted();
				_isSwiping = true;
			}

			if (!ValidateSwipeDirection())
				return;

			_swipeOffset = GetSwipeOffset(_initialPoint, point);

			UpdateSwipeItems();

			if (Math.Abs(_swipeOffset) > double.Epsilon)
				Swipe();
			else
				ResetSwipe();

			RaiseSwipeChanging();
		}

		void ProcessTouchUp()
		{
			_isTouchDown = false;
			_isSwiping = false;
			RaiseSwipeEnded();

			if (!ValidateSwipeDirection())
				return;

			ValidateSwipeThreshold();
		}

		SwipeItems GetSwipeItemsByDirection()
		{
			SwipeItems swipeItems = null;

			switch (_swipeDirection)
			{
				case SwipeDirection.Left:
					swipeItems = Element.RightItems;
					break;
				case SwipeDirection.Right:
					swipeItems = Element.LeftItems;
					break;
				case SwipeDirection.Up:
					swipeItems = Element.BottomItems;
					break;
				case SwipeDirection.Down:
					swipeItems = Element.TopItems;
					break;
			}

			return swipeItems;
		}

		void Swipe()
		{
			if (_contentView == null)
				return;

			_originalBounds = _contentView.Bounds;

			if (_swipeTransitionMode == SwipeTransitionMode.Reveal)
			{
				switch (_swipeDirection)
				{
					case SwipeDirection.Left:
						_contentView.Frame = new CGRect(_originalBounds.X + ValidateSwipeOffset(_swipeOffset), _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
						break;
					case SwipeDirection.Right:
						_contentView.Frame = new CGRect(_originalBounds.X + ValidateSwipeOffset(_swipeOffset), _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
						break;
					case SwipeDirection.Up:
						_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + ValidateSwipeOffset(_swipeOffset), _originalBounds.Width, _originalBounds.Height);
						break;
					case SwipeDirection.Down:
						_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + ValidateSwipeOffset(_swipeOffset), _originalBounds.Width, _originalBounds.Height);
						break;
				}
			}

			if (_swipeTransitionMode == SwipeTransitionMode.Drag)
			{
				var actionBounds = _actionView.Bounds;
				double actionSize;

				switch (_swipeDirection)
				{
					case SwipeDirection.Left:
						_contentView.Frame = new CGRect(_originalBounds.X + ValidateSwipeOffset(_swipeOffset), _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
						actionSize = Element.RightItems.Count * SwipeItemWidth;
						_actionView.Frame = new CGRect(actionSize + ValidateSwipeOffset(_swipeOffset), actionBounds.Y, actionBounds.Width, actionBounds.Height);
						break;
					case SwipeDirection.Right:
						_contentView.Frame = new CGRect(_originalBounds.X + ValidateSwipeOffset(_swipeOffset), _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
						actionSize = Element.LeftItems.Count * SwipeItemWidth;
						_actionView.Frame = new CGRect(-actionSize + ValidateSwipeOffset(_swipeOffset), actionBounds.Y, actionBounds.Width, actionBounds.Height);
						break;
					case SwipeDirection.Up:
						_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + ValidateSwipeOffset(_swipeOffset), _originalBounds.Width, _originalBounds.Height);
						actionSize = _contentView.Frame.Height;
						_actionView.Frame = new CGRect(actionBounds.X, actionSize - Math.Abs(ValidateSwipeOffset(_swipeOffset)), actionBounds.Width, actionBounds.Height);
						break;
					case SwipeDirection.Down:
						_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + ValidateSwipeOffset(_swipeOffset), _originalBounds.Width, _originalBounds.Height);
						actionSize = _contentView.Frame.Height;
						_actionView.Frame = new CGRect(actionBounds.X, -actionSize + Math.Abs(ValidateSwipeOffset(_swipeOffset)), actionBounds.Width, actionBounds.Height);
						break;
				}
			}
		}

		double ValidateSwipeOffset(double offset)
		{
			var swipeThreshold = GetSwipeThreshold();

			switch (_swipeDirection)
			{
				case SwipeDirection.Left:
					if (offset > 0)
						offset = 0;

					if (Math.Abs(offset) > swipeThreshold)
						return -swipeThreshold;
					break;
				case SwipeDirection.Right:
					if (offset < 0)
						offset = 0;

					if (Math.Abs(offset) > swipeThreshold)
						return swipeThreshold;
					break;
				case SwipeDirection.Up:
					if (offset > 0)
						offset = 0;

					if (Math.Abs(offset) > swipeThreshold)
						return -swipeThreshold;
					break;
				case SwipeDirection.Down:
					if (offset < 0)
						offset = 0;

					if (Math.Abs(offset) > swipeThreshold)
						return swipeThreshold;
					break;
			}

			return offset;
		}

		void DisposeSwipeItems()
		{
			if (_actionView != null)
			{
				_actionView.RemoveFromSuperview();
				_actionView.Dispose();
				_actionView = null;
			}

			if (_swipeItemsRect != null)
			{
				_swipeItemsRect.Clear();
				_swipeItemsRect = null;
			}
		}

		private void ResetSwipe()
		{
			if (_swipeItemsRect == null)
				return;

			if (_contentView != null)
			{
				_isSwiping = false;
				_swipeThreshold = 0;
				_swipeDirection = null;

				Animate(SwipeAnimationDuration, 0.0, UIViewAnimationOptions.CurveEaseOut, () =>
				{
					_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
				},
				() =>
				{
					DisposeSwipeItems();
				});
			}
		}

		void ValidateSwipeThreshold()
		{
			if (_swipeDirection == null)
				return;

			var swipeThresholdPercent = 0.6 * GetSwipeThreshold();

			if (Math.Abs(_swipeOffset) >= swipeThresholdPercent)
			{
				var swipeItems = GetSwipeItemsByDirection();

				if (swipeItems == null)
					return;

				if (swipeItems.Mode == SwipeMode.Execute)
				{
					foreach (var swipeItem in swipeItems)
					{
						ExecuteSwipeItem(swipeItem);
					}

					if (swipeItems.SwipeBehaviorOnInvoked != SwipeBehaviorOnInvoked.RemainOpen)
						ResetSwipe();
				}
				else
					CompleteSwipe();
			}
			else
			{
				ResetSwipe();
			}
		}

		void CompleteSwipe()
		{
			if (_swipeTransitionMode == SwipeTransitionMode.Reveal)
			{
				Animate(SwipeAnimationDuration, 0.0, UIViewAnimationOptions.CurveEaseIn,
					() =>
					{
						double swipeThreshold = GetSwipeThreshold();

						switch (_swipeDirection)
						{
							case SwipeDirection.Left:
								_contentView.Frame = new CGRect(_originalBounds.X - swipeThreshold, _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
								break;
							case SwipeDirection.Right:
								_contentView.Frame = new CGRect(_originalBounds.X + swipeThreshold, _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
								break;
							case SwipeDirection.Up:
								_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y - swipeThreshold, _originalBounds.Width, _originalBounds.Height);
								break;
							case SwipeDirection.Down:
								_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + swipeThreshold, _originalBounds.Width, _originalBounds.Height);
								break;
						}
					},
				   () =>
				   {
					   _isSwiping = false;
				   });
			}

			if (_swipeTransitionMode == SwipeTransitionMode.Drag)
			{
				Animate(SwipeAnimationDuration, 0.0, UIViewAnimationOptions.CurveEaseIn,
					() =>
					{
						double swipeThreshold = GetSwipeThreshold();
						var actionBounds = _actionView.Bounds;
						double actionSize;

						switch (_swipeDirection)
						{
							case SwipeDirection.Left:
								_contentView.Frame = new CGRect(_originalBounds.X - swipeThreshold, _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
								actionSize = Element.RightItems.Count * SwipeItemWidth;
								_actionView.Frame = new CGRect(actionSize - swipeThreshold, actionBounds.Y, actionBounds.Width, actionBounds.Height);
								break;
							case SwipeDirection.Right:
								_contentView.Frame = new CGRect(_originalBounds.X + swipeThreshold, _originalBounds.Y, _originalBounds.Width, _originalBounds.Height);
								actionSize = Element.LeftItems.Count * SwipeItemWidth;
								_actionView.Frame = new CGRect(-actionSize + swipeThreshold, actionBounds.Y, actionBounds.Width, actionBounds.Height);
								break;
							case SwipeDirection.Up:
								_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y - swipeThreshold, _originalBounds.Width, _originalBounds.Height);
								actionSize = _contentView.Frame.Height;
								_actionView.Frame = new CGRect(actionBounds.X, actionSize - Math.Abs(swipeThreshold), actionBounds.Width, actionBounds.Height);
								break;
							case SwipeDirection.Down:
								_contentView.Frame = new CGRect(_originalBounds.X, _originalBounds.Y + swipeThreshold, _originalBounds.Width, _originalBounds.Height);
								actionSize = _contentView.Frame.Height;
								_actionView.Frame = new CGRect(actionBounds.X, -actionSize + Math.Abs(swipeThreshold), actionBounds.Width, actionBounds.Height);
								break;
						}
					},
				   () =>
				   {
					   _isSwiping = false;
				   });
			}

		}

		double GetSwipeThreshold()
		{
			if (Math.Abs(_swipeThreshold) > double.Epsilon)
				return _swipeThreshold;

			_swipeThreshold = GetSwipeThreshold(GetSwipeItemsByDirection());

			return _swipeThreshold;
		}

		double GetSwipeThreshold(SwipeItems swipeItems)
		{
			double swipeThreshold = 0;

			if (swipeItems == null)
				return 0;

			bool isHorizontal = _swipeDirection == SwipeDirection.Left || _swipeDirection == SwipeDirection.Right;

			if (swipeItems.Mode == SwipeMode.Reveal)
			{
				if (isHorizontal)
				{
					foreach (var swipeItem in swipeItems)
						swipeThreshold += SwipeItemWidth;
				}
				else
					swipeThreshold = (SwipeThreshold > _contentView.Frame.Height) ? _contentView.Frame.Height : SwipeThreshold;
			}
			else
			{
				if (isHorizontal)
					swipeThreshold = SwipeThreshold;
				else
					swipeThreshold = (SwipeThreshold > _contentView.Frame.Height) ? _contentView.Frame.Height : SwipeThreshold;
			}

			if (isHorizontal)
				return swipeThreshold - SwipeThresholdMargin;
			return swipeThreshold - SwipeThresholdMargin / 2;
		}

		bool ValidateSwipeDirection()
		{
			var swipeItems = GetSwipeItemsByDirection();
			return IsValidSwipeItems(swipeItems);
		}

		double GetSwipeOffset(CGPoint initialPoint, CGPoint endPoint)
		{
			double swipeOffset = 0;

			switch (_swipeDirection)
			{
				case SwipeDirection.Left:
				case SwipeDirection.Right:
					swipeOffset = endPoint.X - initialPoint.X;
					break;
				case SwipeDirection.Up:
				case SwipeDirection.Down:
					swipeOffset = endPoint.Y - initialPoint.Y;
					break;
			}

			return swipeOffset;
		}

		void ProcessTouchSwipeItems(CGPoint point)
		{
			var swipeItems = GetSwipeItemsByDirection();

			if (swipeItems == null || _swipeItemsRect == null)
				return;

			int i = 0;

			foreach (var swipeItemRect in _swipeItemsRect)
			{
				var swipeItem = swipeItems[i];

				var swipeItemX = swipeItemRect.Left;
				var swipeItemY = swipeItemRect.Top;

				if (TouchInsideContent(swipeItemX, swipeItemY, swipeItemRect.Width, swipeItemRect.Height, point.X, point.Y))
				{
					ExecuteSwipeItem(swipeItem);

					if (swipeItems.SwipeBehaviorOnInvoked != SwipeBehaviorOnInvoked.RemainOpen)
						ResetSwipe();

					break;
				}

				i++;
			}
		}

		UIViewController GetViewController()
		{
			var window = UIApplication.SharedApplication.KeyWindow;
			var viewController = window.RootViewController;

			while (viewController.PresentedViewController != null)
				viewController = viewController.PresentedViewController;

			return viewController;
		}

		UINavigationController GetUINavigationController(UIViewController controller)
		{
			if (controller != null)
			{
				if (controller is UINavigationController)
				{
					return (controller as UINavigationController);
				}

				if (controller.ChildViewControllers.Any())
				{
					var childs = controller.ChildViewControllers.Count();

					for (int i = 0; i < childs; i++)
					{
						var child = GetUINavigationController(controller.ChildViewControllers[i]);

						if (child is UINavigationController)
						{
							return (child as UINavigationController);
						}
					}
				}
			}

			return null;
		}

		void ExecuteSwipeItem(ISwipeItem swipeItem)
		{
			if (swipeItem == null)
				return;

			swipeItem.OnInvoked();
		}

		void OnParentScrolled(object sender, ScrolledEventArgs e)
		{
			ResetSwipe();
		}

		void OnParentScrolled(object sender, ItemsViewScrolledEventArgs e)
		{
			if (e.HorizontalDelta > 10 || e.VerticalDelta > 10)
				ResetSwipe();
		}

		void OnCloseRequested(object sender, EventArgs e)
		{
			ResetSwipe();
		}

		void RaiseSwipeStarted()
		{
			if (_swipeDirection == null || !ValidateSwipeDirection())
				return;

			var swipeStartedEventArgs = new SwipeStartedEventArgs(_swipeDirection.Value);
			Element.SendSwipeStarted(swipeStartedEventArgs);
		}

		void RaiseSwipeChanging()
		{
			if (_swipeDirection == null)
				return;

			var swipeChangingEventArgs = new SwipeChangingEventArgs(_swipeDirection.Value, _swipeOffset);
			Element.SendSwipeChanging(swipeChangingEventArgs);
		}

		void RaiseSwipeEnded()
		{
			if (_swipeDirection == null || !ValidateSwipeDirection())
				return;

			var swipeEndedEventArgs = new SwipeEndedEventArgs(_swipeDirection.Value);
			Element.SendSwipeEnded(swipeEndedEventArgs);
		}
	}
}