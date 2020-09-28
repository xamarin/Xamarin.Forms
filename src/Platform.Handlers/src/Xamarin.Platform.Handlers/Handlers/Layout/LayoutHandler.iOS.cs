using System;

namespace Xamarin.Platform.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, LayoutView>
	{
		protected override LayoutView CreateView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var view = new LayoutView
			{
				CrossPlatformMeasure = VirtualView.Measure,
				CrossPlatformArrange = VirtualView.Arrange,
			};

			return view;
		}

		public override void SetView(IView view)
		{
			base.SetView(view);

			_ = TypedNativeView ?? throw new InvalidOperationException($"{nameof(TypedNativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			TypedNativeView.CrossPlatformMeasure = VirtualView.Measure;
			TypedNativeView.CrossPlatformArrange = VirtualView.Arrange;

			foreach (var child in VirtualView.Children)
			{
				TypedNativeView.AddSubview(child.ToNative());
			}
		}

		public override void TearDown()
		{
			if (TypedNativeView != null)
			{
				TypedNativeView.CrossPlatformArrange = null;
				TypedNativeView.CrossPlatformMeasure = null;

				foreach (var subview in TypedNativeView.Subviews)
				{
					subview.RemoveFromSuperview();
				}
			}

			base.TearDown();
		}
	}
}
