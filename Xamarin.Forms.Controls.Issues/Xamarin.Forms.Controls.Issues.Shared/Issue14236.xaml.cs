using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Linq;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 14236, "CollectionView does not call DataTemplateSelector", PlatformAffected.UWP)]
	public partial class Issue14236 : TestContentPage
	{
		public Issue14236()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue14236ViewModel();
		}
	}

	public class Issue14236ViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<Issue14236Item> Items { get; set; } = new ObservableCollection<Issue14236Item>();
		public ICommand AddItemsCommand { get; set; }

		public Issue14236ViewModel()
		{
			AddItems(20);
			AddItemsCommand = new Command(() => AddItems(100));
		}

		private void AddItems(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Items.Add(new Issue14236Item()
				{
					Color = i % 2 == 0 ? Issue14236Colors.Yellow : Issue14236Colors.Green
				});
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
		 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public class Issue14236ItemsTemplateSelector : DataTemplateSelector
	{
		public DataTemplate YellowTemplate { get; set; }
		public DataTemplate GreenTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is Issue14236Item i)
			{
				i.RequestTemplateCount++;

				return i.Color == Issue14236Colors.Yellow ? YellowTemplate : GreenTemplate;
			}

			return YellowTemplate;
		}
	}

	public enum Issue14236Colors
	{
		Yellow,
		Green
	}

	public class Issue14236Item : INotifyPropertyChanged
	{
		private int _requestTemplateCount;

		private Issue14236Colors _color;

		public Issue14236Colors Color
		{
			get => _color;
			set
			{
				_color = value;
				OnPropertyChanged();
			}
		}

		public int RequestTemplateCount
		{
			get => _requestTemplateCount;
			set
			{
				_requestTemplateCount = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
		 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}


}