using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9833, "[Bug] [UWP] Propagate CollectionView BindingContext to EmptyView",	
		PlatformAffected.UWP)]
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	public class Issue9833 : TestContentPage
	{
		public Issue9833()
		{
			Title = "Issue 9833";
			
			BindingContext = new Issue9833ViewModel();

			var layout = new StackLayout();

			var instructions = new Label
			{
				Text = "If can execute the command from the EmptyView, the test has passed.",
				BackgroundColor = Color.Black,
				TextColor = Color.White
			};

			var collectionView = new CollectionView();
			collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Model.Items");

			var emptyView = new StackLayout
			{
				BackgroundColor = Color.LightGray
			};

			var emptyButton = new Button
			{
				Text = "Execute Command (EmptyView)"
			};

			emptyButton.SetBinding(Button.CommandProperty, "Model.Command");

			emptyView.Children.Add(emptyButton);

			collectionView.EmptyView = emptyView;

			layout.Children.Add(instructions);
			layout.Children.Add(collectionView);

			Content = layout;
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue9833Model
	{
		int _counter;

		public Issue9833Model()
		{
			Command = new Command(Execute);
		}

		public ObservableCollection<string> Items { get; set; }
		public ICommand Command  { get; set; }

		void Execute()
		{
			_counter++;
			Console.WriteLine($"Issue9833: {_counter}");
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue9833ViewModel : BindableObject
	{
		Issue9833Model _model;

		public Issue9833ViewModel()
		{
			LoadData();
		}

		public Issue9833Model Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged();
			}
		}

		void LoadData()
		{
			Model = new Issue9833Model();
		}
	}
}
