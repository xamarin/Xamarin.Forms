using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF
{
    public static class LayoutHelper
    {
        public static T GetChild<T>(this DependencyObject frameworkElement) where T: DependencyObject
        {
            var root = frameworkElement as T;
            if (root != null) return root;

            int n = VisualTreeHelper.GetChildrenCount(frameworkElement);
            for (var i = 0; i < n; i++)
            {
                DependencyObject element = VisualTreeHelper.GetChild(frameworkElement, i);
                if (element is T)
                {
                    return element as T;
                }
            }

            for (var i = 0; i < n; i++)
            {
                DependencyObject element = VisualTreeHelper.GetChild(frameworkElement, i);
                var result = GetChild<T>(element);
                if (result != null) return result;
            }

            return null;
        }
    }
}
