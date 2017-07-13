using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
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
	[Category(UITestCategories.ListView)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 45722, "Memory leak in Xamarin Forms ListView", PlatformAffected.UWP)]
	public class Bugzilla45722 : TestNavigationPage
	{
		const string Success = "Success";
		const string Running = "Running...";
		const string Push = "Push List Page";
		const string GarbageCollect = "GC";
		const string Update = "Update List";

		Label _currentLabelCount;
		Label _statusLabel;

		static IEnumerable<string> CreateItems()
		{
			var r = new Random(DateTime.Now.Millisecond);
			for (int n = 0; n < 30; n++)
			{
				yield return r.NextDouble().ToString(CultureInfo.InvariantCulture);
			}
		}

		static ContentPage ListPage()
		{
			var lv = new ListView(ListViewCachingStrategy.RetainElement) { };

			var items = new ObservableCollection<string>();

			foreach (string item in CreateItems())
			{
				items.Add(item);
			}

			var dt = new DataTemplate(() =>
			{
				var label = new _45722Label();
				label.SetBinding(Label.TextProperty, new Binding("."));
				return new ViewCell { View = label };
			});

			lv.ItemsSource = items;
			lv.ItemTemplate = dt;

			var button = new Button { Text = Update };
			button.Clicked += (sender, args) =>
			{
				items.Clear();
				foreach (string item in CreateItems())
				{
					items.Add(item);
				}
			};

			return new ContentPage
			{
				Content = new StackLayout
				{
					Padding = new Thickness(0, 20, 0, 0),
					Children = { button, lv }
				}
			};
		}

		ContentPage Root()
		{
			var button = new Button { Text = Push  };
			var gc = new Button { Text = GarbageCollect };

			button.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(Intermediate());
				await Navigation.PushAsync(ListPage());
			};

			gc.Clicked += (sender, args) =>
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			};

			return new ContentPage { Content = new StackLayout
			{
				Children = { _currentLabelCount, _statusLabel, button, gc }
			} };
		}

		static ContentPage Intermediate()
		{
			return new ContentPage { Content = new Label { Text = "Nothing to see here" } };
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
						if (int.Parse(m) == 0)
						{
							_statusLabel.Text = Success;
						}
						else
						{
							_statusLabel.Text = Running;
						}
					});
				}
			}));

			PushAsync(Root());
		}

		class _45722Label : Label
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

#if UITEST
		[Test]
		public void LabelsInListViewTemplatesShouldBeCollected()
		{
			RunningApp.WaitForElement(Push);
			RunningApp.Tap(Push);

			for(int n = 0; n < 10; n++)
			{
				RunningApp.Tap(Update);
			}

			RunningApp.Back();
			RunningApp.Back();

			RunningApp.Tap(GarbageCollect);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}