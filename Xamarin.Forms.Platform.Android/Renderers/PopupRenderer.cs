using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Internals;
using static Android.App.ActionBar;
using AView = Android.Views.View;
using GravityFlags = Android.Views.GravityFlags;

namespace Xamarin.Forms.Platform.Android
{
	public class PopupRenderer : Dialog, IVisualElementRenderer, IDialogInterfaceOnCancelListener
	{
		public BasePopup Element { get; private set; }
		void IVisualElementRenderer.UpdateLayout() => _tracker?.UpdateLayout();
		VisualElement IVisualElementRenderer.Element => Element;
		AView IVisualElementRenderer.View => _container;
		ViewGroup IVisualElementRenderer.ViewGroup => null;
		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;
		
		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		int? _defaultLabelFor;
		VisualElementTracker _tracker;
		ContainerView _container;
		bool _isDisposed = false;
		public PopupRenderer(Context context) :base(context)
		{
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			if (!(element is BasePopup popup))
				throw new ArgumentNullException("Element is not of type " + typeof(BasePopup), nameof(element));

			BasePopup oldElement = Element;
			Element = popup;
			CreateControl();

			Performance.Start(out string reference);

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;

			element.PropertyChanged += OnElementPropertyChanged;

			if (_tracker == null)
				_tracker = new VisualElementTracker(this);

			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldElement, Element));
			Element?.SendViewInitialized(_container);

			Performance.Stop(reference);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			if (e.NewElement != null && !_isDisposed)
			{
				SetEvents();
				SetColor();
				SetSize();
				SetAnchor();

				Show();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == BasePopup.VerticalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.HorizontalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.SizeProperty.PropertyName)
			{
				SetSize();
				SetAnchor();
			}
			else if (args.PropertyName == BasePopup.ColorProperty.PropertyName)
			{
				SetColor();
			}

