using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using GravityFlags = Android.Views.GravityFlags;

namespace Xamarin.Forms.Platform.Android
{
	// todo - convert to IVisualElementRenderer since we don't need everything from VisualElementRenderer
	public class PopupRenderer : VisualElementRenderer<BasePopup>, IDialogInterfaceOnCancelListener
	{
		BasePopup _popup;
		Dialog _dialog;
		ContainerView _container;
		bool _isDisposed = false;

		public PopupRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			_popup = e.NewElement;
			base.OnElementChanged(e);

			CreateControl();
			SetEvents();
			SetColor();
			SetSize();
			SetAnchor();

			_dialog.Show();
		}

		private void CreateControl()
		{
			_container = new ContainerView(Context, _popup.View);
			_dialog = new Dialog(Context);
			_dialog.SetContentView(_container);
		}

		private void SetEvents()
		{
			_dialog.SetOnCancelListener(this);
			_popup.Dismissed += OnDismissed;
		}

		private void SetColor()
		{
			_dialog.Window.SetBackgroundDrawable(new ColorDrawable(_popup.Color.ToAndroid()));
		}

		private void SetSize()
		{
			if (_popup.Size != default)
			{
				var decorView = (ViewGroup)_dialog.Window.DecorView;
				var child = (FrameLayout)decorView.GetChildAt(0);

				var childLayoutParams = (FrameLayout.LayoutParams)child.LayoutParameters;
				childLayoutParams.Width = (int)_popup.Size.Width;
				childLayoutParams.Height = (int)_popup.Size.Height;
				child.LayoutParameters = childLayoutParams;

				int horizontalParams;
				switch (_popup.View.HorizontalOptions.Alignment)
				{
					case LayoutAlignment.Center:
					case LayoutAlignment.End:
					case LayoutAlignment.Start:
						horizontalParams = LayoutParams.WrapContent;
						break;
					case LayoutAlignment.Fill:
						horizontalParams = LayoutParams.MatchParent;
						break;
				}

				int verticalParams;
				switch (_popup.View.VerticalOptions.Alignment)
				{
					case LayoutAlignment.Center:
					case LayoutAlignment.End:
					case LayoutAlignment.Start:
						verticalParams = LayoutParams.WrapContent;
						break;
					case LayoutAlignment.Fill:
						verticalParams = LayoutParams.MatchParent;
						break;
				}

				if (_popup.View.WidthRequest > -1)
				{
					var inputMeasuredWidth = _popup.View.WidthRequest > _popup.Size.Width ?
						(int)_popup.Size.Width : (int)_popup.View.WidthRequest;
					_container.Measure(inputMeasuredWidth, (int)MeasureSpecMode.Unspecified);
					horizontalParams = _container.MeasuredWidth;
				}
				else
				{
					_container.Measure((int)_popup.Size.Width, (int)MeasureSpecMode.Unspecified);
					horizontalParams = _container.MeasuredWidth > _popup.Size.Width ?
						(int)_popup.Size.Width : _container.MeasuredWidth;
				}

				if (_popup.View.HeightRequest > -1)
					verticalParams = (int)_popup.View.HeightRequest;
				else
				{
					var inputMeasuredWidth = _popup.View.WidthRequest > -1 ? horizontalParams : (int)_popup.Size.Width;
					_container.Measure(inputMeasuredWidth, (int)MeasureSpecMode.Unspecified);
					verticalParams = _container.MeasuredHeight > _popup.Size.Height ?
						(int)_popup.Size.Height : _container.MeasuredHeight;
				}

				var containerLayoutParams = new FrameLayout.LayoutParams(horizontalParams, verticalParams);

				switch (_popup.View.VerticalOptions.Alignment)
				{
					case LayoutAlignment.Start:
						containerLayoutParams.Gravity = GravityFlags.Top;
						break;
					case LayoutAlignment.Center:
					case LayoutAlignment.Fill:
						containerLayoutParams.Gravity = GravityFlags.CenterVertical;
						break;
					case LayoutAlignment.End:
						containerLayoutParams.Gravity = GravityFlags.Bottom;
						break;
				}

				switch (_popup.View.HorizontalOptions.Alignment)
				{
					case LayoutAlignment.Start:
						containerLayoutParams.Gravity |= GravityFlags.Left;
						break;
					case LayoutAlignment.Center:
					case LayoutAlignment.Fill:
						containerLayoutParams.Gravity |= GravityFlags.CenterHorizontal;
						break;
					case LayoutAlignment.End:
						containerLayoutParams.Gravity |= GravityFlags.Right;
						break;
				}

				_container.LayoutParameters = containerLayoutParams;
			}
		}

		private void SetAnchor()
		{
			if (_popup.Anchor != null)
			{
				var anchorView = Platform.GetRenderer(_popup.Anchor).View;
				int[] locationOnScreen = new int[2];
				anchorView.GetLocationOnScreen(locationOnScreen);

				_dialog.Window.SetGravity(GravityFlags.Top | GravityFlags.Left);
				_dialog.Window.DecorView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

				// This logic is tricky, please read these notes if you need to modify
				// Android window coordinate starts (0,0) at the top left and (max,max) at the bottom right. All of the positions
				// that are being handled in this operation assume the point is at the top left of the rectangle. This means the
				// calculation operates in this order:
				// 1. Calculate top-left position of Anchor
				// 2. Calculate the Actual Center of the Anchor by adding the width /2 and height / 2
				// 3. Calculate the top-left point of where the dialog should be positioned by subtracting the Width / 2 and height / 2
				//    of the dialog that is about to be drawn.
				_dialog.Window.Attributes.X = locationOnScreen[0] + (anchorView.Width / 2) - (_dialog.Window.DecorView.MeasuredWidth / 2);
				_dialog.Window.Attributes.Y = locationOnScreen[1] + (anchorView.Height / 2) - (_dialog.Window.DecorView.MeasuredHeight / 2);
			}
		}

		private void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			_dialog.Dismiss();
		}

		public void OnCancel(IDialogInterface dialog)
		{
			if (Element is BasePopup popup)
			{
				popup.LightDismiss();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_isDisposed)
			{
				_isDisposed = true;
				if (_popup != null)
				{
					_popup.Dismissed -= OnDismissed;
					_popup = null;
				}

				_dialog?.Dispose();
				_dialog = null;
			}

			base.Dispose(disposing);
		}
	}
}