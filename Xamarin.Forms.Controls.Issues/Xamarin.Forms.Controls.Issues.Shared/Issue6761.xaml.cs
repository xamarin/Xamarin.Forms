using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6761, "Memory leak in ListView with group headers on IOS", PlatformAffected.Default)]
	public partial class Issue6761 : TestContentPage
	{
		public Issue6761()
		{
#if APP
			InitializeComponent();
#endif
			BindingContext = viewModel = new ViewModelIssue6761();

		}
		protected override void Init()
		{

		}
		int _count;
		ViewModelIssue6761 viewModel;
		IList<WeakReference<ModelIssue6761Group>> _weakList = new List<WeakReference<ModelIssue6761Group>>();
		private void RefreshItems(object sender, EventArgs e)
		{
			foreach (var item in this.viewModel.ItemGroups)
			{
				item.Clear();
			}
			this.viewModel.ItemGroups.Clear();

			for (int i = 0; i < 30; i++)
			{
				_count++;
				var g1 = new ModelIssue6761Group("Group " + _count);
				_weakList.Add(new WeakReference<ModelIssue6761Group>(g1));
				this.viewModel.ItemGroups.Add(g1);
			}

			CleanAndReport();
		}

		private void GCClick(object sender, EventArgs e)
		{
			CleanAndReport();
		}
		void CleanAndReport()
		{ 
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			CleanWeakList(_weakList);
			viewModel.DisposeString = $"{_weakList.Count} object(s) alive";

			string report = $"MEMORY:{GC.GetTotalMemory(true)}";
			viewModel.TotalMemory = report;
		}

		private void CleanWeakList(IList<WeakReference<ModelIssue6761Group>> weakList)
		{
			ModelIssue6761Group item;
			for (int i = weakList.Count-1; i >= 0; i--)
			{
				if (!weakList[i].TryGetTarget(out item))
				{
					weakList.RemoveAt(i);
				}
			}
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue6761 : INotifyPropertyChanged
	{
		public ObservableCollection<ModelIssue6761Group> ItemGroups { get; set; } = new ObservableCollection<ModelIssue6761Group>();
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
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

		public ViewModelIssue6761()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ModelIssue6761Group : ObservableCollection<ModelIssue6761>
	{
		public ModelIssue6761Group(string name)
		{
			GroupName = name;
		}
		public string GroupName { get; set; }

	}

	[Preserve(AllMembers = true)]
	public class ModelIssue6761
	{
		public ModelIssue6761(string name)
		{
			Name = name;
		}
		public string Name { get; set; }

	}
}