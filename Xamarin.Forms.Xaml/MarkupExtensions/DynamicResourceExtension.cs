using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(Key))]
	public sealed class DynamicResourceExtension : IMarkupExtension<DynamicResource>
	{
		public string Key { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider) => ((IMarkupExtension<DynamicResource>)this).ProvideValue(serviceProvider);

		DynamicResource IMarkupExtension<DynamicResource>.ProvideValue(IServiceProvider serviceProvider)
		{
			if (Key == null)
				throw new XFException(XFException.Ecode.SomethingNotFound, serviceProvider.GetLineInfo(), nameof(Key), nameof(DynamicResource));
			return new DynamicResource(Key);
		}
	}
}