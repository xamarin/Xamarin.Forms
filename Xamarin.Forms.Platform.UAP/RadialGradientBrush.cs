// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// UWP Replacement for WPF RadialGradientBrush: https://msdn.microsoft.com/en-us/library/system.windows.media.radialgradientbrush(v=vs.110).aspx.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Graphics.DirectX;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WColor = Windows.UI.Color;
using WGradientStopCollection = Windows.UI.Xaml.Media.GradientStopCollection;
using WGradientStop = Windows.UI.Xaml.Media.GradientStop;
using WPoint = Windows.Foundation.Point;

namespace Xamarin.Forms.Platform.UWP
{
	/// <summary>
	/// Helper Brush class to interop with Win2D Canvas calls.
	/// </summary>
	public abstract class CanvasBrushBase : XamlCompositionBrushBase
	{
		private CompositionSurfaceBrush _surfaceBrush;

		/// <summary>
		/// Gets or sets the internal surface render width.  Modify during construction.
		/// </summary>
		protected float SurfaceWidth { get; set; }

		/// <summary>
		/// Gets or sets the internal surface render height.  Modify during construction.
		/// </summary>
		protected float SurfaceHeight { get; set; }

		private CanvasDevice _device;

		private CompositionGraphicsDevice _graphics;

		/// <summary>
		/// Implemented by parent class and called when canvas is being constructed for brush.
		/// </summary>
		/// <param name="device">Canvas device.</param>
		/// <param name="session">Canvas drawing session.</param>
		/// <param name="size">Size of surface to draw on.</param>
		/// <returns>True if drawing was completed and the brush is ready, otherwise return False to not create brush yet.</returns>
		protected abstract bool OnDraw(CanvasDevice device, CanvasDrawingSession session, Vector2 size);

		/// <summary>
		/// Initializes the Composition Brush.
		/// </summary>
		protected override void OnConnected()
		{
			base.OnConnected();

			if (_device != null)
			{
				_device.DeviceLost -= CanvasDevice_DeviceLost;
			}

			_device = CanvasDevice.GetSharedDevice();
			_device.DeviceLost += CanvasDevice_DeviceLost;

			if (_graphics != null)
			{
				_graphics.RenderingDeviceReplaced -= CanvasDevice_RenderingDeviceReplaced;
			}

			_graphics = CanvasComposition.CreateCompositionGraphicsDevice(Window.Current.Compositor, _device);
			_graphics.RenderingDeviceReplaced += CanvasDevice_RenderingDeviceReplaced;

			// Delay creating composition resources until they're required.
			if (CompositionBrush == null)
			{
				// Abort if effects aren't supported.
				if (!CompositionCapabilities.GetForCurrentView().AreEffectsSupported())
				{
					return;
				}

				var size = new Vector2(SurfaceWidth, SurfaceHeight);
				var surface = _graphics.CreateDrawingSurface(size.ToSize(), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

				using (var session = CanvasComposition.CreateDrawingSession(surface))
				{
					// Call Implementor to draw on session.
					if (!OnDraw(_device, session, size))
					{
						return;
					}
				}

				_surfaceBrush = Window.Current.Compositor.CreateSurfaceBrush(surface);
				_surfaceBrush.Stretch = CompositionStretch.Fill;

				CompositionBrush = _surfaceBrush;
			}
		}

		private void CanvasDevice_RenderingDeviceReplaced(CompositionGraphicsDevice sender, object args)
		{
			OnDisconnected();
			OnConnected();
		}

		private void CanvasDevice_DeviceLost(CanvasDevice sender, object args)
		{
			OnDisconnected();
			OnConnected();
		}

		/// <summary>
		/// Deconstructs the Composition Brush.
		/// </summary>
		protected override void OnDisconnected()
		{
			base.OnDisconnected();

			if (_device != null)
			{
				_device.DeviceLost -= CanvasDevice_DeviceLost;
				_device = null;
			}

			if (_graphics != null)
			{
				_graphics.RenderingDeviceReplaced -= CanvasDevice_RenderingDeviceReplaced;
				_graphics = null;
			}

			// Dispose of composition resources when no longer in use.
			if (CompositionBrush != null)
			{
				CompositionBrush.Dispose();
				CompositionBrush = null;
			}

			if (_surfaceBrush != null)
			{
				_surfaceBrush.Dispose();
				_surfaceBrush = null;
			}
		}
	}

