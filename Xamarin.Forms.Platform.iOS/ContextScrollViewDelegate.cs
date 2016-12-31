using System;
using System.Collections.Generic;
using UIKit;
using NSAction = System.Action;
using PointF = CoreGraphics.CGPoint;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	internal class iOS7ButtonContainer : UIView
	{
		readonly nfloat _buttonWidth;

		public iOS7ButtonContainer(nfloat buttonWidth) : base(new RectangleF(0, 0, 0, 0))
		{
			_buttonWidth = buttonWidth;
			ClipsToBounds = true;
		}

		public override void LayoutSubviews()
		{
			var width = Frame.Width;
			nfloat takenSpace = 0;

			for (var i = 0; i < Subviews.Length; i++)
			{
				var view = Subviews[i];

				var pos = Subviews.Length - i;
				var x = width - _buttonWidth * pos;
				view.Frame = new RectangleF(x, 0, view.Frame.Width, view.Frame.Height);

				takenSpace += view.Frame.Width;
			}
		}
	}

	internal class ContextScrollViewDelegate : UIScrollViewDelegate
	{
		readonly nfloat _finalButtonSize;
		UIView _backgroundView;
		List<UIButton> _buttons;
		UITapGestureRecognizer _closer;
		UIView _container;
		GlobalCloseContextGestureRecognizer _globalCloser;

		bool _isDisposed;
		UIScrollView _scrollView;
		UITableView _table;

		public ContextScrollViewDelegate(UIScrollView scrollView, UIView container, List<UIButton> buttons, bool isOpen)
		{
			IsOpen = isOpen;
			_container = container;
			_buttons = buttons;
			_scrollView = scrollView;

			for (var i = 0; i < buttons.Count; i++)
			{
				var b = buttons[i];
				b.Hidden = !isOpen;

				ButtonsWidth += b.Frame.Width;
				_finalButtonSize = b.Frame.Width;
			}

			if (IsOpen)
			{
				RemoveHighlight();
				AddClosers();
			}
		}

		public nfloat ButtonsWidth { get; }

		public NSAction ClosedCallback { get; set; }

		public bool IsOpen { get; private set; }

		public override void DraggingStarted(UIScrollView scrollView)
		{
			if (!IsOpen)
				SetButtonsShowing(true);

			var cell = GetContextCell();
			if (!cell.Selected)
				return;

			if (!IsOpen)
				RemoveHighlight();
		}

		public void PrepareForDeselect()
		{
			RestoreHighlight();
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			var width = _finalButtonSize;
			var count = _buttons.Count;

			if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				_container.Frame = new RectangleF(scrollView.Frame.Width, 0, scrollView.ContentOffset.X, scrollView.Frame.Height);
			else
			{
				var ioffset = scrollView.ContentOffset.X / (float)count;

				if (ioffset > width)
					width = ioffset + 1;

				for (var i = count - 1; i >= 0; i--)
				{
					var b = _buttons[i];
					var rect = b.Frame;
					b.Frame = new RectangleF(scrollView.Frame.Width + (count - (i + 1)) * ioffset, 0, width, rect.Height);
				}
			}

			if (scrollView.ContentOffset.X == 0)
			{
				IsOpen = false;
				SetButtonsShowing(false);
				RestoreHighlight();

				ClearCloserRecognizer();

				if (ClosedCallback != null)
					ClosedCallback();
			}
		}

		public void Unhook()
		{
			RestoreHighlight();
			ClearCloserRecognizer();
		}

		public override void WillEndDragging(UIScrollView scrollView, PointF velocity, ref PointF targetContentOffset)
		{
			var width = ButtonsWidth;
			var x = targetContentOffset.X;
			var parentThreshold = scrollView.Frame.Width * .4f;
			var contentThreshold = width * .8f;

			if (x >= parentThreshold || x >= contentThreshold)
			{
				IsOpen = true;
				targetContentOffset = new PointF(width, 0);
				RemoveHighlight();

				AddClosers();
			}
			else
			{
				ClearCloserRecognizer();

				IsOpen = false;
				targetContentOffset = new PointF(0, 0);

				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
					RestoreHighlight();
			}
		}

		private void AddClosers()
		{
			NSAction close = () =>
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
					RestoreHighlight();

				IsOpen = false;
				_scrollView.SetContentOffset(new PointF(0, 0), true);

				ClearCloserRecognizer();
			};

			if (_globalCloser == null)
			{
				UIView view = _scrollView;
				while (view.Superview != null)
				{
					view = view.Superview;

					var table = view as UITableView;
					if (table != null)
					{
						_table = table;
						_globalCloser = new GlobalCloseContextGestureRecognizer(_scrollView, close);
						_globalCloser.ShouldRecognizeSimultaneously = (recognizer, r) => r == _table.PanGestureRecognizer;
						table.AddGestureRecognizer(_globalCloser);

						_closer = new UITapGestureRecognizer(close);
						var cell = GetContextCell();
						cell.ContentCell.AddGestureRecognizer(_closer);

						break;
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing)
			{
				ClearCloserRecognizer();
				ClosedCallback = null;

				_backgroundView = null;
				_container = null;

				_buttons = null;
				_scrollView = null;
			}

			base.Dispose(disposing);
		}

		void ClearCloserRecognizer()
		{
			if (_globalCloser == null || _globalCloser.State == UIGestureRecognizerState.Cancelled)
				return;

			var cell = GetContextCell();
			cell.ContentCell.RemoveGestureRecognizer(_closer);
			_closer.Dispose();
			_closer = null;

			_table.RemoveGestureRecognizer(_globalCloser);
			_table = null;
			_globalCloser.Dispose();
			_globalCloser = null;
		}

		ContextActionsCell GetContextCell()
		{
			var view = _scrollView.Superview.Superview;
			var cell = view as ContextActionsCell;
			while (view.Superview != null)
			{
				cell = view as ContextActionsCell;
				if (cell != null)
					break;

				view = view.Superview;
			}

			return cell;
		}

		void RemoveHighlight()
		{
			var subviews = _scrollView.Superview.Superview.Subviews;

			var count = 0;
			for (var i = 0; i < subviews.Length; i++)
			{
				var s = subviews[i];
				if (s.Frame.Height > 1)
					count++;
			}

			if (count <= 1)
				return;

			_backgroundView = subviews[0];
			_backgroundView.RemoveFromSuperview();

			var cell = GetContextCell();
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		void RestoreHighlight()
		{
			if (_backgroundView == null)
				return;

			var cell = GetContextCell();
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
			cell.SetSelected(true, false);

			_scrollView.Superview.Superview.InsertSubview(_backgroundView, 0);
			_backgroundView = null;
		}

		void SetButtonsShowing(bool show)
		{
			for (var i = 0; i < _buttons.Count; i++)
				_buttons[i].Hidden = !show;
		}
	}
}