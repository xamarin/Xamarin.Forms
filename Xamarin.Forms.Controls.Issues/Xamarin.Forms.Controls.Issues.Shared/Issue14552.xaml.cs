using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 14552,
		"[Bug] NullReferenceException in DragAndDropGestureHandler.<OnLongPress> on Android",
		PlatformAffected.Android)]
	public partial class Issue14552 : TestContentPage
	{
		public Issue14552()
		{
#if APP
			InitializeComponent();

			BindingContext = this;

			for (int i = 0; i < 25; i++)
				ViewModel.Items.Add(new Issue14552Item { Name = $"Item{i}" });
#endif
		}

		public Issue14552ViewModel ViewModel { get; } = new Issue14552ViewModel();

		protected override void Init()
		{
		}

		void OnDragStarting(object sender, DragStartingEventArgs args)
		{
			Issue14552Item item = (Issue14552Item)((Element)sender).BindingContext;

			int index = ViewModel.Items.IndexOf(item);
			if (index != -1)
			{
				// TODO - Xamarin - ideally we'd assign item directly into the property bag, but that crashes
				args.Data.Text = index.ToString(CultureInfo.InvariantCulture);
				args.Handled = true;
			}
		}

		void OnDragOver(object sender, DragEventArgs args)
		{
			Issue14552Item draggedOver = (Issue14552Item)((Element)sender).BindingContext;

			args.AcceptedOperation = DataPackageOperation.None;

			if (Int32.TryParse(args.Data.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int draggedIndex) && draggedIndex >= 0 && draggedIndex < this.ViewModel.Items.Count)
			{
				Issue14552Item dropped = this.ViewModel.Items[draggedIndex];
				if (!ReferenceEquals(dropped, draggedOver))
					args.AcceptedOperation = DataPackageOperation.Copy;
			}
		}
	}
	public class Issue14552Item : Observable
	{
		string _name;
		public string Name
		{
			get => _name;
			set => Set(ref _name, value);
		}

		bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set => Set(ref _isSelected, value);
		}

		bool _isEnabled = true;
		public bool IsEnabled
		{
			get => _isEnabled;
			set => Set(ref _isEnabled, value);
		}
	}

	public class Issue14552ViewModel
	{
		public ObservableCollection<Issue14552Item> Items { get; } = new ObservableCollection<Issue14552Item>();
	}


	public abstract class Observable : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected bool Set<T>(T storage, T value, Action<T> assign, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
				return false;

			assign(value);

			OnPropertyChanged(propertyName);
			return true;
		}

		public void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}