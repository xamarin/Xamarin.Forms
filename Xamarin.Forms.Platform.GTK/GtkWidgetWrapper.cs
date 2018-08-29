using System;
using Gtk;
using Xamarin.Forms;

namespace Xamarin.Forms.Platform.GTK
{
	/// <summary>
	/// Convenience helper to wrap an existing Gtk+ <see cref="Widget"/> in a forms <see cref="View"/>.
	/// It can be used to reuse existing Gtk+ <see cref="Widget"/> and ease porting of Gtk+ applications.
	/// </summary>
	public class GtkWidgetWrapper : View
	{
		Widget nativeWidget;

		public static readonly BindableProperty WidgetTypeProperty = BindableProperty.Create(
			propertyName: "WidgetType", returnType: typeof(Type), declaringType: typeof(GtkWidgetWrapper), defaultValue: null,
			defaultBindingMode: BindingMode.OneWay);

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Xamarin.Forms.Platform.GTK.GtkWidgetWrapper"/> class.
		/// </summary>
		public GtkWidgetWrapper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Xamarin.Forms.Platform.GTK.GtkWidgetWrapper"/> class
		/// with an instance of the <see cref="Widget"/> to wrap.
		/// </summary>
		/// <param name="widget">Widget.</param>
		public GtkWidgetWrapper(Widget widget)
		{
			Widget = widget;
		}

		/// <summary>
		/// Gets the <see cref="Widget"/> wrapped.
		/// </summary>
		/// <value>The native widget.</value>
		public Widget Widget
		{
			get
			{
				return nativeWidget;
			}
			internal set
			{
				nativeWidget = value;
				OnPropertyChanged(nameof(Widget));
			}
		}

		/// <summary>
		/// Gets the type of the <see cref="Widget"/>.
		/// </summary>
		/// <value>The type of the widget.</value>
		public Type WidgetType
		{
			set
			{
				if (!typeof(Widget).IsAssignableFrom(value))
				{
					throw new InvalidCastException($"The input type {value} must inherit from {typeof(Widget)}");
				}
				SetValue(WidgetTypeProperty, value);
			}
			get
			{
				return (Type)GetValue(WidgetTypeProperty);
			}
		}
	}
}
