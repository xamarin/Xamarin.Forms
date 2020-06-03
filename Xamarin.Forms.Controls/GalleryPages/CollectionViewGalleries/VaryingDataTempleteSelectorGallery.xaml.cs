using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaryingDataTempleteSelectorGallery : ContentPage, INotifyPropertyChanged
	{
		string _index = "1";

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
				yield return new Plane("Plane1");
				yield return new Plane("Plane2");
				yield return new Car("Car1");
				yield return new Plane("Plane3");
				yield return new Car("Car2");
				yield return new Plane("Plane4");
			}
		}

		public ObservableCollection<VehicleBase> Items { get; set; } =
			new ObservableCollection<VehicleBase>();

		public string Index
		{
			get => _index;
			set => SetValue(ref _index, value);
		}

		void InsertCar_OnClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Index)) return;
			if (!int.TryParse(Index, out var index)) return;
			if (index > Items.Count || index < 0) return;

			Items.Insert(index, new Car("rCar " + new Random().Next(0,1000).ToString()));
		}

		void InsertPlane_OnClicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Index)) return;
			if (!int.TryParse(Index, out var index)) return;
			if (index > Items.Count || index < 0) return;

			Items.Insert(index, new Plane("rPlane " + new Random().Next(0,1000).ToString()));
		}

		void SetValue<T>(ref T backingField, in T value, [CallerMemberName] string callerName = null)
		{
			if (Equals(backingField, value)) return;
			OnPropertyChanging(callerName);
			backingField = value;
			OnPropertyChanged(callerName);
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