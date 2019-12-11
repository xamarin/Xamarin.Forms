using System;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(TypeName))]
	[ProvideCompiled("Xamarin.Forms.Build.Tasks.DataTemplateExtension")]
	public sealed class DataTemplateExtension : IMarkupExtension<DataTemplate>
	{
		public string TypeName { get; set; }

		public DataTemplate ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			if (string.IsNullOrEmpty(TypeName)) {
				throw new XFException(XFException.Ecode.TypeName, serviceProvider.GetLineInfo());
			}

			if (typeResolver.TryResolve(TypeName, out var type))
				return new DataTemplate(type);

			throw new XFException(XFException.Ecode.ResolveType, serviceProvider.GetLineInfo(), TypeName);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<DataTemplate>).ProvideValue(serviceProvider);
		}
	}
}