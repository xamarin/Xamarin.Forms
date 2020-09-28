using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Platform.Handlers
{
	public partial class LayoutHandler : AbstractViewHandler<ILayout, LayoutViewGroup>
	{
		protected override LayoutViewGroup CreateView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var viewGroup = new LayoutViewGroup(Context!)
			{
				CrossPlatformMeasure = VirtualView.Measure,
				CrossPlatformArrange = VirtualView.Arrange
			};

			return viewGroup;
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
				TypedNativeView.AddView(child.ToNative(Context!));
			}
		}

		public override void TearDown()
		{
			if (TypedNativeView != null)
			{
				TypedNativeView.CrossPlatformArrange = null;
				TypedNativeView.CrossPlatformMeasure = null;
				TypedNativeView.RemoveAllViews();
			}

			base.TearDown();
		}
	}
}
