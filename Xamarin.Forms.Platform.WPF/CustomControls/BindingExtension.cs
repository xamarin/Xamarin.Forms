using System.Windows;
using System.Windows.Data;

namespace Xamarin.WPF.CustomControls
{
    static class BindingExtension
    {
        public static Binding SetSource(this Binding binding, object source)
        {
            binding.Source = source;
            return binding;
        }

        public static Binding SetMode(this Binding binding, BindingMode mode)
        {
            binding.Mode = mode;
            return binding;
        }

        public static FrameworkElement SetLiteBinding(this FrameworkElement frameworkElement, object source, string path, DependencyProperty dependencyProperty, BindingMode mode = BindingMode.Default)
        {
            var bindingSelectionStart = new Binding(path);
            bindingSelectionStart.Source = source;
            bindingSelectionStart.Mode = BindingMode.TwoWay;
            frameworkElement.SetBinding(dependencyProperty, bindingSelectionStart);
            return frameworkElement;
        }
    }
}