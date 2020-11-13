using System;
using Xamarin.Platform.Tizen;

namespace Xamarin.Platform.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, Canvas>
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

		protected override Canvas CreateNativeView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a Canvas");
			}

			if (NativeParent == null)
			{
				throw new InvalidOperationException($"{nameof(NativeParent)} cannot be null");
			}

			var view = new Canvas(NativeParent)
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

			TypedNativeView.CrossPlatformMeasure = VirtualView.Measure;
			TypedNativeView.CrossPlatformArrange = VirtualView.Arrange;

			foreach (var child in VirtualView.Children)
			{
				TypedNativeView.Children.Add(child.ToNative(Context!, false));
				if (child.Handler is ITizenViewHandler thandler)
				{
					thandler?.SetParent(this);
				}
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
