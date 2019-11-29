using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
	[Category(UITestCategories.CarouselView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8525, "[Bug] CarouselView not working in ios application", PlatformAffected.iOS)]
	public partial class Issue8525 : TestContentPage
	{
		public Issue8525()
		{
#if APP
			Title = "Issue 8525";
			InitializeComponent();
			BindingContext = new Issue8525ViewModel();
#endif
		}

		protected override void Init()
		{

		}

		void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			var peekAreaInsets = e.NewValue;
			CarouselView.PeekAreaInsets = new Thickness(peekAreaInsets);
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue8525Model
	{
		public Color Color { get; set; }
		public string Name { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue8525ViewModel : BindableObject
	{
		ObservableCollection<Issue8525Model> _items;

		public Issue8525ViewModel()
		{
			LoadItems();
		}

		public ObservableCollection<Issue8525Model> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public void LoadItems()
		{
			Items = new ObservableCollection<Issue8525Model>();

			var random = new Random();
			var items = new List<Issue8525Model>();

			for (int n = 0; n < 5; n++)
			{
				items.Add(new Issue8525Model
				{
					Color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
					Name = $"{n + 1}"
				});
			}

			_items = new ObservableCollection<Issue8525Model>(items);
			OnPropertyChanged(nameof(Items));
		}
	}
}