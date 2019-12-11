using System;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(TypeName))]
	[ProvideCompiled("Xamarin.Forms.Build.Tasks.TypeExtension")]
	public class TypeExtension : IMarkupExtension<Type>
	{
		public string TypeName { get; set; }

		public Type ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException(nameof(serviceProvider));
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				throw new ArgumentException("No IXamlTypeResolver in IServiceProvider");
			if (string.IsNullOrEmpty(TypeName))
				throw new XFException(XFException.Ecode.TypeName, serviceProvider.GetLineInfo());

			return typeResolver.Resolve(TypeName, serviceProvider);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<Type>).ProvideValue(serviceProvider);
		}
	}
}