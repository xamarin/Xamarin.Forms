using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public sealed class DataTemplateSelectorList : DataTemplateSelector
	{
		public DataTemplateSelectorList()
		{
			DataTemplates = new Dictionary<string, DataTemplate>();
		}

		public DataTemplateSelectorList(Dictionary<string, DataTemplate> dataTemplates)
		{
			DataTemplates = dataTemplates ?? new Dictionary<string, DataTemplate>();
		}

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (DataTemplates.TryGetValue(item.GetType().Name, out DataTemplate template))
			{
				return template;
			}

			return new DataTemplate(item.GetType());
		}

		public Dictionary<string, DataTemplate> DataTemplates { get; }
	}
}