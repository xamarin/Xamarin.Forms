using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("Mode")]
	[AcceptEmptyServiceProvider]
	public sealed class RelativeSourceExtension : IMarkupExtension<RelativeBindingSource>
	{
		Type _ancestorType;
		int _ancestorLevel = 1;

		public RelativeBindingSourceMode Mode
		{
			get;
			set;
		}

		public int AncestorLevel
		{
			get => _ancestorLevel;
			set
			{
				_ancestorLevel = value;
				if (_ancestorLevel > 0)
					this.Mode = RelativeBindingSourceMode.FindAncestor;
			}
		}

		public Type AncestorType
		{
			get => _ancestorType;
			set
			{
				_ancestorType = value;
				if (_ancestorType != null)
					this.Mode = RelativeBindingSourceMode.FindAncestor;
			}
		}

		RelativeBindingSource IMarkupExtension<RelativeBindingSource>.ProvideValue(IServiceProvider serviceProvider)
		{
			switch (this.Mode)
			{
				case RelativeBindingSourceMode.Self:
					return RelativeBindingSource.Self;
				case RelativeBindingSourceMode.TemplatedParent:
					return RelativeBindingSource.TemplatedParent;
				case RelativeBindingSourceMode.FindAncestor:
					if (AncestorType == null)
						throw new Exception(
							$"{nameof(RelativeBindingSourceMode.FindAncestor)} " +
							$"binding must specify valid {nameof(AncestorType)}");
					return new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor)
					{
						AncestorType = AncestorType,
						AncestorLevel = AncestorLevel
					};
				default:
					throw new NotImplementedException();
			}
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<RelativeBindingSource>).ProvideValue(serviceProvider);
		}
	}
}