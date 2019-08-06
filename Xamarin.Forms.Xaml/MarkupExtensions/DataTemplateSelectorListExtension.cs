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
				if (elementNode == null)
					continue;

				if(elementNode.Properties.TryGetValue(XmlName.xDataType, out var dataType))
				{
					string typeId;
					if (dataType is MarkupNode markupNode)
					{
						typeId = markupNode.MarkupString.Replace("{", "").Replace("}", "");
					}
					else
					{
						typeId = (dataType as ValueNode)?.Value as string ?? string.Empty;
					}
					if (typeResolver.TryResolve(typeId, out var type))
					{
						templates.DataTemplates[type] = item;
					}

					count++;
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