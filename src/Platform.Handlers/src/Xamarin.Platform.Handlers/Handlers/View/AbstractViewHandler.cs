using System;
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
#else
		where TNativeView : class
#endif
	{
		protected readonly PropertyMapper _defaultMapper;
		protected PropertyMapper _mapper;
		bool _hasContainer;
		static bool HasSetDefaults;

		protected AbstractViewHandler(PropertyMapper mapper)
		{
			_ = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_defaultMapper = mapper;
			_mapper = _defaultMapper;
		}

		protected abstract TNativeView CreateNativeView();

		protected TNativeView? TypedNativeView { get; private set; }

		protected TVirtualView? VirtualView { get; private set; }

		public NativeView? View => TypedNativeView;

		public object? NativeView => TypedNativeView;


		public void SetView(IView view)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));

			if (VirtualView != null)
			{
				TearDownOldVirtualView(VirtualView);
				_mapper.UpdateProperty(this, VirtualView, nameof(TearDownOldVirtualView));
			}

			VirtualView = view as TVirtualView;
			TypedNativeView ??= CreateNativeView();

			if (!HasSetDefaults)
			{
				if (TypedNativeView != null)
				{
					SetupDefaults(TypedNativeView);
					_mapper.UpdateProperty(this, VirtualView, nameof(SetupDefaults));
				}

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

			if (VirtualView != null)
			{
				SetUpNewVirtualView(VirtualView);
				_mapper.UpdateProperty(this, VirtualView, nameof(SetUpNewVirtualView));
			}

			_mapper.UpdateProperties(this, VirtualView);
		}


		protected virtual void SetUpNewVirtualView(TVirtualView newView)
		{
		}

		protected virtual void TearDownOldVirtualView(TVirtualView oldView)
		{
		}


		void IViewHandler.TearDown()
		{
			if (TypedNativeView != null)
			{
				TearDownNativeView(TypedNativeView);
				_mapper.UpdateProperty(this, VirtualView, nameof(TypedNativeView));
			}
		}

		public virtual void TearDownNativeView(TNativeView nativeView)
		{
			
		}

		public virtual void UpdateValue(string property)
			=> _mapper?.UpdateProperty(this, VirtualView, property);

		protected virtual void SetupDefaults(TNativeView nativeView) { }

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