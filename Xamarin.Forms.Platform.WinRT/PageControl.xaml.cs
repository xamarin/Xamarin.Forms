using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

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
        Border _titleBar;
        ToolbarPlacement _toolbarPlacement;
#endif

        TaskCompletionSource<CommandBar> _commandBarTcs;
		Windows.UI.Xaml.Controls.ContentPresenter _presenter;

        public PageControl()
		{
			InitializeComponent();
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
                UpdateToolbarPlacement();
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
            _titleBar = GetTemplateChild("TitleBar") as Border;
			UpdateToolbarPlacement();
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

#if WINDOWS_UWP
        void UpdateToolbarPlacement()
		{
			if (_commandBar == null)
			{
				return;
			}

			switch (ToolbarPlacement)
			{
				case ToolbarPlacement.Top:
					Windows.UI.Xaml.Controls.Grid.SetRow(_commandBar, 0);
					break;
				case ToolbarPlacement.Bottom:
					Windows.UI.Xaml.Controls.Grid.SetRow(_commandBar, 2);
					break;
				case ToolbarPlacement.Default:
				default:
					Windows.UI.Xaml.Controls.Grid.SetRow(_commandBar, Device.Idiom == TargetIdiom.Phone ? 2 : 0);
					break;
			}

		    AdjustCommandBarForTitle();
		}

        void AdjustCommandBarForTitle()
		{
           if (_commandBar == null || _titleBar == null)
		    {
		        return;
		    }
            
		   // TODO EZH This fails if you have the toolbar at the top and remove all of the commands; it collapses and we're left with a blank section to the right of the title
		   // We can probably make this work by dropping the left auto column in each layout and making the top row control a border which goes all the way across with the title background
		   // the contents would be a grid with auto|* and a placeholder for the command bar; the code below could just reparent the command bar and ignore titlebar altogether
		   // probably will fix the top bar gap under the title on narrow screens when the secondary command menu opens (see phone with toolbarplacement top for example)

			if (Windows.UI.Xaml.Controls.Grid.GetRow(_commandBar) == 0)
			{
				Windows.UI.Xaml.Controls.Grid.SetColumn(_commandBar, 1);
				Windows.UI.Xaml.Controls.Grid.SetColumnSpan(_commandBar, 1);
                Windows.UI.Xaml.Controls.Grid.SetColumnSpan(_titleBar, 1);
			}
			else
			{
				Windows.UI.Xaml.Controls.Grid.SetColumn(_commandBar, 0);
				Windows.UI.Xaml.Controls.Grid.SetColumnSpan(_commandBar, 2);
                Windows.UI.Xaml.Controls.Grid.SetColumnSpan(_titleBar, 2);
			}
		}
#endif
    }
}