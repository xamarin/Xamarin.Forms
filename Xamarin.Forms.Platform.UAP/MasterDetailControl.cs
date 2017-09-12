using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using System.Linq;

namespace Xamarin.Forms.Platform.UWP
{
	public class MasterDetailControl : Control, IToolbarProvider
	{
		public static readonly DependencyProperty MasterProperty = DependencyProperty.Register("Master", typeof(FrameworkElement), typeof(MasterDetailControl),
			new PropertyMetadata(default(FrameworkElement)));

		public static readonly DependencyProperty MasterTitleProperty = DependencyProperty.Register("MasterTitle", typeof(string), typeof(MasterDetailControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty DetailContentProperty = DependencyProperty.Register("DetailContent", typeof(Canvas), typeof(MasterDetailControl),
			new PropertyMetadata(default(Canvas)));

		public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(MasterDetailControl), new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty ShouldShowSplitModeProperty = DependencyProperty.Register(nameof(ShouldShowSplitMode), typeof(bool), typeof(MasterDetailControl),
			new PropertyMetadata(default(bool), OnShouldShowSplitModeChanged));

		public static readonly DependencyProperty CollapseStyleProperty = DependencyProperty.Register(nameof(CollapseStyle), typeof(CollapseStyle), 
			typeof(MasterDetailControl), new PropertyMetadata(CollapseStyle.Full, CollapseStyleChanged));

		public static readonly DependencyProperty CollapsedPaneWidthProperty = DependencyProperty.Register(nameof(CollapsedPaneWidth), typeof(double), typeof(MasterDetailControl),
			new PropertyMetadata(48d, CollapsedPaneWidthChanged));

		public static readonly DependencyProperty DetailTitleProperty = DependencyProperty.Register("DetailTitle", typeof(string), typeof(MasterDetailControl), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(MasterDetailControl),
			new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(MasterDetailControl),
			new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty MasterTitleVisibilityProperty = DependencyProperty.Register("MasterTitleVisibility", typeof(Visibility), typeof(MasterDetailControl),
			new PropertyMetadata(default(Visibility)));

		public static readonly DependencyProperty DetailTitleVisibilityProperty = DependencyProperty.Register("DetailTitleVisibility", typeof(Visibility), typeof(MasterDetailControl),
			new PropertyMetadata(default(Visibility)));

		public static readonly DependencyProperty MasterToolbarVisibilityProperty = DependencyProperty.Register("MasterToolbarVisibility", typeof(Visibility), typeof(MasterDetailControl),
			new PropertyMetadata(default(Visibility)));

		public static readonly DependencyProperty ContentTogglePaneButtonVisibilityProperty = DependencyProperty.Register(nameof(ContentTogglePaneButtonVisibility), typeof(Visibility), typeof(MasterDetailControl),
			new PropertyMetadata(default(Visibility)));
		
		CommandBar _commandBar;
		readonly ToolbarPlacementHelper _toolbarPlacementHelper = new ToolbarPlacementHelper();

		public bool ShouldShowToolbar
		{
			get { return _toolbarPlacementHelper.ShouldShowToolBar; }
			set { _toolbarPlacementHelper.ShouldShowToolBar = value; }
		}

		TaskCompletionSource<CommandBar> _commandBarTcs;
		FrameworkElement _masterPresenter;
		FrameworkElement _detailPresenter;
		SplitView _split;
	    ToolbarPlacement _toolbarPlacement;

	    public MasterDetailControl()
		{
			DefaultStyleKey = typeof(MasterDetailControl);

			DetailTitleVisibility = Visibility.Collapsed;

			CollapseStyle = CollapseStyle.Full;

            DetailContent = new Canvas();
		}

        public Canvas DetailContent
        {
            get { return (Canvas)GetValue(DetailContentProperty); }
            set { SetValue(DetailContentProperty, value); }
        }


        /*** Every time display a element, that's take too much performance, especially for some exist page, That's why
        I changed the original lotic, replace it with visibility, next time navigate exist page or switch exists detail page(For MasterDetailPage) it will be fast displayed, even there are too much element in target page ***/
        public UIElement Detail
        {
            get
            {
                return this.DetailContent.Children.Where(x => x.Visibility == Visibility.Visible).FirstOrDefault();
            }
            set
            {
                UIElement newElement = value as UIElement;
                UIElement oldElement = this.Detail as UIElement;


                if (newElement != oldElement)
                {
                    if (null != oldElement)
                        oldElement.Visibility = Visibility.Collapsed;


                    if (null != newElement)
                    {
                        if (this.DetailContent.Children.Any(x => x == newElement))
                            newElement.Visibility = Visibility.Visible;
                        else
                            this.DetailContent.Children.Add(newElement);
                    }
                }

            }
        }

        internal bool CheckContentIfExist(FrameworkElement element)
        {
            return this.DetailContent.Children.Any(x => x == element);
        }

        internal void RemoveContent(FrameworkElement element)
        {
            DetailContent.Children.Remove(element);
        }


        public Windows.Foundation.Size DetailSize
		{
			get
			{
                //double height = ActualHeight;
                //double width = ActualWidth;

                //if (_commandBar != null)
                //	height -= _commandBar.ActualHeight;

                //if (ShouldShowSplitMode && IsPaneOpen)
                //{
                //	if (_split != null)
                //		width -= _split.OpenPaneLength;
                //	else if (_detailPresenter != null)
                //		width -= _masterPresenter.ActualWidth;
                //}

                //return new Windows.Foundation.Size(width, height);

                // This is more clean and good way, if use the old way, it will failed calculate the details size after resize
                return new Windows.Foundation.Size(this.DetailContent.ActualWidth, this.DetailContent.ActualHeight);

            }
		}

		public string DetailTitle
		{
			get { return (string)GetValue(DetailTitleProperty); }
			set { SetValue(DetailTitleProperty, value); }
		}

		public Visibility DetailTitleVisibility
		{
			get { return (Visibility)GetValue(DetailTitleVisibilityProperty); }
			set { SetValue(DetailTitleVisibilityProperty, value); }
		}

		public bool IsPaneOpen
		{
			get { return (bool)GetValue(IsPaneOpenProperty); }
			set { SetValue(IsPaneOpenProperty, value); }
		}

		public FrameworkElement Master
		{
			get { return (FrameworkElement)GetValue(MasterProperty); }
			set { SetValue(MasterProperty, value); }
		}

		public Windows.Foundation.Size MasterSize
		{
			get
			{
				// Use the ActualHeight of the _masterPresenter to automatically adjust for the Master Title
				double height = _masterPresenter?.ActualHeight ?? 0;

				// If there's no content, use the height of the control to make sure the background color expands.
				if (height == 0)
					height = ActualHeight;

				double width = 0;

				// On first load, the _commandBar will still occupy space by the time this is called.
				// Check ShouldShowToolbar to make sure the _commandBar will still be there on render.
				if (_commandBar != null && ShouldShowToolbar)
					height -= _commandBar.ActualHeight;

				if (_split != null)
					width = _split.OpenPaneLength;
				else if (_masterPresenter != null)
					width = _masterPresenter.ActualWidth;

				return new Windows.Foundation.Size(width, height);
			}
		}

		public string MasterTitle
		{
			get { return (string)GetValue(MasterTitleProperty); }
			set { SetValue(MasterTitleProperty, value); }
		}

		public Visibility MasterTitleVisibility
		{
			get { return (Visibility)GetValue(MasterTitleVisibilityProperty); }
			set { SetValue(MasterTitleVisibilityProperty, value); }
		}

		public Visibility MasterToolbarVisibility
		{
			get { return (Visibility)GetValue(MasterToolbarVisibilityProperty); }
			set { SetValue(MasterToolbarVisibilityProperty, value); }
		}

		public bool ShouldShowSplitMode
		{
			get { return (bool)GetValue(ShouldShowSplitModeProperty); }
			set { SetValue(ShouldShowSplitModeProperty, value); }
		}

		public CollapseStyle CollapseStyle
		{
			get { return (CollapseStyle)GetValue(CollapseStyleProperty); }
			set { SetValue(CollapseStyleProperty, value); }
		}

	    public ToolbarPlacement ToolbarPlacement
	    {
	        get { return _toolbarPlacement; }
	        set
	        {
	            _toolbarPlacement = value;
	            _toolbarPlacementHelper.UpdateToolbarPlacement();
	        }
	    }

	    public Visibility ContentTogglePaneButtonVisibility
		{
			get { return (Visibility)GetValue(ContentTogglePaneButtonVisibilityProperty); }
			set { SetValue(ContentTogglePaneButtonVisibilityProperty, value); }
		}

		public double CollapsedPaneWidth
		{
			get { return (double)GetValue(CollapsedPaneWidthProperty); }
			set { SetValue(CollapsedPaneWidthProperty, value); }
		}

		public Brush ToolbarBackground
		{
			get { return (Brush)GetValue(ToolbarBackgroundProperty); }
			set { SetValue(ToolbarBackgroundProperty, value); }
		}

		public Brush ToolbarForeground
		{
			get { return (Brush)GetValue(ToolbarForegroundProperty); }
			set { SetValue(ToolbarForegroundProperty, value); }
		}

		Task<CommandBar> IToolbarProvider.GetCommandBarAsync()
		{
			if (_commandBar != null)
				return Task.FromResult(_commandBar);

			_commandBarTcs = new TaskCompletionSource<CommandBar>();
			ApplyTemplate();

			var commandBarFromTemplate = _commandBarTcs.Task;
			_commandBarTcs = null;

			return commandBarFromTemplate;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_split = GetTemplateChild("SplitView") as SplitView;
			if (_split == null)
				return;

			var paneToggle = GetTemplateChild("PaneTogglePane") as Windows.UI.Xaml.Controls.Button;
			if (paneToggle != null)
				paneToggle.Click += OnToggleClicked;

			var contentToggle = GetTemplateChild("ContentTogglePane") as Windows.UI.Xaml.Controls.Button;
			if (contentToggle != null)
				contentToggle.Click += OnToggleClicked;

			_masterPresenter = GetTemplateChild("MasterPresenter") as FrameworkElement;
			_detailPresenter = GetTemplateChild("DetailPresenter") as FrameworkElement;

			_commandBar = GetTemplateChild("CommandBar") as CommandBar;
			_toolbarPlacementHelper.Initialize(_commandBar, () => ToolbarPlacement, GetTemplateChild);
			
			UpdateMode(); 

			if (_commandBarTcs != null)
				_commandBarTcs.SetResult(_commandBar);
		}

		static void OnShouldShowSplitModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			((MasterDetailControl)dependencyObject).UpdateMode();
		}

		static void CollapseStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			((MasterDetailControl)dependencyObject).UpdateMode();
		}

		static void CollapsedPaneWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((MasterDetailControl)dependencyObject).UpdateMode();
		}

