using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.ComponentModel;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60033, "[UWP] ListView override void SetupContent not triggered", PlatformAffected.UWP)]
	public class Bugzilla60033 : TestContentPage
	{
		protected override void Init()
		{
			var items = new ObservableCollection<string>() { "A", "B", "C" };
			var lv = new Bugzilla60033ListView()
			{
				ItemsSource = items
			};
			var setupContentCallCount = new Label();
			
			Content = new StackLayout
			{
				Children =
				{
					new Label
					{
						Text = "The below count should start at 3 for each SetupContent call (as well as when reset), and increase when 'Add item' is clicked."
					},
					setupContentCallCount,
					lv,
					new Button
					{
						Text = "Add item",
						Command = new Command(() => {
							((ObservableCollection<string>)lv.ItemsSource).Add("Item");
							setupContentCallCount.Text = lv.SetupContentCallCount.ToString();
						})
					},
					new Button
					{
						Text = "Reset list",
						Command = new Command(() => {
							lv.SetupContentCallCount = 0;
							var newItems = new ObservableCollection<string>() { "A", "B", "C" };
							lv.ItemsSource = newItems;
							setupContentCallCount.Text = lv.SetupContentCallCount.ToString();
						})
					}
				}
			};

			Appearing += (s, e) =>
			{
				setupContentCallCount.Text = lv.SetupContentCallCount.ToString();
			};
		}

		public class Bugzilla60033ListView : ListView
		{
			public int SetupContentCallCount { get; set; }
			
			public Bugzilla60033ListView() : base()
			{
			}

			protected override void SetupContent(Cell cell, int index)
			{
				base.SetupContent(cell, index);
				SetupContentCallCount++;
			}
		}
	}
}