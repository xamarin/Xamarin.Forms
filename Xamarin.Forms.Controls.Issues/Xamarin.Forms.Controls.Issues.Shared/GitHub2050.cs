using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2050, "SelectedItemColor in ListView", PlatformAffected.Default)]
	public class Issue2050 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		[Preserve(AllMembers = true)]
		public class Selector : DataTemplateSelector
		{
			[Preserve(AllMembers = true)]
			public static class Cell
			{
				public static Color color = Color.Blue;

				[Preserve(AllMembers = true)]
				public class Text : TextCell
				{
					public Text()
					{
						SetBinding(TextProperty, new Binding("."));
						SelectedItemBackgroundColor = color;
						Tapped += (object sender, System.EventArgs e) => {
							SelectedItemBackgroundColor = Color.Green;
						};
					}
				}
				[Preserve(AllMembers = true)]
				public class Switch : SwitchCell
				{
					public Switch()
					{
						SetBinding(OnProperty, new Binding("."));
						SelectedItemBackgroundColor = color;
					}
				}
				[Preserve(AllMembers = true)]
				public class Image : ImageCell
				{
					public Image()
					{
						SetBinding(ImageSourceProperty, new Binding("."));
						SelectedItemBackgroundColor = color;
					}
				}
				[Preserve(AllMembers = true)]
				public class Entry : EntryCell
				{
					public Entry()
					{
						Text = "Entry: ";
						SetBinding(TextProperty, new Binding("."));
						SelectedItemBackgroundColor = color;
					}
				}
				[Preserve(AllMembers = true)]
				public class Any : ViewCell
				{
					public Any()
					{
						View = new Label() { Text = "ViewCell" };
						SelectedItemBackgroundColor = color;
					}
				}
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				if (item is string)
					return new DataTemplate(typeof(Cell.Text));

				if (item is bool)
					return new DataTemplate(typeof(Cell.Switch));

				if (item is FileImageSource)
					return new DataTemplate(typeof(Cell.Image));

				if (item is int)
					return new DataTemplate(typeof(Cell.Entry));

				if (item is View)
					return new DataTemplate(typeof(Cell.Any));

				throw new System.Exception();
			}
		}

		[Preserve(AllMembers = true)]
		public class ListOfCells : ListView
		{
			public ListOfCells() 
				: base(ListViewCachingStrategy.RecycleElementAndDataTemplate)
			{
				ItemsSource = new object[] {
					"Text",
					true,
					new FileImageSource { File = "crimson.jpg" },
					0,
					new Label() { Text = "ViewCell" }
				};
				ItemTemplate = new Selector();
			}
		}

		protected override void Init()
		{
			Content = new ListOfCells();
		}

#if UITEST
		[Test]
		public void Issue1Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot ("I am at Issue 1");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}
}