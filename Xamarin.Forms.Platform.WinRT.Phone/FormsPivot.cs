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
	public class FormsPivot : Pivot, IToolbarProvider
	{
		public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register(nameof(TitleVisibility), typeof(Visibility), typeof(FormsPivot),
			new PropertyMetadata(Visibility.Collapsed));

		public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register(nameof(ToolbarForeground), typeof(Brush), typeof(FormsPivot), new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register(nameof(ToolbarBackground), typeof(Brush), typeof(FormsPivot), new PropertyMetadata(default(Brush)));

		CommandBar _commandBar;

		TaskCompletionSource<CommandBar> _commandBarTcs;
	    ToolbarPlacement _toolbarPlacement;
	    ContentControl _titleBar;

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

		public Visibility TitleVisibility
		{
			get { return (Visibility)GetValue(TitleVisibilityProperty); }
			set { SetValue(TitleVisibilityProperty, value); }
		}

        public ToolbarPlacement ToolbarPlacement
	    {
	        get { return _toolbarPlacement; }
	        set
	        {
	            _toolbarPlacement = value;
	            UpdateToolbarPlacement();
	        }
	    }

		Task<CommandBar> IToolbarProvider.GetCommandBarAsync()
		{
			if (_commandBar != null)
				return Task.FromResult(_commandBar);

			_commandBarTcs = new TaskCompletionSource<CommandBar>();
			ApplyTemplate();
			return _commandBarTcs.Task;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			_commandBar = GetTemplateChild("CommandBar") as CommandBar;
            _titleBar = GetTemplateChild("TitleBar") as ContentControl;
			UpdateToolbarPlacement();

			TaskCompletionSource<CommandBar> tcs = _commandBarTcs;
		    tcs?.SetResult(_commandBar);
		}

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
	}
}