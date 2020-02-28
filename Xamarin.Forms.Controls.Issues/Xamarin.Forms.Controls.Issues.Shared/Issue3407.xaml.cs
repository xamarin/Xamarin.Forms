using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3407, "ListView's GroupHeaderTemplate do not work with DataTemplateSelector on Android")]
	public partial class Issue3407 : TestContentPage
	{
#if UITEST
		[Test]
		public void Issue3407Test()
		{
			RunningApp.WaitForElement(q => q.Marked("list1"));
			RunningApp.WaitForElement(q => q.Marked("list2"));
		}
#endif

		PersonViewModel ViewModel
		{
			get { return BindingContext as PersonViewModel; }
		}

		public Issue3407()
		{

#if APP
			InitializeComponent();
			BindingContext = new PersonViewModel();
#endif
		}

		protected override void Init()
		{
		}


		[Preserve(AllMembers = true)]
		public class PersonViewModel : BaseViewModelF
		{
			public ObservableCollection<object> PersonsList { get; set; }

			public PersonViewModel()
			{
				var sList = new PersonList()
				{
					new Person("Sally"),
					new Person("Sampson"),
					new Person("Swift" ),
					new Person("Smith" ),
					new Person("Sally"),
					new Person("Sampson"),
					new Person("Swift" ),
					new Person("Smith" ),
					new Person("Sally"),
					new Person("Sampson"),
					new Person("Swift" ),
					new Person("Smith" ),
					new Person("Sally"),
					new Person("Sampson"),
					new Person("Swift" ),
					new Person("Smith" ),
					new Person("Sally"),
					new Person("Sampson"),
					new Person("Swift" ),
					new Person("Smith" ),
				};
				sList.GroupTitle = "S";

				var xList = new DifferentPersonList()
				{
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),
					new DifferentPerson("X" ),

				};
				xList.GroupTitle = "X";

				var dList = new PersonList()
				{
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
					new Person("Daniel"),
					new Person("Doe" ),
				};
				dList.GroupTitle = "D";

				var jList = new DifferentPersonList()
				{
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),
					new DifferentPerson("Jane" ),
					new DifferentPerson("Jon" ),

				};
				jList.GroupTitle = "J";

				PersonsList = new ObservableCollection<object> { sList, xList, dList, jList};
			}
		}

		void Button_OnClicked(object sender, EventArgs e)
		{
			if(list1.IsVisible)
			{
				list1.IsVisible = false;
				list2.IsVisible = false;

				collection1.IsVisible = true;
				collection2.IsVisible = true;
			}
			else
			{
				collection1.IsVisible = false;
				collection2.IsVisible = false;

				list1.IsVisible = true;
				list2.IsVisible = true;
			}
		}
	}

	public class DifferentPerson : Person
	{
		public DifferentPerson(string name) : base(name)
		{

		}

	}

	public class PersonList : List<Person>
	{
		public string GroupTitle { get; set; }
		public List<Person> Persons => this;
	}

	public class DifferentPersonList : List<DifferentPerson>
	{
		public string GroupTitle { get; set; }
		public List<DifferentPerson> Persons => this;
	}

	#region ListView
	public class Issue3407ItemDataTemplateSelector : DataTemplateSelector
	{
		public Issue3407ItemDataTemplateSelector()
		{
			ItemTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForPeople));
			DifferentItemTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForDifferentPeople));
		}

		public DataTemplate ItemTemplate { get;  }
		public DataTemplate DifferentItemTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPerson))
				return DifferentItemTemplate;
			
			return ItemTemplate;
		}
	}

	public class Issue3407GroupDataTemplateSelector : DataTemplateSelector
	{
		public Issue3407GroupDataTemplateSelector()
		{
			GroupTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForGroup));
			DifferentGroupTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForDifferentGroup));
		}

		public DataTemplate GroupTemplate { get; }
		public DataTemplate DifferentGroupTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPersonList))
				return DifferentGroupTemplate;

			return GroupTemplate;
		}
	}

	public class Issue3407GroupDataTemplateSelectorForAllTypes : DataTemplateSelector
	{
		public Issue3407GroupDataTemplateSelectorForAllTypes()
		{
			GroupTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForGroup));
			DifferentGroupTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForDifferentGroup));
			ItemTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForPeople));
			DifferentItemTemplate = new DataTemplate(typeof(Issue3407MyItemTemplateForDifferentPeople));
		}

		public DataTemplate ItemTemplate { get; }
		public DataTemplate DifferentItemTemplate { get; }
		public DataTemplate GroupTemplate { get; }
		public DataTemplate DifferentGroupTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPersonList))
				return DifferentGroupTemplate;
		
			if (item.GetType() == typeof(DifferentPerson))
				return DifferentItemTemplate;
			
			if (item.GetType() == typeof(PersonList))
				return GroupTemplate;
			
			if (item.GetType() == typeof(Person))
				return ItemTemplate;

			return null;
		}
	}

	public class Issue3407MyItemTemplateForPeople : ViewCell
	{
		public Issue3407MyItemTemplateForPeople()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.White
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "Name");
			grid.Children.Add(label);
			View = grid;
		}
	}

	public class Issue3407MyItemTemplateForDifferentPeople : ViewCell
	{
		public Issue3407MyItemTemplateForDifferentPeople()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.Gold
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "Name");
			grid.Children.Add(label);
			View = grid;
		}
	}

	public class Issue3407MyItemTemplateForGroup : ViewCell
	{
		public Issue3407MyItemTemplateForGroup()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.LightBlue
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "GroupTitle");
			grid.Children.Add(label);
			View = grid;
		}
	}

	public class Issue3407MyItemTemplateForDifferentGroup : ViewCell
	{
		public Issue3407MyItemTemplateForDifferentGroup()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.Red
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "GroupTitle");
			grid.Children.Add(label);
			View = grid;
		}
	}

	#endregion

	#region CollectionView
	public class Issue3407CollectionItemDataTemplateSelector : DataTemplateSelector
	{
		public Issue3407CollectionItemDataTemplateSelector()
		{
			ItemTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForPeople));
			DifferentItemTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForDifferentPeople));
		}

		public DataTemplate ItemTemplate { get; }
		public DataTemplate DifferentItemTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPerson))
				return DifferentItemTemplate;

			return ItemTemplate;
		}
	}

	public class Issue3407CollectionGroupDataTemplateSelector : DataTemplateSelector
	{
		public Issue3407CollectionGroupDataTemplateSelector()
		{
			GroupTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForGroup));
			DifferentGroupTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForDifferentGroup));
		}

		public DataTemplate GroupTemplate { get; }
		public DataTemplate DifferentGroupTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPersonList))
				return DifferentGroupTemplate;

			return GroupTemplate;
		}
	}

	public class Issue3407CollectionGroupDataTemplateSelectorForAllTypes : DataTemplateSelector
	{
		public Issue3407CollectionGroupDataTemplateSelectorForAllTypes()
		{
			GroupTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForGroup));
			DifferentGroupTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForDifferentGroup));
			ItemTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForPeople));
			DifferentItemTemplate = new DataTemplate(typeof(Issue3407CollectionMyItemTemplateForDifferentPeople));
		}

		public DataTemplate ItemTemplate { get; }
		public DataTemplate DifferentItemTemplate { get; }
		public DataTemplate GroupTemplate { get; }
		public DataTemplate DifferentGroupTemplate { get; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item.GetType() == typeof(DifferentPersonList))
				return DifferentGroupTemplate;

			if (item.GetType() == typeof(DifferentPerson))
				return DifferentItemTemplate;

			if (item.GetType() == typeof(PersonList))
				return GroupTemplate;

			if (item.GetType() == typeof(Person))
				return ItemTemplate;

			return null;
		}
	}

	public class Issue3407CollectionMyItemTemplateForPeople : ContentView
	{
		public Issue3407CollectionMyItemTemplateForPeople()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.White
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "Name");
			grid.Children.Add(label);
			Content = grid;
		}
	}

	public class Issue3407CollectionMyItemTemplateForDifferentPeople : ContentView
	{
		public Issue3407CollectionMyItemTemplateForDifferentPeople()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.Gold
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "Name");
			grid.Children.Add(label);
			Content = grid;
		}
	}

	public class Issue3407CollectionMyItemTemplateForGroup : ContentView
	{
		public Issue3407CollectionMyItemTemplateForGroup()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.LightBlue
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "GroupTitle");
			grid.Children.Add(label);
			Content = grid;
		}
	}

	public class Issue3407CollectionMyItemTemplateForDifferentGroup : ContentView
	{
		public Issue3407CollectionMyItemTemplateForDifferentGroup()
		{
			var grid = new Grid
			{
				Padding = 10,
				BackgroundColor = Color.Red
			};

			var label = new Label();
			label.SetBinding(Label.TextProperty, "GroupTitle");
			grid.Children.Add(label);
			Content = grid;
		}
	}

	#endregion
}

