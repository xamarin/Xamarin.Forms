using System;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class TemplatedCell : BaseCell
	{
		public IVisualElementRenderer VisualElementRenderer { get; private set; }

		protected nfloat ConstrainedDimension;

		public void SetRenderer(IVisualElementRenderer renderer)
		{
			VisualElementRenderer = renderer;

			// TODO hartez 2018/09/07 16:00:46 Move this loop to its own method	
			for (int n = ContentView.Subviews.Length - 1; n >= 0; n--)
			{
				// TODO hartez 2018/09/07 16:14:43 Does this also need to clear the constraints?	
				ContentView.Subviews[n].RemoveFromSuperview();
			}

			var nativeView = VisualElementRenderer.NativeView;

			InitializeContentConstraints(nativeView);
		}

		public abstract CGSize Layout();

		[Export("initWithFrame:")]
		protected TemplatedCell(CGRect frame) : base(frame)
		{
		}
	}
}