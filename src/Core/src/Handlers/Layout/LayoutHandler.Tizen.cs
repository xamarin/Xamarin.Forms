using System;
using ElmSharp;
using Tizen.UIExtensions.Common;

namespace Microsoft.Maui.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, LayoutCanvas>
	{
		bool _layoutUpdatedRegistered = false;

		public void RegisterOnLayoutUpdated()
		{
			if (!_layoutUpdatedRegistered)
			{
				TypedNativeView!.LayoutUpdated += OnLayoutUpdated;
				_layoutUpdatedRegistered = true;
			}
		}

		protected override LayoutCanvas CreateNativeView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a Canvas");
			}

			if (NativeParent == null)
			{
				throw new InvalidOperationException($"{nameof(NativeParent)} cannot be null");
			}

			var view = new LayoutCanvas(NativeParent)
			{
				CrossPlatformMeasure = VirtualView.Measure,
				CrossPlatformArrange = VirtualView.Arrange
			};
			view.Show();
			return view;
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			_ = TypedNativeView ?? throw new InvalidOperationException($"{nameof(TypedNativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiApp.Current?.Context ?? throw new InvalidOperationException($"The MauiApp.Current.Context can't be null.");

			TypedNativeView.CrossPlatformMeasure = VirtualView.Measure;
			TypedNativeView.CrossPlatformArrange = VirtualView.Arrange;

			foreach (var child in VirtualView.Children)
			{
				TypedNativeView.Children.Add(child.ToNative(MauiApp.Current.Context!, false));
				if (child.Handler is ITizenViewHandler thandler)
				{
					thandler?.SetParent(this);
				}
			}
		}

		public void Add(IView child)
		{
			_ = TypedNativeView ?? throw new InvalidOperationException($"{nameof(TypedNativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiApp.Current?.Context ?? throw new InvalidOperationException($"The MauiApp.Current.Context can't be null.");

			TypedNativeView.Children.Add(child.ToNative(MauiApp.Current.Context!, false));
		}

		public void Remove(IView child)
		{
			_ = TypedNativeView ?? throw new InvalidOperationException($"{nameof(TypedNativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			if (child?.Handler?.NativeView is EvasObject nativeView)
			{
				TypedNativeView.Children.Remove(nativeView);
				//TODO: need to check
				nativeView.Unrealize();
			}
		}

		protected void OnLayoutUpdated(object sender, LayoutEventArgs e)
		{
			if (VirtualView != null && View != null)
			{
				var nativeGeometry = View.Geometry.ToDP();
				if (nativeGeometry.Width > 0 && nativeGeometry.Height > 0 )
				{
					VirtualView.InvalidateMeasure();
					VirtualView.InvalidateArrange();
					VirtualView.Measure(nativeGeometry.Width, nativeGeometry.Height);
					VirtualView.Arrange(nativeGeometry);
				}
			}
		}
	}
}
