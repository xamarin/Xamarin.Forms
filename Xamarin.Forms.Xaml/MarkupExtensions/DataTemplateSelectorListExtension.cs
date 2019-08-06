using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(DataTemplates))]
	public class DataTemplateSelectorListExtension : IMarkupExtension<DataTemplateSelectorList>
	{
		public DataTemplateSelectorListExtension()
		{
			DataTemplates = new List<DataTemplate>();
		}

		public IList<DataTemplate> DataTemplates { get; }
		

		public DataTemplateSelectorList ProvideValue(IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));

			if (!(serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfo))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");

			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");



			if (DataTemplates == null)
				return null;

			var templates = new DataTemplateSelectorList();


			var node = lineInfo.XmlLineInfo as IListNode;

			if (node == null)
				return null;
			
			

			var count = 0;
			foreach (DataTemplate item in DataTemplates)
			{
				var elementNode = node.CollectionItems[count] as IElementNode;
				//if(elementNode == null)

				var dataType = elementNode.Properties.FirstOrDefault(f => f.Key == XmlName.xDataType);
				//if(dataType == null)

				//dataType.Key.
				if (typeResolver.TryResolve((dataType.Value as MarkupNode)?.MarkupString.Replace("{","").Replace("}",""), out var type))
				{
					templates.DataTemplates[type] = item;
				}


				
			}

			return templates;
		}



		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<DataTemplateSelectorList>).ProvideValue(serviceProvider);
		}
	}
}