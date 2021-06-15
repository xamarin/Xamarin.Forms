using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13417,
		"Change background brush in trigger does not work ",
		PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue13417 : TestContentPage
	{
		public Issue13417()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue13417ViewModel();
#endif
		}

		protected override void Init()
		{
		}
	}
	[Preserve(AllMembers = true)]
	public class Issue13417ViewModel : BindableObject
	{
		bool _changed1;
		bool _changed2;
		bool _changed3;

		public bool Changed1
		{
			get { return _changed1; }
			set
			{
				_changed1 = true;
				OnPropertyChanged();
			}
		}

		public bool Changed2
		{
			get { return _changed2; }
			set
			{
				_changed2 = true;
				OnPropertyChanged();
			}
		}

		public bool Changed3
		{
			get { return _changed3; }
			set
			{
				_changed3 = true;
				OnPropertyChanged();
			}
		}

		public ICommand ChangeBackground1Command => new Command(ExecuteChange1Background);
		public ICommand ChangeBackground2Command => new Command(ExecuteChange2Background);
		public ICommand ChangeBackground3Command => new Command(ExecuteChange3Background);

		void ExecuteChange1Background() => Changed1 = true;
		void ExecuteChange2Background() => Changed2 = true;
		void ExecuteChange3Background() => Changed3 = true;
	}
}