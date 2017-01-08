﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using System.Linq;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
    public sealed partial class PageControl : IToolbarProvider
	{
		public static readonly DependencyProperty InvisibleBackButtonCollapsedProperty = DependencyProperty.Register("InvisibleBackButtonCollapsed", typeof(bool), typeof(PageControl),
			new PropertyMetadata(true, OnInvisibleBackButtonCollapsedChanged));

		public static readonly DependencyProperty ShowBackButtonProperty = DependencyProperty.Register("ShowBackButton", typeof(bool), typeof(PageControl),
			new PropertyMetadata(false, OnShowBackButtonChanged));

		public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register(nameof(TitleVisibility), typeof(Visibility), typeof(PageControl), new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register(nameof(ToolbarBackground), typeof(Brush), typeof(PageControl),
			new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty BackButtonTitleProperty = DependencyProperty.Register("BackButtonTitle", typeof(string), typeof(PageControl), new PropertyMetadata(false));

		public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Windows.UI.Xaml.Thickness), typeof(PageControl),
			new PropertyMetadata(default(Windows.UI.Xaml.Thickness)));

		public static readonly DependencyProperty TitleInsetProperty = DependencyProperty.Register("TitleInset", typeof(double), typeof(PageControl), new PropertyMetadata(default(double)));

		public static readonly DependencyProperty TitleBrushProperty = DependencyProperty.Register("TitleBrush", typeof(Brush), typeof(PageControl), new PropertyMetadata(null));

		AppBarButton _backButton;
		CommandBar _commandBar;

#if WINDOWS_UWP
        ToolbarPlacement _toolbarPlacement;
	    readonly ToolbarPlacementHelper _toolbarPlacementHelper = new ToolbarPlacementHelper();

		public bool ShouldShowToolbar
		{
			get { return _toolbarPlacementHelper.ShouldShowToolBar; }
			set { _toolbarPlacementHelper.ShouldShowToolBar = value; }
		}
#endif

		TaskCompletionSource<CommandBar> _commandBarTcs;
		Windows.UI.Xaml.Controls.ContentPresenter _presenter;

        Canvas ContentController;



        public PageControl()
		{
			InitializeComponent();
            ContentController = new Canvas();
            base.Content = ContentController;


        }

		public string BackButtonTitle
		{
			get { return (string)GetValue(BackButtonTitleProperty); }
			set { SetValue(BackButtonTitleProperty, value); }
		}

		public double ContentHeight
		{
			get { return _presenter != null ? _presenter.ActualHeight : 0; }
		}

		public Windows.UI.Xaml.Thickness ContentMargin
		{
			get { return (Windows.UI.Xaml.Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}

		public double ContentWidth
		{
			get { return _presenter != null ? _presenter.ActualWidth : 0; }
		}

		public bool InvisibleBackButtonCollapsed
		{
			get { return (bool)GetValue(InvisibleBackButtonCollapsedProperty); }
			set { SetValue(InvisibleBackButtonCollapsedProperty, value); }
		}

		public Brush ToolbarBackground
		{
			get { return (Brush)GetValue(ToolbarBackgroundProperty); }
			set { SetValue(ToolbarBackgroundProperty, value); }
		}

#if WINDOWS_UWP
        public ToolbarPlacement ToolbarPlacement
        {
            get { return _toolbarPlacement; }
            set
            {
                _toolbarPlacement = value; 
                _toolbarPlacementHelper.UpdateToolbarPlacement();
            }
        }
#endif

		public bool ShowBackButton
		{
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}

		public Visibility TitleVisibility
		{
			get { return (Visibility)GetValue(TitleVisibilityProperty); }
			set { SetValue(TitleVisibilityProperty, value); }
		}

		public Brush TitleBrush
		{
			get { return (Brush)GetValue(TitleBrushProperty); }
			set { SetValue(TitleBrushProperty, value); }
		}

		public double TitleInset
		{
			get { return (double)GetValue(TitleInsetProperty); }
			set { SetValue(TitleInsetProperty, value); }
		}

		Task<CommandBar> IToolbarProvider.GetCommandBarAsync()
		{
			if (_commandBar != null)
				return Task.FromResult(_commandBar);

			_commandBarTcs = new TaskCompletionSource<CommandBar>();
			ApplyTemplate();
			return _commandBarTcs.Task;
		}

		public event RoutedEventHandler BackClicked;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_backButton = GetTemplateChild("backButton") as AppBarButton;
			if (_backButton != null)
				_backButton.Click += OnBackClicked;

			_presenter = GetTemplateChild("presenter") as Windows.UI.Xaml.Controls.ContentPresenter;

			_commandBar = GetTemplateChild("CommandBar") as CommandBar;

#if WINDOWS_UWP
			_toolbarPlacementHelper.Initialize(_commandBar, () => ToolbarPlacement, GetTemplateChild);
#endif

			TaskCompletionSource<CommandBar> tcs = _commandBarTcs;
		    tcs?.SetResult(_commandBar);
		}

		void OnBackClicked(object sender, RoutedEventArgs e)
		{
			RoutedEventHandler clicked = BackClicked;
			if (clicked != null)
				clicked(this, e);
		}

		static void OnInvisibleBackButtonCollapsedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((PageControl)dependencyObject).UpdateBackButton();
		}

		static void OnShowBackButtonChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((PageControl)dependencyObject).UpdateBackButton();
		}

		void UpdateBackButton()
		{
			if (_backButton == null)
				return;

			if (ShowBackButton)
				_backButton.Visibility = Visibility.Visible;
			else
				_backButton.Visibility = InvisibleBackButtonCollapsed ? Visibility.Collapsed : Visibility.Visible;

			_backButton.Opacity = ShowBackButton ? 1 : 0;
		}

        /*** Every time display a element, that's take too much performance, especially for some exist page, That's why
        I changed the original lotic, replace it with visibility, next time navigate exist page or switch exists content page(For NavigationPage) it will be fast displayed, even there are too much element in target page ***/
        public new object Content
        {
            get
            {
                return this.ContentController.Children.Where(x => x.Visibility == Visibility.Visible).FirstOrDefault();
            }
            set
            {
                UIElement newElement = value as UIElement;
                UIElement oldElement = this.Content as UIElement;


                if (newElement != oldElement)
                {
                    if (null != oldElement)
                        oldElement.Visibility = Visibility.Collapsed;


                    if (null != newElement)
                    {
                        if (this.ContentController.Children.Any(x => x == newElement))
                            newElement.Visibility = Visibility.Visible;
                        else
                            this.ContentController.Children.Add(newElement);
                    }
                }
            }
        }

        internal bool CheckContentIfExist(UIElement element) {
            return this.ContentController.Children.Any(x => x == element);
        }


        internal void RecalcVisiblitiy(UIElement element)
        {
            foreach (var item in this.ContentController.Children)
            {
                if (element == item && element.Visibility != item.Visibility)
                    item.Visibility = element.Visibility;
                else if (element != item && element.Visibility != Visibility.Collapsed)
                    item.Visibility = Visibility.Collapsed;
            }
        }

        internal void RemoveContent(UIElement element)
        {
            ContentController.Children.Remove(element);
        }

    }
}