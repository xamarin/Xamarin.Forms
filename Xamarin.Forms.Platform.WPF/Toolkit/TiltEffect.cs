using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Xamarin.Forms.Platform.WPF.Toolkit
{
	public class TiltEffect
	{
	    public static readonly DependencyProperty IsTiltEnabledProperty = DependencyProperty.RegisterAttached("IsTiltEnabled", typeof(bool), typeof(TiltEffect), new PropertyMetadata(default(bool)));

	    public static bool GetIsTiltEnabled(UIElement element)
	    {
	        return (bool)element.GetValue(IsTiltEnabledProperty);
	    }

	    public static void SetIsTiltEnabled(UIElement element, bool value)
	    {
	        element.SetValue(IsTiltEnabledProperty, value);
	    }
	}
}
