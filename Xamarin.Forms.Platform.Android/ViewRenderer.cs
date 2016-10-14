using System;
using System.ComponentModel;
using Android.App;
using Android.OS;
using Android.Views;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public abstract class ViewRenderer : ViewRenderer<View, AView>
	{
	}

	public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView>, AView.IOnFocusChangeListener where TView : View where TNativeView : AView
	{
		protected virtual TNativeView CreateNativeControl()
		{
			return default(TNativeView);
		}

		ViewGroup _container;

		bool _disposed;
		EventHandler<VisualElement.FocusRequestArgs> _focusChangeHandler;

		SoftInput _startingInputMode;

		internal bool HandleKeyboardOnFocus;

		public TNativeView Control { get; private set; }

		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			if (Element is Entry || Element is SearchBar || Element is Editor)
			{
				var isInViewCell = false;
				Element parent = Element.RealParent;
				while (!(parent is Page) && parent != null)
				{
					if (parent is Cell)
					{
						isInViewCell = true;
						break;
					}
					parent = parent.RealParent;
				}

				if (isInViewCell)
				{
					Window window = ((Activity)Context).Window;
					if (hasFocus)
					{
						_startingInputMode = window.Attributes.SoftInputMode;
						window.SetSoftInputMode(SoftInput.AdjustPan);
					}
					else
						window.SetSoftInputMode(_startingInputMode);
				}
			}
			OnNativeFocusChanged(hasFocus);

			((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (Control == null)
				return (base.GetDesiredSize(widthConstraint, heightConstraint));
				        
			AView view = _container == this ? (AView)Control : _container;
			view.Measure(widthConstraint, heightConstraint);

			return new SizeRequest(new Size(Control.MeasuredWidth, Control.MeasuredHeight), MinimumSize());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (Control != null && ManageNativeControlLifetime)
				{
					Control.OnFocusChangeListener = null;
					RemoveView(Control);
					Control.Dispose();
					Control = null;
				}

				if (_container != null && _container != this)
				{
					_container.RemoveFromParent();
					_container.Dispose();
					_container = null;
				}

				if (Element != null && _focusChangeHandler != null)
				{
					Element.FocusChangeRequested -= _focusChangeHandler;
					_focusChangeHandler = null;
				}

				_disposed = true;
			}

			base.Dispose(disposing);
		}

		[Obsolete("Use SetupElement, SetupControl, TearDownElement, and TearDownControl instead.")]
		protected override void OnElementChanged(ElementChangedEventArgs<TView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				SetupElement(e.NewElement);
				SetupControl(e.NewElement);
			}

			if (e.OldElement != null)
			{
				TearDownElement(e.OldElement);
				TearDownControl(e.OldElement);
			}
		}

		/// <summary>
		/// Subscribe event handlers to and update properties of the new element.
		/// This method is called before SetupControl().
		/// <param name="newElement">New element Element is set to.</param>
		/// </summary>
		protected virtual void SetupElement(TView newElement)
		{
			if (_focusChangeHandler == null)
				_focusChangeHandler = OnFocusChangeRequested;

			newElement.FocusChangeRequested += _focusChangeHandler;
		}

		/// <summary>
		/// Subscribe event handlers to and update properties of Control.
		/// Use newElement if needed while building Control.
		/// Optional: Override CreateNativeControl() and call SetNativeControl() to replace default Control.
		/// This method is called after SetupElement().
		/// <param name="newElement">New element Element is set to.</param>
		/// </summary>
		protected virtual void SetupControl(TView newElement)
		{
		}

		/// <summary>
		/// Unsubscribe event handlers from Element and clean up other resources as necessary.
		/// This method is called before TearDownControl().
		/// <param name="oldElement">Old element before Element is set to the new element.</param>
		/// </summary>
		protected virtual void TearDownElement(TView oldElement)
		{
			oldElement.FocusChangeRequested -= _focusChangeHandler;
		}

		/// <summary>
		/// Unsubscribe event handlers from Control and clean up other resources as necessary.
		/// This method is called after TearDownElement().
		/// <param name="oldElement">Old element before Element is set to the new element.</param>
		/// </summary>
		protected virtual void TearDownControl(TView oldElement)
		{
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateIsEnabled();
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);
			if (Control == null)
				return;

			AView view = _container == this ? (AView)Control : _container;

			view.Measure(MeasureSpecFactory.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly));
			view.Layout(0, 0, r - l, b - t);
		}

		protected override void OnRegisterEffect(PlatformEffect effect)
		{
			base.OnRegisterEffect(effect);
			effect.Control = Control;
		}

		protected override void SetAutomationId(string id)
		{
			if (Control == null)
				base.SetAutomationId(id);
			else
			{
				ContentDescription = id + "_Container";
				Control.ContentDescription = id;
			}
		}

		protected void SetNativeControl(TNativeView control)
		{
			SetNativeControl(control, this);
		}

		internal virtual void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			if (Control == null)
				return;

			e.Result = true;

			if (e.Focus)
			{
				// use post being BeginInvokeOnMainThread will not delay on android
				Looper looper = Context.MainLooper;
				var handler = new Handler(looper);
				handler.Post(() =>
				{
					Control?.RequestFocus();
				});
			}
			else
			{
				Control.ClearFocus();
			}

			//handles keyboard on focus for Editor, Entry and SearchBar
			if (HandleKeyboardOnFocus)
			{
				if (e.Focus)
					Control.ShowKeyboard();
				else
					Control.HideKeyboard();
			}
		}

		internal virtual void OnNativeFocusChanged(bool hasFocus)
		{
		}

		internal override void SendVisualElementInitialized(VisualElement element, AView nativeView)
		{
			base.SendVisualElementInitialized(element, Control);
		}

		internal void SetNativeControl(TNativeView control, ViewGroup container)
		{
			if (Control != null)
			{
				Control.OnFocusChangeListener = null;
				RemoveView(Control);
			}

			_container = container;

			Control = control;

			AView toAdd = container == this ? control : (AView)container;
			AddView(toAdd, LayoutParams.MatchParent);

			Control.OnFocusChangeListener = this;

			UpdateIsEnabled();
		}

		void UpdateIsEnabled()
		{
			if (Control != null)
				Control.Enabled = Element.IsEnabled;
		}
	}
}
