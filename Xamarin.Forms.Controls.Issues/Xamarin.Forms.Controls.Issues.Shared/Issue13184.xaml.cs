using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13184, "[Bug] SwipeView Items Lose their Positions when Scrolled & Swiped (iOS)",
		PlatformAffected.iOS)]
	public partial class Issue13184 : TestContentPage
	{
		public Issue13184()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue13184ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	class Issue13184Model : BindableObject
	{
		int _count = 1;

		public string Title { get; set; }

		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged("Count");
			}
		}
	}

	[Preserve(AllMembers = true)]
	class Issue13184ViewModel : BindableObject
	{
		public ObservableCollection<Issue13184Model> Items { get; private set; } = new ObservableCollection<Issue13184Model>();

		public ICommand IncrementCommand => new Command<Issue13184Model>(Increment);
		public ICommand DecrementCommand => new Command<Issue13184Model>(Decrement);

		public Issue13184ViewModel()
		{
			Items = new ObservableCollection<Issue13184Model>
			{
				new Issue13184Model(){ Title = "item 1" , Count=2 },
				new Issue13184Model(){ Title = "item 2" , Count=5 },
				new Issue13184Model(){ Title = "item 3" , Count=5 },
				new Issue13184Model(){ Title = "item 4" , Count=7 },
				new Issue13184Model(){ Title = "item 5" , Count=2 },
				new Issue13184Model(){ Title = "item 6" , Count=9 },
				new Issue13184Model(){ Title = "item 7" , Count=6 },
				new Issue13184Model(){ Title = "item 8" , Count=5 },
				new Issue13184Model(){ Title = "item 9" , Count=8 },
				new Issue13184Model(){ Title = "item 10" , Count=2 },
				new Issue13184Model(){ Title = "item 11" , Count=5 },
				new Issue13184Model(){ Title = "item 12" , Count=1 },
				new Issue13184Model(){ Title = "item 13" , Count=7 },
				new Issue13184Model(){ Title = "item 14" , Count=3 },
				new Issue13184Model(){ Title = "item 15" , Count=8 },
				new Issue13184Model(){ Title = "item 16" , Count=7 },
				new Issue13184Model(){ Title = "item 17" , Count=5 },
				new Issue13184Model(){ Title = "item 18" , Count=2 },
				new Issue13184Model(){ Title = "item 19" , Count=8 },
				new Issue13184Model(){ Title = "item 20" , Count=4 }
			};
		}

		public void Increment(Issue13184Model item)
		{
			item.Count++;
		}
		public void Decrement(Issue13184Model item)
		{
			item.Count--;
		}
	}
}