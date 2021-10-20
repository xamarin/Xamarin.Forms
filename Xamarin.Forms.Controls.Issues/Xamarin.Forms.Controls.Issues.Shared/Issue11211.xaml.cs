using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CarouselView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11211,
		"[Bug] CarouselView Position stops working when the collection updates",
		PlatformAffected.Android)]
	public partial class Issue11211 : TestContentPage
	{
		int _position;

		public Issue11211()
		{
#if APP
			BindingContext = this;

			AddFewItems();

			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}

		public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

		public int Position
		{
			get { return _position; }
			set
			{
				_position = value;
				OnPropertyChanged();
			}
		}

		public ICommand NextItem => new Command(() =>
		{
			Position = (Position + 1) % Items.Count;
		});

		public ICommand AddItem => new Command(() =>
		{
			var i = Items.Count;
			Items.Add(i.ToString());
			Position = i;
		});

		void AddFewItems()
		{
			var count = 3;
			for (int i = 0; i < count; i++)
			{
				AddItem.Execute(null);
			}
		}
	}
}