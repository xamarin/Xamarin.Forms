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
			if (DataTemplates.TryGetValue(item.GetType(), out DataTemplate template))
			{
				return template;
			}

			return new DataTemplate(item.GetType());
		}

		public Dictionary<Type, DataTemplate> DataTemplates { get; }
	}
}