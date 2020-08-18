﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.ComponentModel;
using System.Collections.ObjectModel;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7128, "[iOS] Changing model property scrolls CollectionView back to top",
		PlatformAffected.iOS)]
	public class Issue7128 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(CreateRoot());
		}

		class _7128Model : INotifyPropertyChanged
		{
			private string _url;
			private string _text;

			public _7128Model(string url, string text)
			{
				Url = url;
				Text = text;
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public string Url
			{
				get => _url;
				set
				{
					_url = value;
					OnPropertyChanged();
				}
			}

			public string Text
			{
				get => _text;
				set
				{
					_text = value;
					OnPropertyChanged();
				}
			}

			protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		View Template()
		{
			var layout = new Grid();

			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			var image = new Image { Aspect = Aspect.AspectFill };
			image.SetBinding(Image.SourceProperty, new Binding("Url"));
			
			var label = new Label { Margin = 10, BackgroundColor = Color.Red, HorizontalOptions = LayoutOptions.Fill };
			label.SetBinding(Label.TextProperty, new Binding("Text"));

			layout.Children.Add(image);
			layout.Children.Add(label);
			Grid.SetRow(image, 0);
			Grid.SetRow(image, 1);

			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += Tapped;

			label.GestureRecognizers.Add(tapGesture);

			return layout;
		}

		void Tapped(object sender, EventArgs e)
		{
			var label = sender as Label;

			var model = (_7128Model)label.BindingContext;

			model.Text = DateTime.UtcNow.Millisecond.ToString();
		}

		Page CreateRoot()
		{
			var page = new ContentPage() { Title = "Issue7128" };

			var layout = new StackLayout() { Padding = 5 };

			var instructions = new Label { Text = "Scroll the CollectionView down several pages, then click on one " +
				"of the labels. The text of the label should change, but the CollectionView should not scroll to a " +
				"different location. If it does scroll, the test has failed."
			};

			layout.Children.Add(instructions);

			var cv = new CollectionView
			{
				ItemTemplate = new DataTemplate(() => Template())
			};

			var source = new ObservableCollection<_7128Model>();

			var images = new string[] {
				"https://raw.githubusercontent.com/xamarin/Xamarin.Forms/main/Xamarin.Forms.ControlGallery.iOS/oasis.jpg",
				"https://raw.githubusercontent.com/xamarin/Xamarin.Forms/main/Xamarin.Forms.ControlGallery.iOS/photo.jpg",
				"https://raw.githubusercontent.com/xamarin/Xamarin.Forms/main/Xamarin.Forms.ControlGallery.iOS/xamarinstore.jpg",
				"https://raw.githubusercontent.com/xamarin/Xamarin.Forms/main/Xamarin.Forms.ControlGallery.iOS/crimson.jpg",
				"https://raw.githubusercontent.com/xamarin/Xamarin.Forms/main/Xamarin.Forms.ControlGallery.WindowsUniversal/cover1.jpg"
			};

			for (int n = 0; n < 35; n++)
			{
				source.Add(new _7128Model(images[n % 5], $"{n}.jpg"));
			}

			cv.ItemsSource = source;

			layout.Children.Add(cv);

			page.Content = layout;

			return page;
		}
	}
}
