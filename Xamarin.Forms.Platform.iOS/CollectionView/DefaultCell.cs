using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public interface IConstrainedCell
	{
		void SetConstrainedDimension(nfloat constant);
	}

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

		protected abstract void Layout();

		[Export("initWithFrame:")]
		protected TemplatedCell(CGRect frame) : base(frame)
		{
		}
	}

	internal sealed class TemplatedVerticalListCell : TemplatedCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedVerticalListCell");

		[Export("initWithFrame:")]
		public TemplatedVerticalListCell(CGRect frame) : base(frame)
		{
		}

		public void SetConstrainedDimension(nfloat constant)
		{
			ConstrainedDimension = constant;
			Layout();
		}

		protected override void Layout()
		{
			var nativeView = VisualElementRenderer.NativeView;

			Debug.WriteLine($">>>>> TemplatedCell SetRenderer: ContentView.Frame.Width {ContentView.Frame.Width}");
			Debug.WriteLine($">>>>> TemplatedCell SetRenderer: ContentView.Frame.Height {ContentView.Frame.Height}");

			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension, 
				ContentView.Frame.Height, MeasureFlags.IncludeMargins);

			Debug.WriteLine($">>>>> TemplatedCell SetRenderer: sizeRequest is {measure}");

			var height = VisualElementRenderer.Element.Height > 0 
				? VisualElementRenderer.Element.Height : measure.Request.Height;

			nativeView.Frame = new CGRect(0, 0, ConstrainedDimension, height);

			Debug.WriteLine($">>>>> TemplatedCell SetRenderer 48: {nativeView.Frame}");

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			Debug.WriteLine($">>>>> TemplatedCell SetRenderer 52: {VisualElementRenderer.Element.Bounds}");
			Debug.WriteLine($">>>>> TemplatedCell SetRenderer 53: {ContentView.Frame}");
		}
	}

	internal sealed class TemplatedHorizontalListCell : TemplatedCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.TemplatedHorizontalListCell");

		[Export("initWithFrame:")]
		public TemplatedHorizontalListCell(CGRect frame) : base(frame)
		{
		}

		public void SetConstrainedDimension(nfloat constant)
		{
			ConstrainedDimension = constant;
			Layout();
		}

		protected override void Layout()
		{
			var nativeView = VisualElementRenderer.NativeView;

			var measure = VisualElementRenderer.Element.Measure(ContentView.Frame.Width, 
				ConstrainedDimension, MeasureFlags.IncludeMargins);

			var width = VisualElementRenderer.Element.Width > 0 
				? VisualElementRenderer.Element.Width : measure.Request.Width;

			nativeView.Frame = new CGRect(0, 0, width, ConstrainedDimension);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());
		}
	}

	public abstract class BaseCell : UICollectionViewCell
	{
		[Export("initWithFrame:")]
		protected BaseCell(CGRect frame) : base(frame)
		{
			ContentView.BackgroundColor = UIColor.Clear;
		}

		protected void InitializeContentConstraints(UIView nativeView)
		{
			ContentView.AddSubview(nativeView);
			ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.TopAnchor.ConstraintEqualTo(nativeView.TopAnchor).Active = true;
			ContentView.BottomAnchor.ConstraintEqualTo(nativeView.BottomAnchor).Active = true;
			ContentView.LeadingAnchor.ConstraintEqualTo(nativeView.LeadingAnchor).Active = true;
			ContentView.TrailingAnchor.ConstraintEqualTo(nativeView.TrailingAnchor).Active = true;
		}
	}

	public abstract class DefaultCell : BaseCell
	{
		public UILabel Label { get; }

		[Export("initWithFrame:")]
		protected DefaultCell(CGRect frame) : base(frame)
		{
			Label = new UILabel(frame)
			{
				TextColor = UIColor.Black,
				Lines = 1,
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			ContentView.BackgroundColor = UIColor.Clear;

			InitializeContentConstraints(Label);
		}
	}

	internal sealed class DefaultVerticalListCell : DefaultCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultVerticalListCell");

		[Export("initWithFrame:")]
		public DefaultVerticalListCell(CGRect frame) : base(frame)
		{
			Width = Label.WidthAnchor.ConstraintEqualTo(Frame.Width);
			Width.Active = true;
		}

		NSLayoutConstraint Width { get; }

		public void SetConstrainedDimension(nfloat constant)
		{
			Width.Constant = constant;
		}
	}

	internal sealed class DefaultHorizontalListCell : DefaultCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultHorizontalListCell");

		[Export("initWithFrame:")]
		public DefaultHorizontalListCell(CGRect frame) : base(frame)
		{
			Height = Label.HeightAnchor.ConstraintEqualTo(Frame.Height);
			Height.Active = true;
		}

		NSLayoutConstraint Height { get; }

		public void SetConstrainedDimension(nfloat constant)
		{
			Height.Constant = constant;
		}
	}
}