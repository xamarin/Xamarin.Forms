using System;
using System.Globalization;

namespace Xamarin.Forms
{
	// Theoretically this class shouldn't be necessary given
	// Binding.RelativeSource and TemplateBindingExtension 
	// but it's here to avoid breaking changes.
	public sealed class TemplateBinding : Binding
	{
		public TemplateBinding()
		{
			RelativeSource = RelativeBindingSource.TemplatedParent;
		}

		public TemplateBinding(string path, BindingMode mode = BindingMode.Default, IValueConverter converter = null, object converterParameter = null, string stringFormat = null) : this()
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("path can not be an empty string", "path");

			AllowChaining = true;
			Path = path;
			Converter = converter;
			ConverterParameter = converterParameter;
			Mode = mode;
			StringFormat = stringFormat;
		}
	}	
}