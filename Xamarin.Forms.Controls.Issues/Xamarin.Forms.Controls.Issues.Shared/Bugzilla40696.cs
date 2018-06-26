using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40696, "Cannot add items to start of CarouselView dynamically and Scroll", PlatformAffected.Android)]
	public class Bugzilla40696 : TestContentPage
	{
		protected override void Init()
		{
			BindingContext = new CarouselViewModel();

			var carouselView = new CarouselView
			{
				HeightRequest = 350,
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, ".");
					return label;
				})
			};
			carouselView.SetBinding(CarouselView.PositionProperty, nameof(CarouselViewModel.Position));
			carouselView.SetBinding(CarouselView.ItemsSourceProperty, nameof(CarouselViewModel.Items));

			var posLabel = new Label();
			posLabel.SetBinding(Label.TextProperty, nameof(CarouselViewModel.SelectedDay));

			Content = new StackLayout { Children = { carouselView, posLabel, new Label { Text = "Swipe the CarouselView to the right and left. If the application freezes, this test has failed." } } };
		}

		class CarouselViewModel : ViewModelBase
		{
			int _position;
			public int Position
			{
				get { return _position; }
				set
				{
					System.Diagnostics.Debug.WriteLine("Position");

					_position = value;
					SelectedDay = Items.ElementAt(value);
					OnPropertyChanged(nameof(Position));
					if (value == Items.Count - 2)
					{
						AddExtraDaysToList(SelectedDay.AddDays(2), false);
					}
					else if (value == 1)
					{
						AddExtraDaysToList(SelectedDay.AddDays(-2), true);
					}

				}
			}

			ObservableCollection<DateTime> _items;
			public ObservableCollection<DateTime> Items
			{
				get { return _items; }
				set
				{
					_items = value;
					OnPropertyChanged(nameof(Items));
				}
			}

			DateTime _selectedDay;
			public DateTime SelectedDay
			{
				get { return _selectedDay; }
				set
				{
					_selectedDay = value;
					OnPropertyChanged(nameof(SelectedDay));
				}
			}

			public CarouselViewModel()
			{
				var CurrentDate = DateTime.Today;

				Items = new ObservableCollection<DateTime>();
				int count = 1;
				for (int i = -count; i <= count; i++)
				{
					Items.Add(CurrentDate.AddDays(i));
				}
				Position = count + 1;
			}

			void AddExtraDaysToList(DateTime date, bool addToStart)
			{
				if (!Items.Contains(date.Date))
				{
					int count = 7;
					if (!addToStart)
					{
						for (int i = 0; i < count; i++)
						{
							var day = date.AddDays(i);
							Items.Add(day);
						}
					}
					else
					{
						//TODO: adding to the start crashes the application
						for (int i = 0; i >= -count; i++)
						{
							var day = date.AddDays(i);
							Items.Insert(0, day);
						}
					}
				}

			}
		}
	}
}