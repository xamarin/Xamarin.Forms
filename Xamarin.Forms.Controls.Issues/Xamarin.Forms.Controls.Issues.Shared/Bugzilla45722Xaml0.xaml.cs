using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 945722, "Memory leak in Xamarin Forms ListView", PlatformAffected.UWP)]
	public partial class Bugzilla45722Xaml0 : ContentPage
	{
		const int ItemCount = 50;
		const string Success = "Success";
		const string Running = "Running...";

		public Bugzilla45722Xaml0 ()
		{
			InitializeComponent ();

			Log.Listeners.Add(new DelegateLogListener((c, m) =>
			{
				if (c == nameof(_45722Label))
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						CurrentCount.Text = m;
						Result.Text = int.Parse(m) - ItemCount == 0 ? Success : Running;
					});
				}
			}));

			Model = new ObservableCollection<_45722Group>();

			RefreshModel();

			IsGroupingEnabled = true;
			BindingContext = this;

			RefreshButton.Clicked += (sender, args) => { RefreshModel(); };
		}

		public ObservableCollection<_45722Group> Model { get; }

		public bool IsGroupingEnabled { get; }

		async void RefreshModel()
		{
			Model.Clear();

			for (int n = 0; n < ItemCount; n++)
			{
				var group = new _45722Group($"{n}", new []
				{
					new _45722Item($"{n}-1", $"{n}-1 description"),
					new _45722Item($"{n}-2", $"{n}-2 description"),
					new _45722Item($"{n}-3", $"{n}-3 description")
				});

				Model.Add(group);
			}

			await Task.Delay(1000);

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}

	public class _45722Group : ObservableCollection<_45722Item>
	{
		public _45722Group(string key, IEnumerable<_45722Item> items) : base(items)
		{
			Key = key;
		}

		public string Key { get; set; }
	}

	public class _45722Item
	{
		public _45722Item(string listName, string listDescription)
		{
			ListName = listName;
			ListDescription = listDescription;
		}

	
		public string ListName { get; set; }
		public string ListDescription { get; set; }
	}
}