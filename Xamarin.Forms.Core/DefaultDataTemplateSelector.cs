using System;

namespace Xamarin.Forms
{
	public class DefaultDataTemplateSelector : DataTemplateSelector
	{
		readonly Func<object, BindableObject, DataTemplate> _func;

		public DefaultDataTemplateSelector(Func<object, BindableObject, DataTemplate> func) => _func = func;

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => _func(item, container);
	}
}