using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public sealed class DataTemplateSelectorList : DataTemplateSelector
	{
		public DataTemplateSelectorList()
		{
			DataTemplates = new Dictionary<Type, DataTemplate>();
		}

		public DataTemplateSelectorList(Dictionary<Type, DataTemplate> dataTemplates)
		{
			DataTemplates = dataTemplates ?? new Dictionary<Type, DataTemplate>();
		}

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			throw new NotImplementedException();
		}

		public Dictionary<Type, DataTemplate> DataTemplates { get; }
	}
}