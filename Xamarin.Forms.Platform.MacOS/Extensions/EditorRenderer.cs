using System;
using System.ComponentModel;
using System.Drawing;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class EditorRenderer : ViewRenderer<Editor, NSTextField>
	{
		IElementController ElementController => Element as IElementController;

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				SetNativeControl(new NSTextField());

				Control.Changed += HandleChanged;
				Control.Activated += OnActivated;
			}

			if (e.NewElement != null)
			{
				UpdateText();
				UpdateFont();
				UpdateTextColor();
				UpdateEditable();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Editor.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateEditable();
			else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
				UpdateFont();
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (Control == null)
				return;
			if (color == Color.Default)
				Control.BackgroundColor = NSColor.Clear;
			else
				Control.BackgroundColor = color.ToNSColor();

			base.SetBackgroundColor(color)
		}

		void HandleChanged(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(Editor.TextProperty, Control.StringValue);
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			Element.SetValue(VisualElement.IsFocusedPropertyKey, false);
			Element.SendCompleted();
		}

		void OnActivated(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void UpdateEditable()
		{
			Control.Editable = Element.IsEnabled;
		}

		void UpdateFont()
		{
			Control.Font = Element.ToNSFont();
		}

		void UpdateText()
		{
			if (Control.StringValue != Element.Text)
				Control.StringValue = Element.Text;
		}

		void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault)
				Control.TextColor = NSColor.Black;
			else
				Control.TextColor = textColor.ToNSColor();
		}
	}
}

