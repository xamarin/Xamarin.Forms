using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("Mode")]
	[AcceptEmptyServiceProvider]
	public sealed class RelativeSourceExtension : IMarkupExtension<RelativeBindingSource>
	{
		public RelativeBindingSourceMode Mode
		{
			get;
			set;
		}

		public int AncestorLevel
		{
			get;
			set;
		}

		public Type AncestorType
		{
			get;
			set;
		}

		RelativeBindingSource IMarkupExtension<RelativeBindingSource>.ProvideValue(IServiceProvider serviceProvider)
		{
			if (AncestorType != null)
			{
				RelativeBindingSourceMode mode;

				if ( Mode != RelativeBindingSourceMode.FindAncestor && 
					Mode != RelativeBindingSourceMode.FindAncestorBindingContext )
				{
					// Note to documenters:

					// This permits "{Binding Source={RelativeSource AncestorType={x:Type MyType}}}" syntax
					// where Mode hasn't been explicitly set, consistent with WPF/UWP.

					// Also, we assume FindAncestor is meant if the ancestor type is a visual 
					// Element, otherwise assume FindAncestorBindingContext is intended. (The
					// mode can also be explicitly set in XAML)
					mode = typeof(Element).IsAssignableFrom(AncestorType)
						? RelativeBindingSourceMode.FindAncestor
						: RelativeBindingSourceMode.FindAncestorBindingContext;
				}
				else
				{
					mode = Mode;
				}

				return new RelativeBindingSource(mode, AncestorType, AncestorLevel);
			}
			else if (Mode == RelativeBindingSourceMode.Self)
			{
				return RelativeBindingSource.Self;
			}
			else if (Mode == RelativeBindingSourceMode.TemplatedParent)
			{
				return RelativeBindingSource.TemplatedParent;
			}
			else
			{
				throw new InvalidOperationException($"Invalid {nameof(Mode)}");
			}
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<RelativeBindingSource>).ProvideValue(serviceProvider);
		}
	}
}