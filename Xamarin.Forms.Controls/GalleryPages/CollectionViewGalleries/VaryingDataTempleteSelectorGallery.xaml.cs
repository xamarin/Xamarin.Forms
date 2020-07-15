using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Controls.GalleryPages.SwipeViewGalleries;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaryingDataTempleteSelectorGallery : ContentPage, INotifyPropertyChanged
	{
		string _index = "1";
		string _itemsCount = "1";
		int _counter = 6;
		string _selectedTemplate = nameof(Latte);
		bool _shouldTriggerReset;

		public VaryingDataTempleteSelectorGallery()
		{
			InitializeComponent();
			BindingContext = this;

			foreach (var vehicle in CreateDefaultVehicles())
			{
				Items.Add(vehicle);
			}

			IEnumerable<DrinkBase> CreateDefaultVehicles()
			{
				yield return new Coffee("0");
				yield return new Milk("1");
				yield return new Coffee("2");
				yield return new Coffee("3");
				yield return new Milk("4");
				yield return new Coffee("5");
			}
		}

		public SuppressableObservableCollection<DrinkBase> Items { get; set; } =
			new SuppressableObservableCollection<DrinkBase>();

		public string Index
		{
			get => _index;
			set => SetValue(ref _index, value);
		}

		public string ItemsCount
		{
			get => _itemsCount;
			set => SetValue(ref _itemsCount, value);
		}

		public string SelectedTemplate
		{
			get => _selectedTemplate;
			set => SetValue(ref _selectedTemplate, value);
		}

		public bool ShouldTriggerReset
		{
			get => _shouldTriggerReset;
			set => SetValue(ref _shouldTriggerReset, value);
		}

		void Insert_OnClicked(object sender, EventArgs e)
		{
			if (!IsValid(out var index)) return;

			using (Items.Suppress(ShouldTriggerReset))
			{
				if(int.TryParse(ItemsCount, out var count) && count > 0)
					Items.InsertRange(index, Enumerable.Range(0, count).Select(_ => CreateDrink()));
				else 
					Items.Insert(index, CreateDrink());
			}
		}

		void Add_OnClicked(object sender, EventArgs e)
		{
			if (!IsValid(out var _)) return;

			using (Items.Suppress(ShouldTriggerReset))
			{
				if(int.TryParse(ItemsCount, out var count) && count > 0)
					Items.AddRange(Enumerable.Range(0, count).Select(_ => CreateDrink()));
				else 
					Items.Add(CreateDrink());
					
			}
		}

		void SetValue<T>(ref T backingField, in T value, [CallerMemberName] string callerName = null)
		{
			if (Equals(backingField, value)) return;
			OnPropertyChanging(callerName);
			backingField = value;
			OnPropertyChanged(callerName);
		}

		void Remove_OnClicked(object sender, EventArgs e)
		{
			if(!IsValid(out var index)) return;

			using (Items.Suppress(ShouldTriggerReset))
			{
				Items.RemoveAt(index);
			}
		}

		DrinkBase CreateDrink()
		{
			switch (SelectedTemplate)
			{
				case nameof(Milk): return new Milk(_counter++.ToString());
				case nameof(Coffee): return new Coffee(_counter++.ToString());
				case nameof(Latte):
				{
					var latte = new Latte(_counter++.ToString());
					latte.Ingredients = new ObservableCollection<DrinkBase>(){ new Milk(_counter++.ToString()), new Coffee(_counter++.ToString()) };
					return latte;
				}
				default: throw new ArgumentException();
			}
		}

		bool IsValid(out int index)
		{
			index = -1;
			if (string.IsNullOrWhiteSpace(Index)) return false;
			if (!int.TryParse(Index, out index)) return false;
			if (index > Items.Count || index < 0) return false;

			return true;
		}
	}

	public class SuppressableObservableCollection<T> : ObservableCollection<T>
	{
		bool _suppress;

		public IDisposable Suppress(bool shouldSuppress) => new Suppressor(this, ref shouldSuppress);

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(!_suppress)
				base.OnCollectionChanged(e);
		}

		public void InsertRange(int index, IEnumerable<T> items)
		{
			foreach (T item in items)
				Insert(index, item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items)
				Add(item);
		}

		class Suppressor : IDisposable
		{
			readonly SuppressableObservableCollection<T> _source;
			public Suppressor(SuppressableObservableCollection<T> source, ref bool shouldSuppress)
			{
				_source = source;
				_source._suppress = shouldSuppress;
			}
			public void Dispose()
			{
				_source._suppress = false;
				_source.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}
	}

	class DrinkTemplateSelector : DataTemplateSelector
	{
		public DataTemplate CoffeeTemplate { get; set; }
		public DataTemplate MilkTemplate { get; set; }
		public DataTemplate LatteTemplate { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			ThrowIfNullHelper(CoffeeTemplate, MilkTemplate, LatteTemplate);

			if (item is Coffee c) return CoffeeTemplate;
			if (item is Milk m) return MilkTemplate;
			if (item is Latte l) return LatteTemplate;
			
			throw new ArgumentOutOfRangeException();
		}

		static void ThrowIfNullHelper(params object[] items) => Array.ForEach(items, o => 
		{ 
			if (o == null) throw new ArgumentNullException();
		});
	}

	public abstract class DrinkBase
	{
		protected DrinkBase(string name) => Name = name;

		public string Name { get; set; }
	}

	class Coffee : DrinkBase
	{
		public Coffee(string name) : base(nameof(Coffee) + name) { }
	}

	class Milk : DrinkBase
	{
		public Milk(string name) : base(nameof(Milk) + name) { }
	}

	class Latte : DrinkBase
	{
		public Latte(string name) : base(nameof(Latte) + name) { }

		public ObservableCollection<DrinkBase> Ingredients { get; set; } = new ObservableCollection<DrinkBase>();
	}
}