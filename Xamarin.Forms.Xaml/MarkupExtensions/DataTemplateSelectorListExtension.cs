using System;
using System.Collections;
using System.Collections.Generic;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Items))]
	[AcceptEmptyServiceProvider]
	public class DataTemplateSelectorListExtension : IMarkupExtension<DataTemplateSelectorList>
	{
		public DataTemplateSelectorListExtension()
		{
			Items = new List<DataTemplate>();
		}

		public IList<DataTemplate> Items { get; }
		

		public DataTemplateSelectorList ProvideValue(IServiceProvider serviceProvider)
		{
			

			if (Items == null)
				return null;

			var array = new DataTemplateSelectorList();
	

			return array;
		}



		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<DataTemplateSelectorList>).ProvideValue(serviceProvider);
		}
	}
}