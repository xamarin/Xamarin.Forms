using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Brush)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13017, "[Bug] Background doesn't change from LinearGradientBrush to SolidColorBrush by DataTrigger on Android",
		PlatformAffected.Android)]
	public partial class Issue13017 : TestContentPage
	{
		public Issue13017()
		{
#if APP
			Title = "Issue 13017";
			InitializeComponent();
			BindingContext = new Issue13017ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue13017ViewModel : INotifyPropertyChanged
	{
		int _backgroundNumber = 1;

		public Issue13017ViewModel()
		{
			SelectGradient = new Command(() =>
			{
				BackgroundNumber = 1;
			});

			SelectSolidBrushGradient = new Command(() =>
			{
				BackgroundNumber = 2;
			});
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand SelectGradient { get; private set; }
		public ICommand SelectSolidBrushGradient { get; private set; }

		public int BackgroundNumber
		{
			get => _backgroundNumber;
			set
			{
				_backgroundNumber = value;
				OnPropertyChanged(nameof(BackgroundNumber));
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}