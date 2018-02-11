using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[ContentProperty("Text")]
	public sealed class Span : Element, IFontElement, ITextElement
	{
		readonly ObservableCollection<IGestureRecognizer> _gestureRecognizers = new ObservableCollection<IGestureRecognizer>();

		internal readonly MergedStyle _mergedStyle;
		
		public Span()
		{
			_mergedStyle = new MergedStyle(GetType(), this);

			_gestureRecognizers.CollectionChanged += (sender, args) =>
			{
				switch (args.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (IElement item in args.NewItems.OfType<IElement>())
						{
							ValidateGesture(item as IGestureRecognizer);
							item.Parent = this;
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (IElement item in args.OldItems.OfType<IElement>())
							item.Parent = null;
						break;
					case NotifyCollectionChangedAction.Replace:
						foreach (IElement item in args.NewItems.OfType<IElement>())
						{
							ValidateGesture(item as IGestureRecognizer);
							item.Parent = this;
						}
						foreach (IElement item in args.OldItems.OfType<IElement>())
							item.Parent = null;
						break;
					case NotifyCollectionChangedAction.Reset:
						foreach (IElement item in _gestureRecognizers.OfType<IElement>())
							item.Parent = this;
						break;
				}
			};
		}

		public IList<IGestureRecognizer> GestureRecognizers
		{
			get { return _gestureRecognizers; }
		}

		public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Span), null);

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly BindableProperty StyleProperty = BindableProperty.Create("Style", typeof(Style), typeof(Span), default(Style),
			propertyChanged: (bindable, oldvalue, newvalue) => ((Span)bindable)._mergedStyle.Style = (Style)newvalue);

		public Style Style
		{
			get { return (Style)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		public static readonly BindableProperty BackgroundColorProperty
			= BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Span), default(Color));

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		[Obsolete("Foreground is obsolete as of version 2.6.0. Please use the TextColor property instead.")]
		public static readonly BindableProperty ForegroundColorProperty = TextColorProperty;
		
#pragma warning disable 618
		public Color ForegroundColor
		{
			get { return (Color)GetValue(ForegroundColorProperty); }
			set { SetValue(ForegroundColorProperty, value); }
		}
#pragma warning restore 618
		
		public static readonly BindableProperty TextProperty
			= BindableProperty.Create(nameof(Text), typeof(string), typeof(Span), "");

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set	{ SetValue(TextProperty, value); }
		}

		public static readonly BindableProperty FontProperty = FontElement.FontProperty;

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		[Obsolete("Font is obsolete as of version 1.3.0. Please use the Font properties directly.")]
		public Font Font
		{
			get { return (Font)GetValue(FontElement.FontProperty); }
			set { SetValue(FontElement.FontProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontElement.FontAttributesProperty); }
			set { SetValue(FontElement.FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontElement.FontFamilyProperty); }
			set { SetValue(FontElement.FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontElement.FontSizeProperty); }
			set { SetValue(FontElement.FontSizeProperty, value); }
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue)
		{
		}

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue)
		{
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, new Label());

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
		{			
		}

		void IFontElement.OnFontChanged(Font oldValue, Font newValue)
		{
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		public IList<Rectangle> Positions { get; set; }

		public void CalculatePositions(int[] lineHeights, double maxWidth, double startX, double endX)
		{
			var positions = new List<Rectangle>();
			var endLine = lineHeights.Length - 1;
			var lineHeightTotal = 0;

			for (var i = 0; i <= endLine; i++)
			{
				if (endLine != 0) // MultiLine
				{					
					if (i == 0) // First Line
						positions.Add(new Rectangle(startX, 0, maxWidth - startX, lineHeights[i]));

					else if (i != endLine) // Middle Line
						positions.Add(new Rectangle(0, lineHeightTotal, maxWidth, lineHeights[i]));
					
					else // End Line
						positions.Add(new Rectangle(0, lineHeightTotal, endX, lineHeights[i]));

					lineHeightTotal += lineHeights[i];
				}
				else // SingleLine
				{
					positions.Add(new Rectangle(startX, 0, endX - startX, lineHeights[i]));
				}
			}

			Positions = positions;
		}

		void ValidateGesture(IGestureRecognizer gesture)
		{
			if (gesture == null)
				return;
			if (gesture is PinchGestureRecognizer && _gestureRecognizers.GetGesturesFor<PinchGestureRecognizer>().Count() > 1)
				throw new InvalidOperationException($"Only one {nameof(PinchGestureRecognizer)} per view is allowed");
		}
	}
}