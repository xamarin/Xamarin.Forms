﻿using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Markup;

namespace Xamarin.Forms.Controls.GalleryPages.DragAndDropGalleries
{
	[Preserve(AllMembers = true)]
	public class VariousDragAndDropPermutations : ContentPage
	{
		public Color DraggingColor { get; set; }

		public VariousDragAndDropPermutations()
		{
			Title = "Various Drag And Drop Permutations";

			StackLayout stackLayout = new StackLayout();

			stackLayout.Children.Add(CreateControls<Label>((drag, drop) =>
			{
				drag.Text = "Drag";
				drag.FontSize = 18;
				drop.Text = "Drop";
				drop.FontSize = 18;
			}));

			stackLayout.Children.Add(CreateControls<Image>((drag, drop) =>
			{
				drag.HeightRequest = 50;
				drag.BackgroundColor = Color.Green;
			}));

			stackLayout.Children.Add(CreateControls<Image>((drag, drop) =>
			{
				drag.Source = "coffee.png";
				drag.BackgroundColor = Color.Green;
			}));

			stackLayout.Children.Add(CreateControls<Entry>(dragElementText: "Some text"));
			stackLayout.Children.Add(CreateControls<Editor>(dragElementText: "True"));
			stackLayout.Children.Add(CreateControls<DatePicker>(dragElementText: "False"));
			stackLayout.Children.Add(CreateControls<TimePicker>(dragElementText: $"{DateTime.Now}"));
			stackLayout.Children.Add(CreateControls<CheckBox>(dragElementText: $"{DateTime.Now.TimeOfDay}"));
			stackLayout.Children.Add(CreateControls<Entry>(dragElementText: "https://github.com/xamarin/Xamarin.Forms/blob/f27f5a3650f37894d4a1ac925d6fab4dc7350087/Xamarin.Forms.ControlGallery.Android/Resources/drawable/oasis.jpg?raw=true"));
			stackLayout.Children.Add(CreateControls<StackDrag>());

			Content = stackLayout;
		}

		[Preserve(AllMembers = true)]
		class StackDrag : StackLayout
		{
			public StackDrag()
			{
				Children.Add(new Image() { Source = "coffee.png" });
				Children.Add(new Label { Text = "COFFEE" });
			}
		}

		View CreateControls<TView>(Action<TView, TView> action = null, string dragElementText = null)
			where TView : View
		{
			Grid layout = new Grid();

			layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
			layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

			View drag = null;

			if (!String.IsNullOrWhiteSpace(dragElementText))
				drag = AddDragGesture((View)new Label() { Text = dragElementText });
			else
				drag = AddDragGesture(Activator.CreateInstance<TView>());

			var drop = AddDropGesture(Activator.CreateInstance<TView>());

			drop.SetBinding(VisualElement.BackgroundColorProperty, "DraggingColor");
			drop.BindingContext = this;
			layout.AddChild(drag, 0, 0);
			layout.AddChild(drop, 1, 0);
			action?.Invoke((TView)layout.Children[0], (TView)layout.Children[1]);
			return layout;
		}

		TView AddDragGesture<TView>(TView view)
			where TView : View
		{
			var dragRecognizer = new DragGestureRecognizer()
			{
				CanDrag = true
			};

			dragRecognizer.DragStarting += (_, args) =>
			{
				DraggingColor = Color.Purple;
				OnPropertyChanged(nameof(DraggingColor));

				if (view is StackDrag sd)
				{
					args.Data.Image = "coffee.png";
				}
			};

			dragRecognizer.DropCompleted += (_, __) =>
			{
				DraggingColor = Color.Default;
				OnPropertyChanged(nameof(DraggingColor));
			};

			view.GestureRecognizers.Add(dragRecognizer);

			return view;
		}

		TView AddDropGesture<TView>(TView view)
			where TView : View
		{
			var dropRecognizer = new DropGestureRecognizer()
			{
				AllowDrop = true
			};

			view.GestureRecognizers.Add(dropRecognizer);

			return view;
		}
	}
}
