using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13126, "[Bug] Regression: 5.0.0-pre5 often fails to draw dynamically loaded collection view content", PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CollectionView)]
#endif
	public class Issue13126 : TestContentPage
	{
		_13126VM _vm;
		const string Success = "Success";

		protected override void Init()
		{
			_vm = new _13126VM();
			BindingContext = _vm;

			var collectionView = BindingWithConverter();

			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition() { Height = GridLength.Star },
				}
			};

			grid.Children.Add(collectionView);

			Content = grid;

			_vm.IsBusy = true;

			Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					_vm.Data.Add(Success);
					_vm.IsBusy = false;
				});

				return false;
			});
		}

		CollectionView BindingWithConverter()
		{
			var cv = new CollectionView
			{
				IsVisible = true,

				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, new Binding("."));
					return label;
				})
			};

			cv.SetBinding(CollectionView.ItemsSourceProperty, new Binding("Data"));
			cv.SetBinding(VisualElement.IsVisibleProperty, new Binding("IsBusy", converter: new BoolInverter()));

			return cv;
		}

		class BoolInverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return !((bool)value);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		class _13126VM : INotifyPropertyChanged
		{
			private bool _isBusy;

			public bool IsBusy
			{
				get
				{
					return _isBusy;
				}

				set
				{
					_isBusy = value;
					OnPropertyChanged(nameof(IsBusy));
				}
			}

			public ObservableCollection<string> Data { get; } = new ObservableCollection<string>();

			public event PropertyChangedEventHandler PropertyChanged;

			void OnPropertyChanged(string name)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
			}
		}

#if UITEST
		[Test]
		public void CollectionViewShouldSourceShouldUpdateWhileInvisible()
		{
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
