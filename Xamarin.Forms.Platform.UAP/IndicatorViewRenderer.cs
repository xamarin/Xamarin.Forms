using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.Platform.UWP
{
	class IndicatorViewRenderer : ViewRenderer<IndicatorView, FrameworkElement>
	{
		SolidColorBrush selectedColor;
		SolidColorBrush fillColor;
		double _defaultIndicatorSize = 7;
		double _defaultIndicatorPadding = 4;

		ObservableCollection<Shape> Dots;

		public IndicatorViewRenderer()
		{
			AutoPackage = false;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<IndicatorView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					UpdateControl();	
				}
			}

			fillColor = new SolidColorBrush(Element.IndicatorColor.ToWindowsColor());

			selectedColor = new SolidColorBrush(Element.SelectedIndicatorColor.ToWindowsColor());

			CreateIndicators();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.IsOneOf(IndicatorView.IndicatorColorProperty, IndicatorView.SelectedIndicatorColorProperty))
				UpdateIndicatorsColor();
		}

		void UpdateControl()
		{
			ClearIndicators();

			var control = (Element.IndicatorTemplate != null)
				? (FrameworkElement)Element.IndicatorLayout.GetOrCreateRenderer()
				: CreateNativeControl();

			SetNativeControl(control);
		}

		FrameworkElement CreateNativeControl()
		{
			var itemsControl = new ItemsControl
			{
				ItemsPanel = ParseItemsPanelTemplate(typeof(StackPanel))
			};
			return itemsControl;
		}

		void UpdateIndicatorsColor()
		{
			var position = Element.Position;
			int i = 0;
			foreach (var item in (Control as ItemsControl).Items)
			{
				((Shape)item).Fill = i ==  position ? selectedColor : fillColor;
				i++;
			}
		}

		ItemsPanelTemplate ParseItemsPanelTemplate(Type panelType)
		{
			var itemsPanelTemplateXaml =
				$@"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <StackPanel  Orientation='Horizontal'></StackPanel>
			   </ItemsPanelTemplate>";

			return (ItemsPanelTemplate)XamlReader.Load(itemsPanelTemplateXaml);
		}

		void CreateIndicators()
		{
			if (!Element.IsVisible)
				return;

			var position = Element.Position;
			var indicators = new List<Shape>();

			if (Element.ItemsSource != null && Element.Count > 0)
			{
				int i = 0;
				foreach (var item in Element.ItemsSource)
				{
					indicators.Add(CreateIndicator(i, position));
					i++;
				}
			}

			Dots = new ObservableCollection<Shape>(indicators);
			(Control as ItemsControl).ItemsSource = Dots;
		}
		void ClearIndicators()
		{
		}

		Shape CreateIndicator(int i, int position)
		{
			if (Element.IndicatorsShape == IndicatorShape.Circle)
			{
				return new Ellipse()
				{
					Fill = i == position ? selectedColor : fillColor,
					Height = _defaultIndicatorSize,
					Width = _defaultIndicatorSize,
					Margin = new Windows.UI.Xaml.Thickness(_defaultIndicatorPadding)
				};
			}
			else
			{
				return new Windows.UI.Xaml.Shapes.Rectangle()
				{
					Fill = i == position ? selectedColor : fillColor,
					Height = _defaultIndicatorSize,
					Width = _defaultIndicatorSize,
					Margin = new Windows.UI.Xaml.Thickness(_defaultIndicatorPadding)
				};
			}
		}
	}
}