	/// <summary>
	/// Specifies the way in which an alpha channel affects color channels.
	/// </summary>
	public enum AlphaMode
	{
		/// <summary>
		/// Provides better transparent effects without a white bloom.
		/// </summary>
		Premultiplied = 0,

		/// <summary>
		/// WPF default handling of alpha channel during transparent blending.
		/// </summary>
		Straight = 1,
	}

	/// <summary>
	/// RadialGradientBrush - This GradientBrush defines its Gradient as an interpolation
	/// within an Ellipse.
	/// </summary>
	public class RadialGradientBrush : CanvasBrushBase
	{
		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var brush = (RadialGradientBrush)d;

			// We need to recreate the brush on any property change.
			brush.OnDisconnected();
			brush.OnConnected();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadialGradientBrush"/> class.
		/// </summary>
		public RadialGradientBrush()
		{
			// Rendering surface size, if this is too small the gradient will be pixelated.
			// Larger targets aren't effected as one would expect unless the gradient is very complex.
			// This seems like a good compromise.
			SurfaceWidth = 512;
			SurfaceHeight = 512;

			GradientStops = new WGradientStopCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadialGradientBrush"/> class
		/// with with two colors specified for GradientStops at
		/// offsets 0.0 and 1.0.
		/// </summary>
		/// <param name="startColor"> The Color at offset 0.0. </param>
		/// <param name="endColor"> The Color at offset 1.0. </param>
		public RadialGradientBrush(WColor startColor, WColor endColor)
			: this()
		{
			GradientStops.Add(new WGradientStop() { Color = startColor, Offset = 0.0 });
			GradientStops.Add(new WGradientStop() { Color = endColor, Offset = 1.0 });
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadialGradientBrush"/> class with GradientStops set to the passed-in collection.
		/// </summary>
		/// <param name="gradientStopCollection"> GradientStopCollection to set on this brush. </param>
		public RadialGradientBrush(WGradientStopCollection gradientStopCollection)
			: this()
		{
			GradientStops = gradientStopCollection;
		}

		/// <summary>
		/// Gets or sets a <see cref="AlphaMode"/> enumeration that specifies the way in which an alpha channel affects color channels.  The default is <see cref="AlphaMode.Straight"/> for compatibility with WPF.
		/// </summary>
		public AlphaMode AlphaMode
		{
			get { return (AlphaMode)GetValue(AlphaModeProperty); }
			set { SetValue(AlphaModeProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="AlphaMode"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty AlphaModeProperty =
			DependencyProperty.Register(nameof(AlphaMode), typeof(AlphaMode), typeof(RadialGradientBrush), new PropertyMetadata(AlphaMode.Straight, new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets a <see cref="ColorInterpolationMode"/> enumeration that specifies how the gradient's colors are interpolated.  The default is <see cref="ColorInterpolationMode.SRgbLinearInterpolation"/>.
		/// </summary>
		public ColorInterpolationMode ColorInterpolationMode
		{
			get { return (ColorInterpolationMode)GetValue(ColorInterpolationModeProperty); }
			set { SetValue(ColorInterpolationModeProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ColorInterpolationMode"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ColorInterpolationModeProperty =
			DependencyProperty.Register(nameof(ColorInterpolationMode), typeof(ColorInterpolationMode), typeof(RadialGradientBrush), new PropertyMetadata(ColorInterpolationMode.SRgbLinearInterpolation, new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the brush's gradient stops.
		/// </summary>
		public WGradientStopCollection GradientStops
		{
			get { return (WGradientStopCollection)GetValue(GradientStopsProperty); }
			set { SetValue(GradientStopsProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GradientStops"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GradientStopsProperty =
			DependencyProperty.Register(nameof(GradientStops), typeof(WGradientStopCollection), typeof(RadialGradientBrush), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the center of the outermost circle of the radial gradient.  The default is 0.5,0.5.
		/// </summary>
		public WPoint Center
		{
			get { return (WPoint)GetValue(CenterProperty); }
			set { SetValue(CenterProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Center"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CenterProperty =
			DependencyProperty.Register(nameof(Center), typeof(WPoint), typeof(RadialGradientBrush), new PropertyMetadata(new WPoint(0.5, 0.5), new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the location of the two-dimensional focal point that defines the beginning of the gradient.  The default is 0.5,0.5.
		/// </summary>
		public WPoint GradientOrigin
		{
			get { return (WPoint)GetValue(GradientOriginProperty); }
			set { SetValue(GradientOriginProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="GradientOrigin"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty GradientOriginProperty =
			DependencyProperty.Register(nameof(GradientOrigin), typeof(WPoint), typeof(RadialGradientBrush), new PropertyMetadata(new WPoint(0.5, 0.5), new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the horizontal radius of the outermost circle of the radial gradient.  The default is 0.5.
		/// </summary>
		public double RadiusX
		{
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RadiusX"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RadiusXProperty =
			DependencyProperty.Register(nameof(RadiusX), typeof(double), typeof(RadialGradientBrush), new PropertyMetadata(0.5, new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the vertical radius of the outermost circle of the radial gradient.  The default is 0.5.
		/// </summary>
		public double RadiusY
		{
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RadiusX"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RadiusYProperty =
			DependencyProperty.Register(nameof(RadiusY), typeof(double), typeof(RadialGradientBrush), new PropertyMetadata(0.5, new PropertyChangedCallback(OnPropertyChanged)));

		/// <summary>
		/// Gets or sets the type of spread method that specifies how to draw a gradient that starts or ends inside the bounds of the object to be painted.
		/// </summary>
		public GradientSpreadMethod SpreadMethod
		{
			get { return (GradientSpreadMethod)GetValue(SpreadMethodProperty); }
			set { SetValue(SpreadMethodProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="SpreadMethod"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty SpreadMethodProperty =
			DependencyProperty.Register(nameof(SpreadMethod), typeof(GradientSpreadMethod), typeof(RadialGradientBrush), new PropertyMetadata(GradientSpreadMethod.Pad, new PropertyChangedCallback(OnPropertyChanged)));

		/// <inheritdoc/>
		protected override bool OnDraw(CanvasDevice device, CanvasDrawingSession session, Vector2 size)
		{
			// Create our Brush
			if (GradientStops != null && GradientStops.Count > 0)
			{
				var gradientBrush = new CanvasRadialGradientBrush(
										device,
										GradientStops.ToWin2DGradientStops(),
										SpreadMethod.ToEdgeBehavior(),
										(CanvasAlphaMode)(int)AlphaMode,
										ColorInterpolationMode.ToCanvasColorSpace(),
										CanvasColorSpace.Srgb,
										CanvasBufferPrecision.Precision8UIntNormalized)
				{
					// Calculate Surface coordinates from 0.0-1.0 range given in WPF brush
					RadiusX = size.X * (float)RadiusX,
					RadiusY = size.Y * (float)RadiusY,
					Center = size * Center.ToVector2(),

					// Calculate Win2D Offset from origin/center used in WPF brush
					OriginOffset = size * (GradientOrigin.ToVector2() - Center.ToVector2()),
				};

				// Use brush to draw on our canvas
				session.FillRectangle(size.ToRect(), gradientBrush);

				gradientBrush.Dispose();

				return true;
			}

			return false;
		}
	}
}