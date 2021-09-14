using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{

	class MyTestAdapter : List<int>, INotifyCollectionChanged
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public int NumberOfListener { get => CollectionChanged?.GetInvocationList().Length ?? 0; }
		public bool ExceptionHappened { get; private set; } = false;

		public new void Add(int item)
		{
			base.Add(item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public new void Clear()
		{
			base.Clear();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
			{
				try
				{
					CollectionChanged.Invoke(this, args);
				}
				catch (System.Exception)
				{
					ExceptionHappened = true;
				}
			}
		}
	}

	class MyTestViewModel : System.ComponentModel.INotifyPropertyChanged
	{
		private MyTestAdapter _myTestAdapter;
		public MyTestAdapter MyTestAdapter
		{
			get => _myTestAdapter;
			set => RaiseAndSetIfChanged(ref _myTestAdapter, value);
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		protected void RaiseAndSetIfChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(backingField, value))
			{
				backingField = value;
				OnPropertyChanged(propertyName);
			}
		}
	}


	[TestFixture]
	public class ItemsViewControllerLeakTest : PlatformTestFixture
	{
		[Test, Category("ItemsView")]
		public async Task ItemsViewControllerDoesNotLeakAsync()
		{
			var myAdapter1 = new MyTestAdapter();
			var myAdapter2 = new MyTestAdapter();
			var vm = new MyTestViewModel { MyTestAdapter = myAdapter1 };

			var view = new CollectionView
			{
				ItemTemplate = new DataTemplate(() => new ContentView()),
				BindingContext = vm
			};
			view.SetBinding(ItemsView.ItemsSourceProperty, nameof(MyTestViewModel.MyTestAdapter));
			await GetRendererProperty(view, ver => ver.NativeView, requiresLayout: true);

			await Device.InvokeOnMainThreadAsync(() =>
			{
				vm.MyTestAdapter = myAdapter2;
				vm.MyTestAdapter = myAdapter1;
				vm.MyTestAdapter = myAdapter2;
				vm.MyTestAdapter = myAdapter1;

				myAdapter1.Add(1);
			});
			Assert.That(myAdapter1.NumberOfListener, Is.EqualTo(1));
			Assert.That(myAdapter2.NumberOfListener, Is.EqualTo(0));
		}

		[Test, Category("ItemsView")]
		public async Task ItemsViewControllerDoesNotCrashAsync()
		{
			var myAdapter = new MyTestAdapter();
			var view1 = new CollectionView
			{
				BindingContext = new MyTestViewModel { MyTestAdapter = myAdapter }
			};
			view1.SetBinding(ItemsView.ItemsSourceProperty, nameof(MyTestViewModel.MyTestAdapter));
			await GetRendererProperty(view1, ver => ver.NativeView, requiresLayout: true);

			await Device.InvokeOnMainThreadAsync(() =>
			{
				myAdapter.Add(1);
				myAdapter.Clear();
				view1.BindingContext = null;
			});

			var view2 = new CollectionView
			{
				BindingContext = new MyTestViewModel { MyTestAdapter = myAdapter }
			};

			view2.SetBinding(ItemsView.ItemsSourceProperty, nameof(MyTestViewModel.MyTestAdapter));
			await GetRendererProperty(view2, (ver) => ver.NativeView, requiresLayout: true);


			await Device.InvokeOnMainThreadAsync(() =>
			{
				myAdapter.Add(2);
				myAdapter.Clear();
				view2.BindingContext = null;
			});

			Assert.That(myAdapter.ExceptionHappened, Is.EqualTo(false));
		}
	}
}