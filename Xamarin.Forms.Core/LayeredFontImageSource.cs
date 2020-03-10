using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	[ContentProperty(nameof(Layers))]
	public class LayeredFontImageSource : ImageSource
	{
		public override bool IsEmpty => Layers == null || Layers.Count == 0 || Layers.All(x => x.IsEmpty);

		public static readonly BindableProperty LayersProperty = BindableProperty.Create(nameof(Layers), typeof(IList<FontImageSource>), typeof(LayeredFontImageSource), null,
						propertyChanged: (b, o, n) => ((LayeredFontImageSource)b).OnLayersChanged((IList<FontImageSource>)o, (IList<FontImageSource>)n));

		public IList<FontImageSource> Layers
		{
			get => (IList<FontImageSource>)GetValue(LayersProperty);
			set => SetValue(LayersProperty, value);
		}

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(LayeredFontImageSource), default(Color),
						propertyChanged: (b, o, n) => ((LayeredFontImageSource)b).OnSourceChanged());

		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(LayeredFontImageSource), default(string),
						propertyChanged: (b, o, n) => ((LayeredFontImageSource)b).OnSourceChanged());

		public string FontFamily
		{
			get => (string)GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}

		public static readonly BindableProperty GlyphProperty = BindableProperty.Create(nameof(Glyph), typeof(string), typeof(LayeredFontImageSource), default(string),
						propertyChanged: (b, o, n) => ((LayeredFontImageSource)b).OnSourceChanged());

		public string Glyph
		{
			get => (string)GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}

		public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(double), typeof(LayeredFontImageSource), 30d,
						propertyChanged: (b, o, n) => ((LayeredFontImageSource)b).OnSourceChanged());

		[TypeConverter(typeof(FontSizeConverter))]
		public double Size
		{
			get => (double)GetValue(SizeProperty);
			set => SetValue(SizeProperty, value);
		}

		void OnLayersChanged(IList<FontImageSource> oldLayers, IList<FontImageSource> newLayers)
		{
			if (oldLayers != null)
			{
				foreach (var layer in oldLayers)
				{
					layer.SourceChanged -= LayerSourceChanged;
				}
			}
			if (newLayers != null)
			{
				foreach (var layer in newLayers)
				{
					layer.SourceChanged += LayerSourceChanged;
				}
			}
			OnSourceChanged();
		}

		void LayerSourceChanged(object sender, EventArgs e)
		{
			OnSourceChanged();
		}
	}
}