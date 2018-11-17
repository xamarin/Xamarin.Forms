using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using AView = Android.Views.View;
using Android.Views;
using Xamarin.Forms.Internals;
using AColor = Android.Graphics.Color;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Android.Graphics.Drawables;
using Android.Graphics;
using Xamarin.Forms.Platform.Android.FastRenderers;
using Android.OS;
using Android.Widget;
using Android.Content.Res;
using Android.Support.V4.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class CheckBoxRenderer :
		AppCompatCheckBox,
		IVisualElementRenderer,
		AView.IOnFocusChangeListener,
		CompoundButton.IOnCheckedChangeListener
	{
		bool _disposed;
		bool _skipInvalidate;
		int? _defaultLabelFor;
		VisualElementTracker _tracker;
		VisualElementRenderer _visualElementRenderer;
		IPlatformElementConfiguration<PlatformConfiguration.Android, CheckBox> _platformElementConfiguration;
		private CheckBox _checkBox;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		void IVisualElementRenderer.UpdateLayout() => _tracker?.UpdateLayout();
		VisualElement IVisualElementRenderer.Element => CheckBox;
		AView IVisualElementRenderer.View => this;
		ViewGroup IVisualElementRenderer.ViewGroup => null;
		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

		CheckBox CheckBox
		{
			get => _checkBox;
			set
			{
				_checkBox = value;
				_platformElementConfiguration = null;
			}
		}


		AppCompatCheckBox Control => this;
		public CheckBoxRenderer(Context context) : base(context)
		{
			// These set the defaults so visually it matches up with other platforms
			SetPadding(0, 0, 0, 0);
			SoundEffectsEnabled = false;
			SetOnCheckedChangeListener(this);

			Tag = this;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_tracker?.Dispose();
				_tracker = null;


				if (CheckBox != null)
				{
					CheckBox.PropertyChanged -= OnElementPropertyChanged;

					if (Android.Platform.GetRenderer(CheckBox) == this)
					{
						CheckBox.ClearValue(Android.Platform.RendererProperty);
					}

					CheckBox = null;
				}
			}

			base.Dispose(disposing);
		}

		public override void Invalidate()
		{
			if (_skipInvalidate)
			{
				_skipInvalidate = false;
				return;
			}

			base.Invalidate();
		}

		Size MinimumSize()
		{
			return new Size();
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_disposed)
			{
				return new SizeRequest();
			}
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is CheckBox checkBox))
			{
				throw new ArgumentException("Element is not of type " + typeof(CheckBox), nameof(element));
			}

			CheckBox oldElement = CheckBox;
			CheckBox = checkBox;

			Performance.Start(out string reference);

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			element.PropertyChanged += OnElementPropertyChanged;

			if (_tracker == null)
			{
				_tracker = new VisualElementTracker(this);

			}

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
			}

			Performance.Stop(reference);
			this.EnsureId();
						
			UpdateOnColor();

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, CheckBox));
			CheckBox?.SendViewInitialized(Control);
		}

		// CheckBox related
		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CheckBox.CheckedColorProperty.PropertyName ||
				e.PropertyName == CheckBox.UnCheckedColorProperty.PropertyName)
			{
				UpdateOnColor();
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		void IOnCheckedChangeListener.OnCheckedChanged(CompoundButton buttonView, bool isChecked)
		{
			((IElementController)CheckBox).SetValueFromRenderer(CheckBox.IsCheckedProperty, isChecked);
		}

		void UpdateOnColor()
		{		

			if (CheckBox == null || Control == null)
				return;


			var mode = PorterDuff.Mode.SrcIn;

			var stateChecked = global::Android.Resource.Attribute.StateChecked;
			var stateEnabled = global::Android.Resource.Attribute.StateEnabled;

			//Need to find a way to get this color out of Android somehow.
			var uncheckedDefault = AColor.Gray;
			var disabledColor = AColor.LightGray;

			var list = new ColorStateList(
					new int[][] 
					{
						new int[] { -stateEnabled, stateChecked },
						new int[] { stateEnabled, stateChecked },
						new int[] { stateEnabled, -stateChecked },
						new int[] { },
					},
					new int[]
					{
						disabledColor,
						CheckBox.CheckedColor == Color.Default ? Color.Accent.ToAndroid() : CheckBox.CheckedColor.ToAndroid(),
						CheckBox.UnCheckedColor == Color.Default ? uncheckedDefault : CheckBox.UnCheckedColor.ToAndroid(),
						disabledColor,
					});
				

			if (Forms.IsLollipopOrNewer)
			{
				Control.ButtonTintList = list;
				Control.ButtonTintMode = mode;
			}
			else
			{
				CompoundButtonCompat.SetButtonTintList(Control, list);
				CompoundButtonCompat.SetButtonTintMode(Control, mode);
			}
				
		}


		// general state related
		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			((IElementController)CheckBox).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}
		// general state related



		IPlatformElementConfiguration<PlatformConfiguration.Android, CheckBox> OnThisPlatform()
		{
			if (_platformElementConfiguration == null)
				_platformElementConfiguration = CheckBox.OnThisPlatform();

			return _platformElementConfiguration;
		}

		public void SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = LabelFor;

			LabelFor = (int)(id ?? _defaultLabelFor);
		}
	}
}