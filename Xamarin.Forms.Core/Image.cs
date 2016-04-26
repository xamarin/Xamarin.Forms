using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_ImageRenderer))]
	public class Image : View
	{
		public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(ImageSource), typeof(Image), default(ImageSource), propertyChanging: OnSourcePropertyChanging,
			propertyChanged: OnSourcePropertyChanged);

		public static readonly BindableProperty AspectProperty = BindableProperty.Create("Aspect", typeof(Aspect), typeof(Image), Aspect.AspectFit);

		public static readonly BindableProperty IsOpaqueProperty = BindableProperty.Create("IsOpaque", typeof(bool), typeof(Image), false);

		internal static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty.CreateReadOnly("IsLoading", typeof(bool), typeof(Image), default(bool));

		public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;

		public Aspect Aspect
		{
			get { return (Aspect)GetValue(AspectProperty); }
			set { SetValue(AspectProperty, value); }
		}

		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
		}

		public bool IsOpaque
		{
			get { return (bool)GetValue(IsOpaqueProperty); }
			set { SetValue(IsOpaqueProperty, value); }
		}

		[TypeConverter(typeof(ImageSourceConverter))]
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		protected override void OnBindingContextChanged()
		{
			if (Source != null)
				SetInheritedBindingContext(Source, BindingContext);

			base.OnBindingContextChanged();
		}

		[Obsolete("Use OnMeasure")]
		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			SizeRequest desiredSize = base.OnSizeRequest(double.PositiveInfinity, double.PositiveInfinity);

			double desiredWidth = desiredSize.Request.Width;
			double desiredHeight = desiredSize.Request.Height;

			if (desiredWidth == 0 || desiredHeight == 0)
				return new SizeRequest(new Size(0, 0));

			// Arbitrarily cap size at the screen size because infinity is just crazy.
			Size screenSize = Device.Info.PixelScreenSize;
			if (heightConstraint == double.PositiveInfinity)
				heightConstraint = Device.info.CurrentOrientation.IsPortrait()
				? screenSize.Height
				: screenSize.Width;

			if (widthConstraint == double.PositiveInfinity)
				widthConstraint = Device.info.CurrentOrientation.IsLandscape()
				? screenSize.Height
				: screenSize.Width;

			// Scale the image so it exactly fills the view. Scaling may not be uniform in X and Y.
			if (Aspect == Aspect.Fill)
				return new SizeRequest(new Size(widthConstraint, heightConstraint));

			double width = desiredWidth;
			double height = desiredHeight;

			double heightScale = heightConstraint / desiredHeight;
			double widthScale = widthConstraint / desiredWidth;

			switch (Aspect)
			{
				case Aspect.AspectFit:
					// Scale the image to fit the view. Some parts may be left empty (letter boxing).
					var fitScale = Math.Min(heightScale, widthScale);
					height = desiredHeight * fitScale;
					width = desiredWidth * fitScale;
					break;
				case Aspect.AspectFill:
					// Scale the image to fill the view. Some parts may be clipped in order to fill the view.
					var fillScale = Math.Max(heightScale, widthScale);
					height = desiredHeight * fillScale;
					width = desiredWidth * fillScale;
					break;
			}

			return new SizeRequest(new Size(width, height));
		}

		void OnSourceChanged(object sender, EventArgs eventArgs)
		{
			OnPropertyChanged(SourceProperty.PropertyName);
			InvalidateMeasure(InvalidationTrigger.MeasureChanged);
		}

		static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((Image)bindable).OnSourcePropertyChanged((ImageSource)oldvalue, (ImageSource)newvalue);
		}

		void OnSourcePropertyChanged(ImageSource oldvalue, ImageSource newvalue)
		{
			if (newvalue != null)
			{
				newvalue.SourceChanged += OnSourceChanged;
				SetInheritedBindingContext(newvalue, BindingContext);
			}
			InvalidateMeasure(InvalidationTrigger.MeasureChanged);
		}

		static void OnSourcePropertyChanging(BindableObject bindable, object oldvalue, object newvalue)
		{
			((Image)bindable).OnSourcePropertyChanging((ImageSource)oldvalue, (ImageSource)newvalue);
		}

		async void OnSourcePropertyChanging(ImageSource oldvalue, ImageSource newvalue)
		{
			if (oldvalue == null)
				return;

			oldvalue.SourceChanged -= OnSourceChanged;
			await oldvalue.Cancel();
		}
	}
}