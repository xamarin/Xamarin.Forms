using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6742, "Memory leak in ListView with uneven rows on IOS", PlatformAffected.Default)]
	public partial class Issue6742 : TestContentPage
	{
		public Issue6742()
		{
#if APP
			InitializeComponent();
#endif
			BindingContext = viewModel = new ViewModelIssue6742();

		}
		protected override void Init()
		{

		}
		ViewModelIssue6742 viewModel;
		IList<WeakReference<ModelIssue6742>> _weakList = new List<WeakReference<ModelIssue6742>>();
		private void RefreshItems(object sender, EventArgs e)
		{
			foreach (var item in this.viewModel.ItemGroups)
			{
				item.Clear();
			}
			viewModel.ItemGroups.Clear();

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			CleanWeakList(_weakList);
			viewModel.DisposeString = $"{_weakList.Count} object(s) alive";

			string report = $"MEMORY:{GC.GetTotalMemory(true)}";
			viewModel.TotalMemory = report;

			var g1 = new ModelIssue6742Group("Group 1")
			{
				new ModelIssue6742("first", viewModel),
				new ModelIssue6742("second", viewModel),
			};

			_weakList.Add(new WeakReference<ModelIssue6742>(g1[0]));
			_weakList.Add(new WeakReference<ModelIssue6742>(g1[1]));

			viewModel.ItemGroups.Add(g1);

			var g2 = new ModelIssue6742Group("Group 2")
			{
				new ModelIssue6742("third", viewModel),
				new ModelIssue6742("fourth", viewModel),
			};

			_weakList.Add(new WeakReference<ModelIssue6742>(g2[0]));
			_weakList.Add(new WeakReference<ModelIssue6742>(g2[1]));

			viewModel.ItemGroups.Add(g2);

			var g3 = new ModelIssue6742Group("Group 3")
			{
				new ModelIssue6742("fifth", viewModel),
				new ModelIssue6742("sixth", viewModel),
				new ModelIssue6742("seventh", viewModel),
			};

			_weakList.Add(new WeakReference<ModelIssue6742>(g3[0]));
			_weakList.Add(new WeakReference<ModelIssue6742>(g3[1]));
			_weakList.Add(new WeakReference<ModelIssue6742>(g3[2]));

			viewModel.ItemGroups.Add(g3);
		}

		private void CleanWeakList(IList<WeakReference<ModelIssue6742>> weakList)
		{
			ModelIssue6742 item;
			for (int i = weakList.Count - 1; i >= 0; i--)
			{
				if (!weakList[i].TryGetTarget(out item))
				{
					weakList.RemoveAt(i);
				}
			}
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue6742 : INotifyPropertyChanged
	{
		public ObservableCollection<ModelIssue6742Group> ItemGroups { get; set; } = new ObservableCollection<ModelIssue6742Group>();
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		string _DisposeString;
		public string DisposeString
		{
			get { return _DisposeString; }
			set
			{
				_DisposeString = value;
				NotifyPropertyChanged();
			}
		}

		string _totalMemory;
		public string TotalMemory
		{
			get => _totalMemory;
			set
			{
				_totalMemory = value;
				NotifyPropertyChanged();
			}
		}

		public ViewModelIssue6742()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ModelIssue6742Group : ObservableCollection<ModelIssue6742>
	{
		public ModelIssue6742Group(string name)
		{
			GroupName = name;
		}
		public string GroupName { get; set; }

	}

	[Preserve(AllMembers = true)]
	public class ModelIssue6742
	{
		ViewModelIssue6742 _model;

		public ModelIssue6742(string name, ViewModelIssue6742 model)
		{
			_model = model;
			Name = name;
		}
		public string Name { get; set; }
	}
}
