using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF
{
	public class TableViewRenderer : ViewRenderer<Xamarin.Forms.TableView, TableView>
	{
		TableView _view;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
			result.Minimum = new Size(40, 40);
			return result;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TableView> e)
		{
			base.OnElementChanged(e);

			Element.ModelChanged += OnModelChanged;

			_view = new TableView { DataContext = Element.Root };
            
			_view.MouseUp += OnTapTable;
			_view.MouseRightButtonUp += OnLongPressTable;
			SetNativeControl(_view);
		}

		

		void OnLongPressTable(object sender, MouseButtonEventArgs e)
		{
		}

		void OnModelChanged(object sender, EventArgs eventArgs)
		{
			_view.DataContext = Element.Root;
		}

		void OnTapTable(object sender, MouseEventArgs e)
		{
		}
	}
}