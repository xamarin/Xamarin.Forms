using System;
using System.ComponentModel;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	internal class AutomationPropertiesProvider : IDisposable 
	{
		const string GetFromElement = "GetValueFromElement";
		string _defaultContentDescription;
		bool? _defaultFocusable;
		string _defaultHint;
		bool _disposed;

		IVisualElementRenderer _renderer;

		public AutomationPropertiesProvider(IVisualElementRenderer renderer)
		{
			_renderer = renderer;
			_renderer.ElementPropertyChanged += OnElementPropertyChanged;
			_renderer.ElementChanged += OnElementChanged;
		}

		global::Android.Views.View Control => _renderer?.View;

		VisualElement Element => _renderer?.Element;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (_renderer != null)
			{
				_renderer.ElementChanged -= OnElementChanged;
				_renderer.ElementPropertyChanged -= OnElementPropertyChanged;

				_renderer = null;
			}
		}

		void SetAutomationId(string id = GetFromElement)
		{
			if (Control == null)
			{
				return;
			}

			string value = id;
			if (value == GetFromElement)
			{
				value = Element.AutomationId;
			}

			if (!string.IsNullOrEmpty(value))
			{
				Control.ContentDescription = value;
			}
		}

		void SetContentDescription()
		{
			if (Control == null)
			{
				return;
			}

			if (SetHint())
			{
				return;
			}

			_defaultContentDescription = Control.SetContentDescription(Element, _defaultContentDescription);
		}

		void SetFocusable(bool? value = null)
		{
			if ( Control == null)
			{
				return;
			}

			_defaultFocusable = Control.SetFocusable(Element, _defaultFocusable);
		}

		bool SetHint()
		{
			if (Element == null || Control == null)
			{
				return false;
			}

			var textView = Control as TextView;
			if (textView == null)
			{
				return false;
			}

			// Let the specified Title/Placeholder take precedence, but don't set the ContentDescription (won't work anyway)
			if (((Element as Picker)?.Title ?? (Element as Entry)?.Placeholder) != null)
			{
				return true;
			}

			_defaultHint = textView.SetHint(Element, _defaultHint);

			return true;
		}

		void SetLabeledBy()
		{
			if (Control == null)
				return;

			Control.SetLabeledBy(Element);
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
			}

			SetHint();
			SetAutomationId();
			SetContentDescription();
			SetFocusable();
			SetLabeledBy();
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == AutomationProperties.HelpTextProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.NameProperty.PropertyName)
			{
				SetContentDescription();
			}
			else if (e.PropertyName == AutomationProperties.IsInAccessibleTreeProperty.PropertyName)
			{
				SetFocusable();
			}
			else if (e.PropertyName == AutomationProperties.LabeledByProperty.PropertyName)
			{
				SetLabeledBy();
			}
		}
	}
}