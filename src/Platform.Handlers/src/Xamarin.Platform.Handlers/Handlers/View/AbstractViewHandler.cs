#if __IOS__
using NativeView = UIKit.UIView;
#elif __MACOS__
using NativeView = AppKit.NSView;
#elif MONOANDROID
using NativeView = Android.Views.View;
#elif NETCOREAPP
using NativeView = System.Windows.FrameworkElement;
#elif NETSTANDARD
using NativeView = System.Object;
#endif

namespace Xamarin.Platform.Handlers
{
	public abstract partial class AbstractViewHandler<TVirtualView, TNativeView> : IViewHandler
		where TVirtualView : class, IView
#if !NETSTANDARD
		where TNativeView : NativeView
#endif
	{
		protected readonly PropertyMapper _defaultMapper;
		protected PropertyMapper? _mapper;
		bool _hasContainer;
		static bool HasSetDefaults;

		protected AbstractViewHandler(PropertyMapper mapper)
		{
			_defaultMapper = mapper;
		}

		protected abstract TNativeView CreateView();

		protected TNativeView TypedNativeView { get; private set; } = default!;

		protected TVirtualView? VirtualView { get; private set; } = default!;

		public NativeView? View => TypedNativeView;

		public object? NativeView => TypedNativeView;

		public virtual void SetView(IView view)
		{
			VirtualView = view as TVirtualView;
			TypedNativeView ??= CreateView();

			if (!HasSetDefaults)
			{
				SetupDefaults();
				HasSetDefaults = true;
			}

			_mapper = _defaultMapper;

			if (VirtualView is IPropertyMapperView imv)
			{
				var map = imv.GetPropertyMapperOverrides();
				var instancePropertyMapper = map as PropertyMapper<TVirtualView>;
				if (map != null && instancePropertyMapper == null)
				{
				}
				if (instancePropertyMapper != null)
				{
					instancePropertyMapper.Chained = _defaultMapper;
					_mapper = instancePropertyMapper;
				}
			}

			_mapper?.UpdateProperties(this, VirtualView);
		}

		public virtual void Remove(IView view)
		{
			VirtualView = null;
		}

		protected virtual void DisposeView(TNativeView nativeView)
		{

		}

		public virtual void UpdateValue(string property)
			=> _mapper?.UpdateProperty(this, VirtualView, property);

		protected virtual void SetupDefaults() { }

		public bool HasContainer
		{
			get => _hasContainer;
			set
			{
				if (_hasContainer == value)
					return;

				_hasContainer = value;

				if (value)
					SetupContainer();
				else
					RemoveContainer();
			}
		}
	}
}