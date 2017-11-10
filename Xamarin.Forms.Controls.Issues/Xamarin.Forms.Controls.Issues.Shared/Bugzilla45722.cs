using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ListView)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 45722, "Memory leak in Xamarin Forms ListView", PlatformAffected.UWP)]
	public class Bugzilla45722 : TestNavigationPage
	{
		const string Success = "Success";
		const string Running = "Running...";
		const string Update = "Update List";

		const int ItemCount = 30;

		Label _currentLabelCount;
		Label _statusLabel;

		[Preserve(AllMembers = true)]
		public class _45722Model : INotifyPropertyChanged
		{
			string _text;

			public event PropertyChangedEventHandler PropertyChanged;

			protected virtual void OnPropertyChanged1([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			public _45722Model(string text)
			{
				_text = text;

				Command = new Command(() => Debug.WriteLine($">>>>> _45722Model Command Running"));
			}

			public string Text
			{
				get { return _text; }
				set
				{
					_text = value;
					OnPropertyChanged1();
				}
			}

			public Command Command { get; }
		}

		static IEnumerable<_45722Model> CreateItems()
		{
			var r = new Random(DateTime.Now.Millisecond);
			for (int n = 0; n < ItemCount; n++)
			{
				yield return new _45722Model(r.NextDouble().ToString(CultureInfo.InvariantCulture));
			}
		}

		ContentPage ListPage()
		{
			var lv = new ListView(ListViewCachingStrategy.RetainElement);

			var items = new ObservableCollection<_45722Model>();

			foreach (var item in CreateItems())
			{
				items.Add(item);
			}

			var dt = new DataTemplate(() =>
			{
				var layout = new Grid();

				var label = new _45722Label();
				label.SetBinding(Label.TextProperty, new Binding("Text"));

				var bt = new Button { Text = "Go" };
				bt.SetBinding(Button.CommandProperty, new Binding("Command"));

				var en = new Entry { Text = "entry" };

				layout.Children.Add(bt);
				layout.Children.Add(en);
				layout.Children.Add(label);

				Grid.SetRow(bt, 1);
				Grid.SetRow(en, 2);

				return new ViewCell { View = layout };
			});

			lv.ItemsSource = items;
			lv.ItemTemplate = dt;

			var button = new Button { Text = Update };
			button.Clicked += async (sender, args) =>
			{
				items.Clear();
				foreach (var item in CreateItems())
				{
					items.Add(item);
				}

				await Task.Delay(1000);

				GC.Collect();
				GC.WaitForPendingFinalizers();
			};

			return new ContentPage
			{
				Content = new StackLayout
				{
					Padding = new Thickness(0, 20, 0, 0),
					Children = { _currentLabelCount, _statusLabel, button, lv }
				}
			};
		}

		protected override void Init()
		{
			_currentLabelCount = new Label();
			_statusLabel = new Label { Text = Running };
			
			Log.Listeners.Add(new DelegateLogListener((c, m) =>
			{
				if (c == nameof(_45722Label))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						_currentLabelCount.Text = m;
						_statusLabel.Text = int.Parse(m) - ItemCount == 0 ? Success : Running;
					});
				}
			}));

			PushAsync(ListPage());
		}

#if UITEST && __WINDOWS__
		[Test]
		public void LabelsInListViewTemplatesShouldBeCollected()
		{
			for(int n = 0; n < 10; n++)
			{
				RunningApp.Tap(Update);
			}

			Task.Delay(1500).Wait();

			RunningApp.WaitForElement(Success);
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class _45722Label : Label
	{
		static int s_id;
		static int s_count;

		int Id { get; }

		public _45722Label()
		{
			Id = s_id;
			Interlocked.Increment(ref s_count);
			Log.Warning(nameof(_45722Label), $"{s_count}");
			s_id += 1;
		}

		~_45722Label()
		{
			Interlocked.Decrement(ref s_count);
			Log.Warning(nameof(_45722Label), $"{s_count}");
		}

		public override string ToString()
		{
			return $"{nameof(_45722Label)}: {nameof(Id)}: {Id}";
		}
	}
}