#if __ANDROID_28__
using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;
using Android.Support.V4.View;
using Xamarin.Forms.Platform.Android.Material;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.FastRenderers;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Text;
using Android.Text.Method;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using MTextInputLayout = Android.Support.Design.Widget.TextInputLayout;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Runtime;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(MaterialEntryRenderer), new[] { typeof(VisualRendererMarker.Material) })]
namespace Xamarin.Forms.Platform.Android.Material
{
	public sealed class MaterialEntryRenderer : EntryRendererBase<MaterialFormsTextInputLayout>
	{
		bool _disposed;
		private MaterialFormsEditText _textInputEditText;
		private MaterialFormsTextInputLayout _textInputLayout;

		public MaterialEntryRenderer(Context context) :
			base(MaterialContextThemeWrapper.Create(context))
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		IElementController ElementController => Element as IElementController;

		protected override EditText EditText => _textInputEditText;

		protected override MaterialFormsTextInputLayout CreateNativeControl()
		{
			LayoutInflater inflater = LayoutInflater.FromContext(Context);
			var id = ResourceManager.GetLayoutByName("TextInputLayoutFilledBox");
			var view = inflater.Inflate(id, null);
			_textInputLayout = (MaterialFormsTextInputLayout)view;
			_textInputEditText = _textInputLayout.FindViewById<MaterialFormsEditText>(Resource.Id.materialformsedittext);
			_textInputEditText.FocusChange += _textInputEditText_FocusChange;
			_textInputLayout.Hint = Element.Placeholder;

			return _textInputLayout;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			var oldElement = e.OldElement;

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
				oldElement.FocusChangeRequested -= OnFocusChangeRequested;
			}

			if (e.NewElement != null)
				Element.FocusChangeRequested += OnFocusChangeRequested;

			UpdateBackgroundColor();
		}


		internal override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			e.Result = true;

			if (e.Focus)
			{
				// use post being BeginInvokeOnMainThread will not delay on android
				Looper looper = Context.MainLooper;
				var handler = new Handler(looper);
				handler.Post(() =>
				{
					_textInputEditText.RequestFocus();
				});
			}
			else
			{
				_textInputEditText.ClearFocus();
			}

			if (e.Focus)
				this.ShowKeyboard();
			else
				this.HideKeyboard();
		}


		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing && Element != null)
				Element.FocusChangeRequested -= OnFocusChangeRequested;

			base.Dispose(disposing);
		}


		void _textInputEditText_FocusChange(object sender, FocusChangeEventArgs e)
		{
			// TODO figure out better way to do this
			// this is a hack that changes the active underline color from the accent color to whatever the user 
			// specified
			Device.BeginInvokeOnMainThread(() => UpdatePlaceholderColor(false));
		}

		void UpdatePlaceholderColor(bool reset)
		{
			if (reset && Element.PlaceholderColor != Color.Default)
				_textInputLayout.DefaultHintTextColor = MaterialColors.CreateTextInputFilledPlaceholderColors(Element.PlaceholderColor.ToAndroid());


			UpdateColor();
		}

		protected internal override void UpdateColor()
		{
			base.UpdateColor();
			
			
			// Todo move this to material colors
			if (Element.TextColor != Color.Default)
			{
				int[][] States =
				{
					new []{ global::Android.Resource.Attribute.StateFocused  },
					new []{ -global::Android.Resource.Attribute.StateFocused  },
				};

				var placeHolderColor = new ColorStateList(
							States,
							new int[]{
								Element.TextColor.ToAndroid(),
								new Color(Element.TextColor.R,Element.TextColor.G,Element.TextColor.B, 0.6).ToAndroid()
							}
					);

				ViewCompat.SetBackgroundTintList(_textInputEditText, placeHolderColor);
			}
		}

		protected override void UpdateBackgroundColor()
		{
			if (_textInputLayout == null)
				return;

			var backgroundColor = MaterialColors.Light.PrimaryColorVariant;

			if (Element.BackgroundColor == Color.Default)
			{
				if (Element.TextColor != Color.Default)
					_textInputLayout.BoxBackgroundColor = MaterialColors.CreateTextInputFilledInputBackgroundColor(Element.TextColor.ToAndroid());
				else
					_textInputLayout.BoxBackgroundColor = MaterialColors.CreateTextInputFilledInputBackgroundColor(MaterialColors.Light.PrimaryColorVariant);
			}
			else
			{
				_textInputLayout.BoxBackgroundColor = Element.BackgroundColor.ToAndroid();
			}
		}

		protected internal override void UpdatePlaceHolderText()
		{
			_textInputLayout.Hint = Element.Placeholder;
		}

		protected internal override void UpdatePlaceholderColor()
		{
			UpdatePlaceholderColor(true);
		}

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			_textInputLayout.Typeface = Element.ToTypeface();
			_textInputEditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}
	}
}
#endif