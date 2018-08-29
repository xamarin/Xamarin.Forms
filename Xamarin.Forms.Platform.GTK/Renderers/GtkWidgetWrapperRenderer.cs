using System;
using Gtk;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Renderers;

[assembly: ExportRenderer(typeof(GtkWidgetWrapper), typeof(GtkWidgetWrapperRenderer))]
namespace Xamarin.Forms.Platform.GTK.Renderers
{
	public class GtkWidgetWrapperRenderer : ViewRenderer<GtkWidgetWrapper, Widget>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<GtkWidgetWrapper> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					if (e.NewElement.Widget == null)
					{
						if (e.NewElement.WidgetType != null)
						{
							var widget = (Widget)Activator.CreateInstance(e.NewElement.WidgetType);
							e.NewElement.Widget = widget;
						}
						else
						{
							throw new InvalidOperationException("No widget instance or widget type was defined to create the Control");
						}
					}
					SetNativeControl(e.NewElement.Widget);
				}
			}
			base.OnElementChanged(e);
		}

		protected override void SetNativeControl(Widget view)
		{
			if (view.Parent != null)
			{
				view.Unparent();
			}
			base.SetNativeControl(view);
		}
	}
}
