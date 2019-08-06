using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Controls.Annotations;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public partial class DataTemplatePageGallery : ContentPage
	{
		DataTemplateViewModel _viewModel;
		public DataTemplatePageGallery()
		{
			InitializeComponent();
			_viewModel = new DataTemplateViewModel();
			BindingContext = _viewModel;

			var list = new List<BaseTestClass>
			{
				new FirstTestClass { StringValue = "First", CountValue = 1},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 2 },
				new ThirdTestClass { IntValue = 1, CountValue = 3 },
				new FirstTestClass { StringValue = "First", CountValue = 4},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 5 },
				new ThirdTestClass { IntValue = 1, CountValue = 6 },
				new FirstTestClass { StringValue = "First", CountValue = 7},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 8 },
				new ThirdTestClass { IntValue = 1, CountValue = 9 },
				new FirstTestClass { StringValue = "First", CountValue = 10},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 11 },
				new ThirdTestClass { IntValue = 1, CountValue = 12 },
				new FirstTestClass { StringValue = "First", CountValue = 13},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 14 },
				new ThirdTestClass { IntValue = 1, CountValue = 15 },
				new FirstTestClass { StringValue = "First", CountValue = 16},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 17 },
				new ThirdTestClass { IntValue = 1, CountValue = 18 },
				new FirstTestClass { StringValue = "First", CountValue = 19},
				new SecondTestClass { DateTimeValue = DateTime.Now, CountValue = 20 },
				new ThirdTestClass { IntValue = 1, CountValue = 21 },

			};

			foreach (var item in list)
			{
				_viewModel.Items.Add(item);
			}

		}


	}

	public class DataTemplateViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private ObservableCollection<BaseTestClass> _items = new ObservableCollection<BaseTestClass>();

		public ObservableCollection<BaseTestClass> Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}
	}

	public class BaseTestClass : INotifyPropertyChanged
	{
		int _countValue;

		public int CountValue
		{
			get => _countValue;
			set
			{
				_countValue = value;
				OnPropertyChanged();
			}
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}

	public class FirstTestClass : BaseTestClass
	{
		public string StringValue { get; set; }
	}

	public class SecondTestClass : BaseTestClass
	{
		public DateTime DateTimeValue { get; set; }
	}

	public class ThirdTestClass : BaseTestClass
	{
		public int IntValue { get; set; }
	}
}