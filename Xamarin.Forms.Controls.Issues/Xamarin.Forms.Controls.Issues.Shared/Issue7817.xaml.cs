using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System.Reflection;

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
	[Issue(IssueTracker.Github, 7817, "[Android/iOS] Changing ItemsUpdatingScrollMode has no effect on CarouselView")]
	public partial class Issue7817 : TestContentPage
	{
		public Issue7817()
		{
#if APP
			Device.SetFlags(new List<string> { CollectionView.CollectionViewExperimental });
			Title = "Issue 7817";
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue7817ViewModel();
		}

		void OnItemsUpdatingScrollModeChanged(object sender, EventArgs e)
		{
			carouselView.ItemsUpdatingScrollMode = (ItemsUpdatingScrollMode)(sender as EnumPicker).SelectedItem;
		}
	}

	[Preserve(AllMembers = true)]
	public class EnumPicker : Picker
	{
		public static readonly BindableProperty EnumTypeProperty = BindableProperty.Create(nameof(EnumType), typeof(Type), typeof(EnumPicker),
			propertyChanged: (bindable, oldValue, newValue) =>
			{
				var picker = (EnumPicker)bindable;

				if (oldValue != null)
				{
					picker.ItemsSource = null;
				}
				if (newValue != null)
				{
					if (!((Type)newValue).GetTypeInfo().IsEnum)
						throw new ArgumentException("EnumPicker: EnumType property must be enumeration type");

					picker.ItemsSource = Enum.GetValues((Type)newValue);
				}
			});

		public Type EnumType
		{
			set => SetValue(EnumTypeProperty, value);
			get => (Type)GetValue(EnumTypeProperty);
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue7817Model
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }
		public string Details { get; set; }
		public string ImageUrl { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue7817ViewModel : BindableObject
	{
		public Issue7817ViewModel()
		{
			CreateCollection();
		}

		public ObservableCollection<Issue7817Model> Monkeys { get; private set; } = new ObservableCollection<Issue7817Model>();

		void CreateCollection()
		{
			Monkeys.Add(new Issue7817Model
			{
				Index = 0,
				Name = "Baboon",
				Location = "Africa & Asia",
				Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
				ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"
			});

			Monkeys.Add(new Issue7817Model
			{
				Index = 1,
				Name = "Capuchin Monkey",
				Location = "Central & South America",
				Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
				ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg"
			});

			Monkeys.Add(new Issue7817Model
			{
				Index = 2,
				Name = "Blue Monkey",
				Location = "Central and East Africa",
				Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
				ImageUrl = "http://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg"
			});
		}
	}
}