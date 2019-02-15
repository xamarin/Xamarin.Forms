using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using MaterialComponents;
using UIKit;
using Xamarin.Forms;
using MActivityIndicator = MaterialComponents.ActivityIndicator;


namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, MActivityIndicator>
	{
		SemanticColorScheme _defaultColorScheme;
		SemanticColorScheme _colorScheme;

		CAShapeLayer _backgroundLayer;
		CGPoint _center;

		public MaterialActivityIndicatorRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			_colorScheme?.Dispose();
			_colorScheme = CreateColorScheme();

			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					_defaultColorScheme = CreateColorScheme();

					SetNativeControl(CreateNativeControl());

					_backgroundLayer = new CAShapeLayer
					{
						LineWidth = Control.StrokeWidth,
						FillColor = UIColor.Clear.CGColor,
						Hidden = true
					};
					Control.Layer.InsertSublayer(_backgroundLayer, 0);
				}

				UpdateColor();
				UpdateIsRunning();

				ApplyTheme();
			}
		}

		protected virtual SemanticColorScheme CreateColorScheme()
		{
			return MaterialColors.Light.CreateColorScheme();
		}

		protected virtual void ApplyTheme()
		{
			ActivityIndicatorColorThemer.ApplySemanticColorScheme(_colorScheme, Control);
		}

		protected override MActivityIndicator CreateNativeControl()
		{
			return new MActivityIndicator
			{
				IndicatorMode = ActivityIndicatorMode.Indeterminate,
				StrokeWidth = 4,
				Radius = 24
			};
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (_center != Control.Center)
			{
				_center = Control.Center;
				_backgroundLayer.LineWidth = Control.StrokeWidth;
				var pathRadius = Control.Radius - Control.StrokeWidth / 2;
				_backgroundLayer.Path = UIBezierPath.FromArc(_center, pathRadius, 0, 360, true).CGPath;
			}
			SetBackgroundColor(Element.BackgroundColor);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var radius = NMath.Min(size.Width, size.Height) / 2;
			if (!nfloat.IsInfinity(radius))
			{
				Control.Radius = radius;
				Control.StrokeWidth = Control.Radius / 6;
			}
			return base.SizeThatFits(size);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			var updatedTheme = false;
			if (e.PropertyName == ActivityIndicator.ColorProperty.PropertyName)
			{
				UpdateColor();
				updatedTheme = true;
			}
			else if (e.PropertyName == ActivityIndicator.IsRunningProperty.PropertyName)
			{
				UpdateIsRunning();
			}

			if (updatedTheme)
				ApplyTheme();
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (_backgroundLayer == null)
				return;

			_backgroundLayer.Hidden = color.IsDefault;
			_backgroundLayer.StrokeColor = color.ToCGColor();
		}

		void UpdateColor()
		{
			_colorScheme.PrimaryColor = Element.Color.IsDefault ? _defaultColorScheme.PrimaryColor : Element.Color.ToUIColor();
		}

		void UpdateIsRunning()
		{
			bool isRunning = Element.IsRunning;

			if (Control.Animating == isRunning)
				return;

			if (isRunning)
				Control.StartAnimating();
			else
				Control.StopAnimating();
		}
	}
}