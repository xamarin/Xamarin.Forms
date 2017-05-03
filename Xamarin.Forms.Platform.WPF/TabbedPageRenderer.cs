using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF
{
	public class TabbedPagePresenter : System.Windows.Controls.ContentPresenter
	{
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}
	}

	public class TabbedPageRenderer : TabControl, IVisualElementRenderer
	{
		TabbedPage _page;
		BackgroundTracker<Control> _tracker;

		public TabbedPageRenderer()
		{
			//SetBinding(TitleProperty, new System.Windows.Data.Binding("Title"));
			SetBinding(ItemsSourceProperty, new System.Windows.Data.Binding("Children"));
			ItemTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["TabbedPageHeader"];
			ContentTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["TabbedPage"];

			SelectionChanged += OnSelectionChanged;
		}

		public UIElement ContainerElement
		{
			get { return this; }
		}

		public VisualElement Element
		{
			get { return _page; }
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(widthConstraint, heightConstraint));
		}

		public void SetElement(VisualElement element)
		{
			TabbedPage oldElement = _page;
			_page = (TabbedPage)element;
			_tracker = new BackgroundTracker<Control>(BackgroundProperty) { Model = _page, Element = this };

			DataContext = element;

			_page.PropertyChanged += OnPropertyChanged;

			Loaded += (sender, args) => ((IPageController)_page).SendAppearing();
			Unloaded += (sender, args) => ((IPageController)_page).SendDisappearing();

			OnElementChanged(new VisualElementChangedEventArgs(_page, element));
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			EventHandler<VisualElementChangedEventArgs> changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentPage")
			{
				Page current = _page.CurrentPage;
				if (current != null)
					SelectedItem = current;
			}
		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_page.CurrentPage = (Page)SelectedItem;
		}
	}
}