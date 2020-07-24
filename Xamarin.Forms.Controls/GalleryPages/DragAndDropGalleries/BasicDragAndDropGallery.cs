﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Xamarin.Forms.Controls.GalleryPages.DragAndDropGalleries
{
	public class BasicDragAndDropGallery : ContentPage
	{
		public BasicDragAndDropGallery()
		{
			StackLayout stackLayout = new StackLayout();
			CollectionView collectionView = new CollectionView();
			ObservableCollection<string> observableCollection = new ObservableCollection<string>();
			collectionView.ItemsSource = observableCollection;

			Image imageSource = new Image()
			{
				Source = "coffee.png",
				BackgroundColor = Color.Green
			};

			Image imageDestination = new Image()
			{
				BackgroundColor = Color.Purple,
				HeightRequest = 50,
				WidthRequest = 50
			};

			Button addRemoveDragGesture = new Button()
			{
				Text = "Add/Remove Drag Gesture",
				Command = new Command(() =>
				{
					var dragGestureRecognizer = imageSource.GestureRecognizers.OfType<DragGestureRecognizer>()
						.FirstOrDefault();

					if (dragGestureRecognizer != null)
						imageSource.GestureRecognizers.Remove(dragGestureRecognizer);
					else
					{
						var dragGesture = new DragGestureRecognizer()
						{
							CanDrag = true
						};

						dragGesture.DragStarting += (_, args) =>
						{
							observableCollection.Insert(0, $"DragStarting");
						};

						dragGesture.DropCompleted += (_, args) =>
						{
							observableCollection.Insert(0, $"DropCompleted");
						};

						imageSource.GestureRecognizers.Add(dragGesture);
					}
				})
			};

			Button toggleCanDrag = new Button()
			{
				Text = "Toggle Can Drag",
				Command = new Command(() =>
				{
					var dragGestureRecognizer = imageSource.GestureRecognizers.OfType<DragGestureRecognizer>()
						.FirstOrDefault();

					if (dragGestureRecognizer != null)
						dragGestureRecognizer.CanDrag = !dragGestureRecognizer.CanDrag;
				})
			};



			Button addRemoveDropGesture = new Button()
			{
				Text = "Add/Remove Drop Gesture",
				Command = new Command(() =>
				{
					var dropGestureRecognizer = imageDestination.GestureRecognizers.OfType<DropGestureRecognizer>()
						.FirstOrDefault();

					if (dropGestureRecognizer != null)
						imageDestination.GestureRecognizers.Remove(dropGestureRecognizer);
					else
					{
						var dropGesture = new DropGestureRecognizer()
						{
							AllowDrop = true
						};

						dropGesture.Drop += (_, args) =>
						{
							observableCollection.Insert(0, $"Drop");
						};

						dropGesture.DragOver += (_, args) =>
						{
							observableCollection.Insert(0, $"DragOver");
							args.AcceptedOperation = DataPackageOperation.Copy;
						};

						imageDestination.GestureRecognizers.Add(dropGesture);
					}
				})
			};

			Button toggleCanDrop = new Button()
			{
				Text = "Toggle Can Drop",
				Command = new Command(() =>
				{
					var dropGestureRecognizer = imageDestination.GestureRecognizers.OfType<DropGestureRecognizer>()
						.FirstOrDefault();

					if (dropGestureRecognizer != null)
						dropGestureRecognizer.AllowDrop = !dropGestureRecognizer.AllowDrop;
				})
			};

			stackLayout.Children.Add(imageSource);
			stackLayout.Children.Add(new Label()
			{
				Text = "https://github.com/xamarin/Xamarin.Forms/blob/f27f5a3650f37894d4a1ac925d6fab4dc7350087/Xamarin.Forms.ControlGallery.Android/Resources/drawable/oasis.jpg?raw=true",
				GestureRecognizers =
				{
					new DragGestureRecognizer()
					{
						CanDrag = true
					}
				}
			});

			stackLayout.Children.Add(imageDestination);
			stackLayout.Children.Add(addRemoveDragGesture);
			stackLayout.Children.Add(toggleCanDrag);
			stackLayout.Children.Add(addRemoveDropGesture);
			stackLayout.Children.Add(toggleCanDrop);
			stackLayout.Children.Add(collectionView);

			stackLayout.Padding = 40;
			Content = stackLayout;
		}
	}
}
