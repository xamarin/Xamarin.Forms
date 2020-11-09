using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Linq;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CarouselView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12574, "CarouselView Loop=True default freezes iOS app", PlatformAffected.Default)]
	public class Issue12574 : TestContentPage
	{
		ViewModelIssue12574 viewModel;
		CarouselView carouselView;
		string automationId = "carouselView";

		protected override void Init()
		{
			// Initialize ui here instead of ctor
			carouselView = new CarouselView
			{
				AutomationId = automationId,
				Margin = new Thickness(30),
				BackgroundColor = Color.Yellow,
				ItemTemplate = new DataTemplate(() =>
				{

					var stacklayout = new StackLayout();
					var labelId = new Label();
					var labelText = new Label();
					var labelDescription = new Label();
					labelId.SetBinding(Label.TextProperty, "Id");
					labelText.SetBinding(Label.TextProperty, "Text");
					labelDescription.SetBinding(Label.TextProperty, "Description");

					stacklayout.Children.Add(labelId);
					stacklayout.Children.Add(labelText);
					stacklayout.Children.Add(labelDescription);
					return stacklayout;
				})
			};
			carouselView.SetBinding(CarouselView.ItemsSourceProperty, "Items");
			this.SetBinding(Page.TitleProperty, "Title");
			BindingContext = viewModel = new ViewModelIssue12574();
			Content = carouselView;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			viewModel.OnAppearing();
		}

#if UITEST
		[Test]
		public void Issue12574Test()
		{
			RunningApp.WaitForElement("0 item");
			var rect = RunningApp.Query(c => c.Marked(automationId)).First().Rect;
			var centerX = rect.CenterX;
			var rightX = rect.X - 5;
			RunningApp.DragCoordinates(centerX + 40, rect.CenterY, rightX, rect.CenterY);

			RunningApp.WaitForElement("1 item");

			RunningApp.DragCoordinates(centerX + 40, rect.CenterY, rightX, rect.CenterY);

			RunningApp.WaitForElement("2 item");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	class ViewModelIssue12574 : BaseViewModel1
	{
		public ObservableCollection<ModelIssue12574> Items { get; set; }
		public Command LoadItemsCommand { get; set; }

		public ViewModelIssue12574()
		{
			Title = "CarouselView Looping";
			Items = new ObservableCollection<ModelIssue12574>();
			LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());
		}

		void ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			try
			{
				Items.Clear();
				for (int i = 0; i < 3; i++)
				{
					Items.Add(new ModelIssue12574 { Id = Guid.NewGuid().ToString(), Text = $"{i} item", Description = "This is an item description." });
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public void OnAppearing()
		{
			IsBusy = true;
			LoadItemsCommand.Execute(null);
		}
	}

	[Preserve(AllMembers = true)]
	class ModelIssue12574
	{
		public string Id { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }
	}

	class BaseViewModel1 : INotifyPropertyChanged
	{
		public string Title { get; set; }
		public bool IsInitialized { get; set; }

		bool _isBusy;

		/// <summary>
		/// Gets or sets if VM is busy working
		/// </summary>
		public bool IsBusy
		{
			get { return _isBusy; }
			set { _isBusy = value; OnPropertyChanged("IsBusy"); }
		}

		//INotifyPropertyChanged Implementation
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}