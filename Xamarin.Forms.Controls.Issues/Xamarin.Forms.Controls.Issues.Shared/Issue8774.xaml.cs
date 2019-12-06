using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8774, "SwipeView in CollectionView selection", PlatformAffected.Android)]
	public partial class Issue8774 : TestContentPage
	{
		public Issue8774()
		{
#if APP
			Title = "Issue 8774";
			InitializeComponent();
			BindingContext = new Issue8774ViewModel();
#endif
		}

		protected override void Init()
		{

		}

#if APP
		void SwipeSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			DisplayAlert("SelectionChanged", "SelectionChanged Invoked", "Ok");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class Issue8774Model
	{
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string Description { get; set; }
		public string Date { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue8774ViewModel : BindableObject
	{
		private ObservableCollection<Issue8774Model> _messages;

		public Issue8774ViewModel()
		{
			Messages = new ObservableCollection<Issue8774Model>();
			LoadMessages();
		}

		public ObservableCollection<Issue8774Model> Messages
		{
			get { return _messages; }
			set
			{
				_messages = value;
				OnPropertyChanged();
			}
		}

		private void LoadMessages()
		{
			for (int i = 0; i < 100; i++)
			{
				Messages.Add(new Issue8774Model { Title = $"Lorem ipsum {i + 1}", SubTitle = "Lorem ipsum dolor sit amet", Date = "Yesterday", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." });
			}
		}
	}
}