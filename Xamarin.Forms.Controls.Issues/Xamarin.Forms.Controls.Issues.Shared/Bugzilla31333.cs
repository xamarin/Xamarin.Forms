using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;

#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 31333,
		"Focus() on Entry in ViewCell brings up keyboard, but doesn't have cursor in EditText", PlatformAffected.Android
	)]
	public class Bugzilla31333 : TestContentPage
	{
		[Preserve(AllMembers = true)]
		public class Model31333 : INotifyPropertyChanged
		{
			string _data;

			bool _isFocused = false;

			public string Data
			{
				get { return _data; }
				set
				{
					_data = value;
					OnPropertyChanged();
				}
			}

			public bool IsFocused
			{
				get { return _isFocused; }
				set
				{
					_isFocused = value;
					OnPropertyChanged();
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		[Preserve(AllMembers = true)]
		public interface IHaveControlFocusedProperty
		{
			void SetBinding();
		}

		[Preserve(AllMembers = true)]
		public class ExtendedEntry : Entry, IHaveControlFocusedProperty
		{
			public static readonly BindableProperty IsControlFocusedProperty =
				BindableProperty.Create("IsControlFocused", typeof(bool), typeof(ExtendedEntry), false);

			public bool IsControlFocused
			{
				get { return (bool)GetValue(IsControlFocusedProperty); }
				set { SetValue(IsControlFocusedProperty, value); }
			}

			public void SetBinding()
			{
				this.SetBinding(IsControlFocusedProperty, "IsFocused");
			}

			protected override void OnPropertyChanged(string propertyName = null)
			{
				base.OnPropertyChanged(propertyName);
				if (propertyName == IsControlFocusedProperty.PropertyName)
				{
					if (IsControlFocused)
					{
						Focus();
					}
					else
					{
						Unfocus();
					}
				}
			}
		}

		[Preserve(AllMembers = true)]
		public class ExtendedEditor : Editor, IHaveControlFocusedProperty
		{
			public static readonly BindableProperty IsControlFocusedProperty =
				BindableProperty.Create("IsControlFocused", typeof(bool), typeof(ExtendedEditor), false);

			public bool IsControlFocused
			{
				get { return (bool)GetValue(IsControlFocusedProperty); }
				set { SetValue(IsControlFocusedProperty, value); }
			}

			public void SetBinding()
			{
				this.SetBinding(IsControlFocusedProperty, "IsFocused");
			}

			protected override void OnPropertyChanged(string propertyName = null)
			{
				base.OnPropertyChanged(propertyName);
				if (propertyName == IsControlFocusedProperty.PropertyName)
				{
					if (IsControlFocused)
					{
						Focus();
					}
					else
					{
						Unfocus();
					}
				}
			}
		}

		[Preserve(AllMembers = true)]
		public class ExtendedCell<T> : ViewCell where T : View, IHaveControlFocusedProperty
		{
			public ExtendedCell()
			{
				var control = (T)Activator.CreateInstance(typeof(T));
				control.SetBinding();
				control.HorizontalOptions = LayoutOptions.FillAndExpand;

				View = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Children =
					{
						control
					}
				};
			}
		}

		StackLayout CreateListViewTestSection(Type controlType)
		{
			string name = controlType.GenericTypeArguments[0].Name;
			name = name.Replace("Extended", "");

			var button = new Button() { Text = $"Focus {name} in ListView" };

			var data = new ObservableCollection<Model31333> { new Model31333() };

			var listView = new ListView
			{
				VerticalOptions = LayoutOptions.Start,
				ItemsSource = data,
				ItemTemplate = new DataTemplate(controlType)
			};

			button.Clicked += (sender, args) =>
			{
				Model31333 item = data[0];
				if (item != null)
				{
					item.IsFocused = !item.IsFocused;
				}
			};

			return new StackLayout() { Children = { button, listView } };
		}

		StackLayout CreateTableViewTestSection<T>() where T : View, IHaveControlFocusedProperty
		{
			string name = typeof(T).Name;
			name = name.Replace("Extended", "");

			var button = new Button() { Text = $"Focus {name} in Table" };

			var data = new Model31333();

			var tableView = new TableView
			{
				VerticalOptions = LayoutOptions.Start
			};

			var tableRoot = new TableRoot();
			var tableSection = new TableSection();

			var cell = new ExtendedCell<T>();

			cell.BindingContext = data;

			tableSection.Add(cell);
			tableRoot.Add(tableSection);
			tableView.Root = tableRoot;

			button.Clicked += (sender, args) =>
			{
				Model31333 item = data;
				if (item != null)
				{
					item.IsFocused = !item.IsFocused;
				}
			};

			return new StackLayout() { Children = { button, tableView } };
		}

		protected override void Init()
		{
			StackLayout entrySection = CreateListViewTestSection(typeof(ExtendedCell<ExtendedEntry>));
			StackLayout editorSection = CreateListViewTestSection(typeof(ExtendedCell<ExtendedEditor>));

			StackLayout entryTableSection = CreateTableViewTestSection<ExtendedEntry>();
			StackLayout editorTableSection = CreateTableViewTestSection<ExtendedEditor>();

			Content = new StackLayout()
			{
				Children = { entrySection, editorSection, entryTableSection, editorTableSection }
			};
		}

#if UITEST
		[Test]
		[UiTest (typeof(NavigationPage))]
		public void Issue31333FocusEntryInListViewCell ()
		{
			RunningApp.Tap (q => q.Marked ("Focus Entry in ListView"));
			RunningApp.Screenshot ("Entry control in ListView cell is focused");
			RunningApp.EnterText ("Entry in ListView Success");
			Assert.True(RunningApp.Query(query => query.Text("Entry in ListView Success")).Length > 0);
			RunningApp.Screenshot ("Entry in ListView Success");
			RunningApp.Tap(q => q.Marked("Focus Entry in ListView"));
		}

		[Test]
		[UiTest (typeof(NavigationPage))]
		public void Issue31333FocusEditorInListViewCell ()
		{
			RunningApp.Tap (q => q.Marked ("Focus Editor in ListView"));
			RunningApp.Screenshot ("Editor control in ListView cell is focused");
			RunningApp.EnterText ("Editor in ListView Success");
			Assert.True(RunningApp.Query(query => query.Text("Editor in ListView Success")).Length > 0);
			RunningApp.Screenshot ("Editor in ListView Success");
			RunningApp.Tap(q => q.Marked("Focus Editor in ListView"));
		}

		
		[Test]
		[UiTest (typeof(NavigationPage))]
		public void Issue31333FocusEntryInTableViewCell ()
		{
			RunningApp.Tap (q => q.Marked ("Focus Entry in Table"));
			RunningApp.Screenshot ("Entry control in TableView cell is focused");
			RunningApp.EnterText ("Entry in TableView Success");
			Assert.True(RunningApp.Query(query => query.Text("Entry in TableView Success")).Length > 0);
			RunningApp.Screenshot ("Entry in TableView Success");
			RunningApp.Tap(q => q.Marked("Focus Entry in Table"));
		}

		[Test]
		[UiTest (typeof(NavigationPage))]
		public void Issue31333FocusEditorInTableViewCell ()
		{
			RunningApp.Tap (q => q.Marked ("Focus Editor in Table"));
			RunningApp.Screenshot ("Editor control in TableView cell is focused");
			RunningApp.EnterText ("Editor in TableView Success");
			Assert.True(RunningApp.Query(query => query.Text("Editor in TableView Success")).Length > 0);
			RunningApp.Screenshot ("Editor in TableView Success");
			RunningApp.Tap(q => q.Marked("Focus Editor in Table"));
		}
#endif
	}
}