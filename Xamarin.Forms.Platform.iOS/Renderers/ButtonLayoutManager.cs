using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Specifics = Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
{
	public interface IButtonLayoutRenderer
	{
		UIButton Control { get; }
		Button Element { get; }
		IImageVisualElementRenderer ImageVisualElementRenderer { get; }
		nfloat MinimumHeight { get; }
		event EventHandler<ElementChangedEventArgs<Button>> ElementChanged;
	}

	public class ButtonLayoutManager : IDisposable
	{
		bool _disposed;
		IButtonLayoutRenderer _renderer;
		Button _element;
		bool _spacingAdjustsHorizontalPadding;
		bool _spacingAdjustsVerticalPadding;
		bool _collapseHorizontalPadding;
		bool _borderAdjustsPadding;

		bool _titleChanged;
		CGSize _titleSize;
		UIEdgeInsets _paddingDelta = new UIEdgeInsets();

		public ButtonLayoutManager(IButtonLayoutRenderer renderer,
			bool spacingAdjustsHorizontalPadding = true,
			bool spacingAdjustsVerticalPadding = true,
			bool collapseHorizontalPadding = false,
			bool borderAdjustsPadding = false)
		{
			_renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
			_renderer.ElementChanged += OnElementChanged;
			_spacingAdjustsHorizontalPadding = spacingAdjustsHorizontalPadding;
			_spacingAdjustsVerticalPadding = spacingAdjustsVerticalPadding;
			_collapseHorizontalPadding = collapseHorizontalPadding;
			_borderAdjustsPadding = borderAdjustsPadding;

			ImageElementManager.Init(renderer.ImageVisualElementRenderer);
		}

		UIButton Control => _renderer?.Control;

		IImageVisualElementRenderer ImageVisualElementRenderer => _renderer?.ImageVisualElementRenderer;

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_renderer != null)
					{
						var imageRenderer = ImageVisualElementRenderer;
						if (imageRenderer != null)
							ImageElementManager.Dispose(imageRenderer);

						_renderer.ElementChanged -= OnElementChanged;
						_renderer = null;
					}
				}
				_disposed = true;
			}
		}

		internal void SizeThatFits(ref CGSize result)
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var minHeight = _renderer.MinimumHeight;
			if (result.Height < minHeight)
				result.Height = minHeight;

			if (_borderAdjustsPadding && _element is IBorderElement borderElement && borderElement.IsBorderWidthSet() && borderElement.BorderWidth != borderElement.BorderWidthDefaultValue)
			{
				var adjustment = (nfloat)(_element.BorderWidth * 2.0);
				result.Width += adjustment;
				result.Height += adjustment;
			}
		}

		public void Update()
		{
			UpdatePadding();
			UpdateImage();
			UpdateText();
		}

		public void SetImage(UIImage image)
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			if (image != null)
			{
				control.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
				control.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				ComputeEdgeInsets();
			}
			else
			{
				control.SetImage(null, UIControlState.Normal);
				ClearEdgeInsets();
			}
		}

		void OnElementChanged(object sender, ElementChangedEventArgs<Button> e)
		{
			if (_element != null)
			{
				_element.PropertyChanged -= OnElementPropertyChanged;
				_element = null;
			}

			if (e.NewElement is Button button)
			{
				_element = button;
				_element.PropertyChanged += OnElementPropertyChanged;
			}

			Update();
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			if (e.PropertyName == Button.PaddingProperty.PropertyName)
				UpdatePadding();
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
				UpdateImage();
			else if (e.PropertyName == Button.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.ContentLayoutProperty.PropertyName)
				ComputeEdgeInsets();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName && _borderAdjustsPadding)
				_element.InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
		}

		void UpdateText()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			_titleChanged = true;
			control.SetTitle(_element.Text, UIControlState.Normal);
			ComputeEdgeInsets();
		}

		async void UpdateImage()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var imageRenderer = ImageVisualElementRenderer;
			if (imageRenderer == null)
				return;

			try
			{
				await ImageElementManager.SetImage(imageRenderer, _element);
			}
			catch (Exception ex)
			{
				Internals.Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
		}

		void UpdatePadding()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			control.ContentEdgeInsets = new UIEdgeInsets(
				(float)(_element.Padding.Top + _paddingDelta.Top),
				(float)(_element.Padding.Left + _paddingDelta.Left),
				(float)(_element.Padding.Bottom + _paddingDelta.Bottom),
				(float)(_element.Padding.Right + _paddingDelta.Right)
			);
		}

		void UpdateContentEdge(UIEdgeInsets? delta = null)
		{
			_paddingDelta = delta ?? new UIEdgeInsets();
			UpdatePadding();
		}

		void ClearEdgeInsets()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			control.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			control.TitleEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			UpdateContentEdge();
		}

		void ComputeEdgeInsets()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			var control = Control;
			if (control == null)
				return;

			if (control.ImageView?.Image == null || string.IsNullOrEmpty(control.TitleLabel?.Text))
				return;

			var layout = _element.ContentLayout;
			var position = layout.Position;
			var spacing = (nfloat)(layout.Spacing / 2);

			// left and right

			var horizontalPadding = _spacingAdjustsHorizontalPadding ? spacing * 2 : spacing;

			if (position == Button.ButtonContentLayout.ImagePosition.Left)
			{
				control.ImageEdgeInsets = new UIEdgeInsets(0, -spacing, 0, spacing);
				control.TitleEdgeInsets = new UIEdgeInsets(0, spacing, 0, -spacing);
				UpdateContentEdge(new UIEdgeInsets(0, horizontalPadding, 0, horizontalPadding));
				return;
			}

			if (_titleChanged)
			{
				var stringToMeasure = new NSString(_element.Text);
				UIStringAttributes attribs = new UIStringAttributes { Font = control.TitleLabel.Font };
				_titleSize = stringToMeasure.GetSizeUsingAttributes(attribs);
				_titleChanged = false;
			}

			var labelWidth = _titleSize.Width;
			var imageWidth = control.ImageView.Image.Size.Width;

			if (position == Button.ButtonContentLayout.ImagePosition.Right)
			{
				control.ImageEdgeInsets = new UIEdgeInsets(0, labelWidth + spacing, 0, -(labelWidth + spacing));
				control.TitleEdgeInsets = new UIEdgeInsets(0, -(imageWidth + spacing), 0, imageWidth + spacing);
				UpdateContentEdge(new UIEdgeInsets(0, horizontalPadding, 0, horizontalPadding));
				return;
			}

			// top and bottom

			var imageVertOffset = (_titleSize.Height / 2) + spacing;
			var titleVertOffset = (control.ImageView.Image.Size.Height / 2) + spacing;

			var horizontalImageOffset = labelWidth / 2;
			var horizontalTitleOffset = imageWidth / 2;

			var edgeOffset = (nfloat)Math.Min(imageVertOffset, titleVertOffset);
			if (_spacingAdjustsVerticalPadding)
				edgeOffset += spacing;

			if (position == Button.ButtonContentLayout.ImagePosition.Bottom)
			{
				imageVertOffset = -imageVertOffset;
				titleVertOffset = -titleVertOffset;
			}

			nfloat collapseAdjustment = 0;
			if (_collapseHorizontalPadding)
				collapseAdjustment = (nfloat)(labelWidth + imageWidth - Math.Max(labelWidth, imageWidth)) / 2;

			control.ImageEdgeInsets = new UIEdgeInsets(-imageVertOffset, horizontalImageOffset, imageVertOffset, -horizontalImageOffset);
			control.TitleEdgeInsets = new UIEdgeInsets(titleVertOffset, -horizontalTitleOffset, -titleVertOffset, horizontalTitleOffset);
			UpdateContentEdge(new UIEdgeInsets(edgeOffset, -collapseAdjustment, edgeOffset, -collapseAdjustment));
		}
	}
}
