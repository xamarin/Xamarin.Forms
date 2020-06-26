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
		string _selectedTemplate = nameof(Plane);
		bool _isCarTemplate;
		bool _shouldTriggerReset;

		public VaryingDataTempleteSelectorGallery()
		{
			InitializeComponent();
			BindingContext = this;

			foreach (var vehicle in CreateDefaultVehicles())
			{
				Items.Add(vehicle);
			}

			IEnumerable<VehicleBase> CreateDefaultVehicles()
			{
				yield return new Plane("Plane 0");
				yield return new Plane("Plane 1");
				yield return new Car("Car 2");
				yield return new Plane("Plane 3");
				yield return new Car("Car 4");
				yield return new Plane("Plane 5");
			}
		}

		public ImprovedObservableCollection<VehicleBase> Items { get; set; } =
			new ImprovedObservableCollection<VehicleBase>();

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

		public bool IsCarTemplate
		{
			get => _isCarTemplate;
			set
			{
				SetValue(ref _isCarTemplate, value);
				SelectedTemplate = value ?  nameof(Car) : nameof(Plane);
			}
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
					Items.InsertRange(index, Enumerable.Range(0, count).Select(_ => CreateVehicle()));
				else 
					Items.Insert(index, CreateVehicle());
			}
		}

		void Add_OnClicked(object sender, EventArgs e)
		{
			if (!IsValid(out var _)) return;

			using (Items.Suppress(ShouldTriggerReset))
			{
				if(int.TryParse(ItemsCount, out var count) && count > 0)
					Items.AddRange(Enumerable.Range(0, count).Select(_ => CreateVehicle()));
				else 
					Items.Add(CreateVehicle());
					
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

		VehicleBase CreateVehicle() => IsCarTemplate
				? (VehicleBase)new Car($"{nameof(Car)} {_counter++}")
				: new Plane($"{nameof(Plane)} {_counter++}");

		bool IsValid(out int index)
		{
			index = -1;
			if (string.IsNullOrWhiteSpace(Index)) return false;
			if (!int.TryParse(Index, out index)) return false;
			if (index > Items.Count || index < 0) return false;

			return true;
		}
	}

	public class ImprovedObservableCollection<T> : ObservableCollection<T>
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
			readonly ImprovedObservableCollection<T> _source;
			public Suppressor(ImprovedObservableCollection<T> source, ref bool shouldSuppress)
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

	class VehicleTemplateSelector : DataTemplateSelector
	{
		public DataTemplate CareTemplate { get; set; }
		public DataTemplate PlaneTemplate { get; set; }
		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (CareTemplate == null || PlaneTemplate == null) throw new ArgumentNullException();

			if (item is Car car) return CareTemplate;
			if (item is Plane plane) return PlaneTemplate;

			throw new ArgumentOutOfRangeException();
		}
	}

	public abstract class VehicleBase
	{
		protected VehicleBase(string name) => Name = name;

		public string Name { get; set; }
	}

	class Car : VehicleBase
	{
		public Car(string name) : base(name) { }
	}

	class Plane : VehicleBase
	{
		public Plane(string name) : base(name) { }
	}
}