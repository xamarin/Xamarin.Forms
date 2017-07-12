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
	//[Category(UITestCategories.)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 945722, "Memory leak in Xamarin Forms ListView", PlatformAffected.All)]
	public class Bugzilla45722 : TestContentPage 
	{
		IEnumerable<string> CreateItems()
		{
			var r = new Random(DateTime.Now.Millisecond);
			for (int n = 0; n < 3; n++)
			{
				yield return r.NextDouble().ToString(CultureInfo.InvariantCulture);
			}
		}

		protected override void Init()
		{
			var lv = new ListView(ListViewCachingStrategy.RetainElement) {};

			var items = new ObservableCollection<string>();

			foreach (string item in CreateItems())
			{
				items.Add(item);
			}

			var dt = new DataTemplate(() =>
			{
				var label = new _45722Label();

				//Debug.WriteLine($">>>>> Bugzilla45722 DataTemplate created new Label {label.Id}");

				label.SetBinding(Label.TextProperty, new Binding("."));
				return 	new ViewCell { View = label };
			});

			lv.ItemsSource = items;
			lv.ItemTemplate = dt;

			var button = new Button { Text = "Update List" };
			var gc = new Button { Text = "GC" };

			button.Clicked += (sender, args) =>
			{
				items.Clear();
				foreach (string item in CreateItems())
				{
					items.Add(item);
				}
			};

			gc.Clicked += (sender, args) =>
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			};

			Content = new StackLayout
			{
				Padding = new Thickness(0, 20, 0, 0),
				Children =
				{
					button, gc, lv
				}
			};
		}

		// TODO hartez 2017/07/10 18:33:50 For starters, these never seem to be collected, even if you clear out the list and add new stuff	
		// We need to drop some debug messages into labelrenderer and see if those ever get disposed; my sense is they do not

		class _45722Label : Label
		{
			static int s_id;
			static int s_count;

			public int Id { get; }

			public _45722Label()
			{
				Id = s_id;
				Debug.WriteLine($">>>>> _45722Label constructor for {Id} - instance count is: {Interlocked.Increment(ref s_count)}");
				s_id += 1;
			}

			~_45722Label()
			{
				Debug.WriteLine($">>>>> _45722Label finalizer for {Id} - instance count is: {Interlocked.Decrement(ref s_count)}");
			}

			public override string ToString()
			{
				return $"_45722Label: {nameof(Id)}: {Id}";
			}
		}

#if UITEST
		//[Test]
		//public void _45722Test()
		//{
		//	//RunningApp.WaitForElement(q => q.Marked(""));
		//}
#endif
	}
}