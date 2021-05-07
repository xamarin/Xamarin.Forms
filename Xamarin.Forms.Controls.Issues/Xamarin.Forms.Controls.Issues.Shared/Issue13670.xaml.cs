using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13670,
		"[Bug] [XF 5] SwipeView not rendering properly on iOS and Android when swiping",
		PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue13670 : TestContentPage
	{
		public Issue13670()
		{
#if APP
			InitializeComponent();

			Items = new ObservableCollection<Issue13670Model>
			{
				new Issue13670Model { Name = "First" },
				new Issue13670Model { Name = "Second" },
				new Issue13670Model { Name = "Third" },
			};

			InitializeComponent();

			ViewModel = this;
#endif
		}

		protected override void Init()
		{
		}

		public ObservableCollection<Issue13670Model> Items { get; private set; }

		public Issue13670 ViewModel
		{
			get => BindingContext as Issue13670;
			set => BindingContext = value;
		}
	}

	public class Issue13670Model : INotifyPropertyChanged
	{
		string _name;
		bool _canShow = true;

		public event PropertyChangedEventHandler PropertyChanged;

		public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

		public bool CanShow { get => _canShow; set { _canShow = value; OnPropertyChanged(nameof(CanShow)); } }

		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var changed = PropertyChanged;

			if (changed == null)
				return;

			changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}