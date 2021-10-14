using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13268,
		"[Bug] [iOS] CollectionView iOS inner crash while adding items to group, items aren't displayed",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CollectionView)]
#endif
	public class Issue13268 : TestContentPage
	{
		private string _success = "Guardian-1";

		ObservableCollection<ObservableTeam> _itemsSource;

		protected override void Init()
		{
			_itemsSource = new ObservableCollection<ObservableTeam>();
			var cv = new CollectionView
			{
				IsGrouped = true,
				ItemTemplate = ItemTemplate(),
				ItemsSource = _itemsSource
			};

			Content = cv;

			Appearing += (sender, args) =>
			{
				var groupPrep = new ObservableTeam("Extra Excalibur", new List<Member>(new[] { new Member("hello"), new Member("it's"), new Member("me") }));
				_itemsSource.Add(groupPrep);

				var itemsToAdd = Enumerable.Range(0, 20).Select(x => new Member($"Guardian-{x}")).ToArray();
				_success = itemsToAdd.Skip(1).Take(1).First().Name;
				var group = new ObservableTeam("Excalibur", new List<Member>(0));
				_itemsSource.Clear();
				_itemsSource.Add(group);
				foreach (var item in itemsToAdd)
					group.Add(item);
			};
		}

		DataTemplate ItemTemplate()
		{
			return new DataTemplate(() =>
			{
				var layout = new StackLayout() { Padding = 10 };
				var label1 = new Label()
				{
					FontSize = 16,
					LineBreakMode = LineBreakMode.NoWrap,
					Margin = new Thickness(5, 0, 0, 0),
				};
				label1.SetBinding(Label.TextProperty, new Binding(nameof(Member.Name)));
				var label2 = new Label()
				{
					FontSize = 16,
					LineBreakMode = LineBreakMode.NoWrap,
					Margin = new Thickness(5, 0, 0, 0),
				};
				label2.SetBinding(Label.TextProperty, new Binding(nameof(Member.Title)));

				layout.Children.Add(label1);
				layout.Children.Add(label2);

				return layout;
			});
		}

		[Preserve(AllMembers = true)]
		class ObservableTeam : ObservableCollection<Member>
		{
			public ObservableTeam(string name, List<Member> members) : base(members)
			{
				Name = name;
			}

			public string Name { get; set; }

			public override string ToString()
			{
				return Name;
			}
		}

		[Preserve(AllMembers = true)]
		class Member
		{
			public Member(string name)
			{
				Name = name;
				Title = Guid.NewGuid().ToString("D");
			}

			public string Name { get; private set; }

			public string Title { get; private set; }
		}

#if UITEST
		[Test]
		public void CollectionShouldInvalidateOnVisibilityChange()
		{
			RunningApp.WaitForElement(_success);
		}
#endif
	}
}

