using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12518,
		"[Bug] SwipeView is too sensitive - opens when scrolling",
		PlatformAffected.iOS)]
	public partial class Issue12518 : TestContentPage
	{
		public Issue12518()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue12518ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue12518Model
	{
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string Description { get; set; }
		public string Date { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue12518ViewModel : BindableObject
	{
		ObservableCollection<Issue12518Model> _items;
		string _message;

		public Issue12518ViewModel()
		{
			Items = new ObservableCollection<Issue12518Model>();
			LoadItems();
		}

		public ObservableCollection<Issue12518Model> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				OnPropertyChanged();
			}
		}

		public ICommand FavouriteCommand => new Command(OnFavourite);
		public ICommand DeleteCommand => new Command(OnDelete);
		public ICommand TapCommand => new Command(OnTap);

		void LoadItems()
		{
			for (int i = 0; i < 100; i++)
			{
				Items.Add(new Issue12518Model { Title = $"Lorem ipsum {i + 1}", SubTitle = "Lorem ipsum dolor sit amet", Date = "Yesterday", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." });
			}
		}

		void OnFavourite() => Message = "Favourite";

		void OnDelete() => Message = "Delete";

		void OnTap() => Message = "Tap";
	}
}