		void OnToggleClicked(object sender, RoutedEventArgs args)
		{
			IsPaneOpen = !IsPaneOpen;
		}

		void UpdateMode()
		{
			if (_split == null)
			{
                return;
			}

			_split.DisplayMode = ShouldShowSplitMode 
				? SplitViewDisplayMode.Inline 
				: CollapseStyle == CollapseStyle.Full ? SplitViewDisplayMode.Overlay : SplitViewDisplayMode.CompactOverlay;

			_split.CompactPaneLength = CollapsedPaneWidth;

            if (_split.DisplayMode == SplitViewDisplayMode.Inline)
            {
                // If we've determined that the pane will always be open, then there's no
                // reason to display the show/hide pane button in the master
                MasterToolbarVisibility = Visibility.Collapsed;
            }

            // If we're in compact mode or the pane is always open,
            // we don't need to display the content pane's toggle button
            ContentTogglePaneButtonVisibility = _split.DisplayMode == SplitViewDisplayMode.Overlay 
				? Visibility.Visible 
				: Visibility.Collapsed;

            // if panel default mode is always open, but we set IsPresented = false
            if (!IsPaneOpen)
            {
                ContentTogglePaneButtonVisibility = Visibility.Visible;
            }

            if (ContentTogglePaneButtonVisibility == Visibility.Visible)
                DetailTitleVisibility = Visibility.Visible;
        }
    }
}