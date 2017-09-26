using Android.Content;
using Android.Views;
using AView = Android.Views.View;
using Xamarin.Forms.Internals;
using System;
using System.Linq;
using Android.Runtime;

namespace Xamarin.Forms.Platform.Android
{
	public class ViewCellRenderer : CellRenderer
	{
		protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
		{
			Performance.Start();
			var cell = (ViewCell)item;

			var container = convertView as ViewCellContainer;
			if (container != null)
			{
				container.Update(cell);
				Performance.Stop();
				return container;
			}

			BindableProperty unevenRows = null, rowHeight = null;
			if (ParentView is TableView)
			{
				unevenRows = TableView.HasUnevenRowsProperty;
				rowHeight = TableView.RowHeightProperty;
			}
			else if (ParentView is ListView)
			{
				unevenRows = ListView.HasUnevenRowsProperty;
				rowHeight = ListView.RowHeightProperty;
			}

			if (cell.View == null)
				throw new InvalidOperationException($"ViewCell must have a {nameof(cell.View)}");

			IVisualElementRenderer view = Platform.CreateRenderer(cell.View);
			Platform.SetRenderer(cell.View, view);
			cell.View.IsPlatformEnabled = true;
			var c = new ViewCellContainer(context, view, cell, ParentView, unevenRows, rowHeight);

			Performance.Stop();

			return c;
		}

		internal class ViewCellContainer : ViewGroup, INativeElementView
		{
			readonly View _parent;
			readonly BindableProperty _rowHeight;
			readonly BindableProperty _unevenRows;
			IVisualElementRenderer _view;
			ViewCell _viewCell;
			GestureDetector _longPressGestureDetector;
			ListViewRenderer _listViewRenderer;
			bool _watchForLongPress;

			ListViewRenderer ListViewRenderer
			{
				get
				{
					if (_listViewRenderer != null)
					{
						return _listViewRenderer;
					}

					var listView = _parent as ListView;

					if (listView == null)
					{
						return null;
					}

					_listViewRenderer = Platform.GetRenderer(listView) as ListViewRenderer;

					return _listViewRenderer;
				}
			}

			GestureDetector LongPressGestureDetector
			{
				get
				{
					if (_longPressGestureDetector != null)
					{
						return _longPressGestureDetector;
					}

					_longPressGestureDetector = new GestureDetector(new LongPressGestureListener(TriggerLongClick));
					return _longPressGestureDetector;
				}
			}

			public ViewCellContainer(Context context, IVisualElementRenderer view, ViewCell viewCell, View parent, 
				BindableProperty unevenRows, BindableProperty rowHeight) : base(context)
			{
				_view = view;
				_parent = parent;
				_unevenRows = unevenRows;
				_rowHeight = rowHeight;
				_viewCell = viewCell;
				AddView(view.View);
				UpdateIsEnabled();
				UpdateWatchForLongPress();
			}

			protected bool ParentHasUnevenRows
			{
				get { return (bool)_parent.GetValue(_unevenRows); }
			}

			protected int ParentRowHeight
			{
				get { return (int)_parent.GetValue(_rowHeight); }
			}

			public Element Element
			{
				get { return _viewCell; }
			}

			public override bool OnInterceptTouchEvent(MotionEvent ev)
			{
				if (!Enabled)
					return true;

				if (_watchForLongPress)
				{
					LongPressGestureDetector.OnTouchEvent(ev);
				}

				return base.OnInterceptTouchEvent(ev);
			}

			public void Update(ViewCell cell)
			{
				Performance.Start();

				var renderer = GetChildAt(0) as IVisualElementRenderer;
				var viewHandlerType = Registrar.Registered.GetHandlerType(cell.View.GetType()) ?? typeof(Platform.DefaultRenderer);
				if (renderer != null && renderer.GetType() == viewHandlerType)
				{
					Performance.Start("Reuse");
					_viewCell = cell;

					cell.View.DisableLayout = true;
					foreach (VisualElement c in cell.View.Descendants())
						c.DisableLayout = true;

					Performance.Start("Reuse.SetElement");
					renderer.SetElement(cell.View);
					Performance.Stop("Reuse.SetElement");

					Platform.SetRenderer(cell.View, _view);

					cell.View.DisableLayout = false;
					foreach (VisualElement c in cell.View.Descendants())
						c.DisableLayout = false;

					var viewAsLayout = cell.View as Layout;
					if (viewAsLayout != null)
						viewAsLayout.ForceLayout();

					Invalidate();

					Performance.Stop("Reuse");
					Performance.Stop();
					return;
				}

				RemoveView(_view.View);
				Platform.SetRenderer(_viewCell.View, null);
				_viewCell.View.IsPlatformEnabled = false;
				_view.View.Dispose();

				_viewCell = cell;
				_view = Platform.CreateRenderer(_viewCell.View);

				Platform.SetRenderer(_viewCell.View, _view);
				AddView(_view.View);

				UpdateIsEnabled();
				UpdateWatchForLongPress();

				Performance.Stop();
			}

			public void UpdateIsEnabled()
			{
				Enabled = _viewCell.IsEnabled;
			}

			protected override void OnLayout(bool changed, int l, int t, int r, int b)
			{
				Performance.Start();

				double width = Context.FromPixels(r - l);
				double height = Context.FromPixels(b - t);

				Performance.Start("Element.Layout");
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(_view.Element, new Rectangle(0, 0, width, height));
				Performance.Stop("Element.Layout");

				_view.UpdateLayout();
				Performance.Stop();
			}

			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{
				Performance.Start();

				int width = MeasureSpec.GetSize(widthMeasureSpec);
				int height;

				if (ParentHasUnevenRows)
				{
					SizeRequest measure = _view.Element.Measure(Context.FromPixels(width), double.PositiveInfinity, MeasureFlags.IncludeMargins);
					height = (int)Context.ToPixels(_viewCell.Height > 0 ? _viewCell.Height : measure.Request.Height);
				}
				else
					height = (int)Context.ToPixels(ParentRowHeight == -1 ? BaseCellView.DefaultMinHeight : ParentRowHeight);

				SetMeasuredDimension(width, height);

				Performance.Stop();
			}

			void UpdateWatchForLongPress()
			{
				var vw = _view.Element as Xamarin.Forms.View;
				if (vw == null)
				{
					return;
				}

				// If the view cell has any context actions and the View itself has any Tap Gestures, they're going
				// to conflict with one another - the Tap Gesture handling will prevent the ListViewAdapter's
				// LongClick handling from happening. So we need to watch locally for LongPress and if we see it,
				// trigger the LongClick manually.
				_watchForLongPress = _viewCell.ContextActions.Count > 0 
					&& vw.GestureRecognizers.Any(t => t is TapGestureRecognizer);
			}

			void TriggerLongClick()
			{
				ListViewRenderer?.LongClickOn(this);
			}

			internal class LongPressGestureListener : Java.Lang.Object, GestureDetector.IOnGestureListener
			{
				readonly Action _onLongClick;

				internal LongPressGestureListener(Action onLongClick)
				{
					_onLongClick = onLongClick;
				}

				internal LongPressGestureListener(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
				{
				}

				public bool OnDown(MotionEvent e)
				{
					return true;
				}

				public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
				{
					return false;
				}

				public void OnLongPress(MotionEvent e)
				{
					_onLongClick();
				}

				public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
				{
					return false;
				}

				public void OnShowPress(MotionEvent e)
				{
					
				}

				public bool OnSingleTapUp(MotionEvent e)
				{
					return false;
				}
			}
		}
	}
}