			ElementPropertyChanged?.Invoke(this, args);
		}

		private void CreateControl()
		{
			if (_container == null)
			{
				_container = new ContainerView(Context, Element.View);
				SetContentView(_container);
			}
		}

		private void SetEvents()
		{
			SetOnCancelListener(this);
			Element.Dismissed += OnDismissed;
		}

		private void SetColor()
		{
			Window.SetBackgroundDrawable(new ColorDrawable(Element.Color.ToAndroid()));
		}

		private void SetSize()
		{
			if (Element.Size != default)
			{
				var decorView = (ViewGroup)Window.DecorView;
				var child = (FrameLayout)decorView.GetChildAt(0);

				var childLayoutParams = (FrameLayout.LayoutParams)child.LayoutParameters;
				childLayoutParams.Width = (int)Element.Size.Width;
				childLayoutParams.Height = (int)Element.Size.Height;
				child.LayoutParameters = childLayoutParams;

				int horizontalParams = -1;
				switch (Element.View.HorizontalOptions.Alignment)
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

				int verticalParams = -1;
				switch (Element.View.VerticalOptions.Alignment)
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

				if (Element.View.WidthRequest > -1)
				{
					var inputMeasuredWidth = Element.View.WidthRequest > Element.Size.Width ?
						(int)Element.Size.Width : (int)Element.View.WidthRequest;
					_container.Measure(inputMeasuredWidth, (int)MeasureSpecMode.Unspecified);
					horizontalParams = _container.MeasuredWidth;
				}
				else
				{
					_container.Measure((int)Element.Size.Width, (int)MeasureSpecMode.Unspecified);
					horizontalParams = _container.MeasuredWidth > Element.Size.Width ?
						(int)Element.Size.Width : _container.MeasuredWidth;
				}

				if (Element.View.HeightRequest > -1)
					verticalParams = (int)Element.View.HeightRequest;
				else
				{
					var inputMeasuredWidth = Element.View.WidthRequest > -1 ? horizontalParams : (int)Element.Size.Width;
					_container.Measure(inputMeasuredWidth, (int)MeasureSpecMode.Unspecified);
					verticalParams = _container.MeasuredHeight > Element.Size.Height ?
						(int)Element.Size.Height : _container.MeasuredHeight;
				}

				var containerLayoutParams = new FrameLayout.LayoutParams(horizontalParams, verticalParams);

				switch (Element.View.VerticalOptions.Alignment)
				{
					case LayoutAlignment.Start:
						containerLayoutParams.Gravity = GravityFlags.Top;
						break;
					case LayoutAlignment.Center:
					case LayoutAlignment.Fill:
						containerLayoutParams.Gravity = GravityFlags.FillVertical;
						containerLayoutParams.Height = (int)Element.Size.Height;
						_container.MatchHeight = true;
						break;
					case LayoutAlignment.End:
						containerLayoutParams.Gravity = GravityFlags.Bottom;
						break;
				}

				switch (Element.View.HorizontalOptions.Alignment)
				{
					case LayoutAlignment.Start:
						containerLayoutParams.Gravity |= GravityFlags.Left;
						break;
					case LayoutAlignment.Center:
					case LayoutAlignment.Fill:
						containerLayoutParams.Gravity |= GravityFlags.FillHorizontal;
						containerLayoutParams.Width = (int)Element.Size.Width;
						_container.MatchWidth = true;
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
			if (Element.Anchor != null)
			{
				var anchorView = Platform.GetRenderer(Element.Anchor).View;
				int[] locationOnScreen = new int[2];
				anchorView.GetLocationOnScreen(locationOnScreen);

				Window.SetGravity(GravityFlags.Top | GravityFlags.Left);
				Window.DecorView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

				// This logic is tricky, please read these notes if you need to modify
				// Android window coordinate starts (0,0) at the top left and (max,max) at the bottom right. All of the positions
				// that are being handled in this operation assume the point is at the top left of the rectangle. This means the
				// calculation operates in this order:
				// 1. Calculate top-left position of Anchor
				// 2. Calculate the Actual Center of the Anchor by adding the width /2 and height / 2
				// 3. Calculate the top-left point of where the dialog should be positioned by subtracting the Width / 2 and height / 2
				//    of the dialog that is about to be drawn.
				Window.Attributes.X = locationOnScreen[0] + (anchorView.Width / 2) - (Window.DecorView.MeasuredWidth / 2);
				Window.Attributes.Y = locationOnScreen[1] + (anchorView.Height / 2) - (Window.DecorView.MeasuredHeight / 2);
			}
			else
				SetDialogPosition();
		}

		void SetDialogPosition()
		{
			GravityFlags gravityFlags = GravityFlags.Center;
			switch (Element.VerticalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					gravityFlags = GravityFlags.Top;
					break;
				case LayoutAlignment.End:
					gravityFlags = GravityFlags.Bottom;
					break;
				default:
					gravityFlags = GravityFlags.CenterVertical;
					break;
			}

			switch (Element.HorizontalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					gravityFlags |= GravityFlags.Left;
					break;
				case LayoutAlignment.End:
					gravityFlags |= GravityFlags.Right;
					break;
				default:
					gravityFlags |= GravityFlags.CenterHorizontal;
					break;
			}

			Window.SetGravity(gravityFlags);
		}

		private void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			Dismiss();
		}

		public void OnCancel(IDialogInterface dialog)
		{
			if (Element is Popup popup)
			{
				popup.LightDismiss();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;
			if (disposing)
			{
				_tracker?.Dispose();
				_tracker = null;

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (Android.Platform.GetRenderer(Element) == this)
						Element.ClearValue(Android.Platform.RendererProperty);

					Element = null;
				}
			}

			base.Dispose(disposing);
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_isDisposed || _container == null)
				return new SizeRequest();

			_container.Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(_container.MeasuredWidth, _container.MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = _container.LabelFor;

			_container.LabelFor = (int)(id ?? _defaultLabelFor);
		}
	}
}