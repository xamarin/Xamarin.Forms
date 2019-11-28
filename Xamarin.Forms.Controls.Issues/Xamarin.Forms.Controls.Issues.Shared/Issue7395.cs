﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Linq;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif	
 	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7395, "Changing ItemTemplate does not work as expected", PlatformAffected.Android)]
	public class Issue7395 : TestContentPage
	{
		CollectionView _collectionView;

		public Issue7395()
		{
			Title = "Issue 7395";
		}

		protected override void Init()
		{
			var instructions = new Label
			{
				Text = "Click Two Columns. If all cells render correctly with 2 columns, then click Three Columns. If all cells render correctly with 3 columns, then the test passes."
			};

			var button1 = new Button
			{
				Text = "Two columns",
				AutomationId = "TwoCol"
			};

			button1.Clicked += OnButton1Clicked;

			var button2 = new Button
			{
				Text = "Three columns",
				AutomationId = "ThreeCol"
			};

			button2.Clicked += OnButton2Clicked;

			_collectionView = new CollectionView
			{
				BackgroundColor = Color.LightGreen,
				SelectionMode = SelectionMode.None,
				HeightRequest = 500
			};

			var lines = new List<Issue7395Model>();

			for (int i = 0; i < 30; i++)
			{
				lines.Add(new Issue7395Model() { Text = i.ToString() });
			}

			_collectionView.ItemsSource = lines;

			var stack = new StackLayout();

			stack.Children.Add(instructions);
			stack.Children.Add(button1);
			stack.Children.Add(button2);
			stack.Children.Add(_collectionView);

			Content = stack;
		}

		void OnButton1Clicked(object sender, EventArgs e)
		{
			_collectionView.ItemTemplate = CreateDataGridTemplate(2);
		}

		void OnButton2Clicked(object sender, EventArgs e)
		{
			_collectionView.ItemTemplate = CreateDataGridTemplate(3);
		}

		DataTemplate CreateDataGridTemplate(int columns)
		{
			DataTemplate template = new DataTemplate(() =>
			{
				var grid = new Grid() { Padding = new Thickness(0), Margin = 0, RowSpacing = 0, ColumnSpacing = 0 };

				grid.RowDefinitions.Clear();
				grid.ColumnDefinitions.Clear();
				grid.Children.Clear();
				grid.RowDefinitions.Add(new RowDefinition() { Height = 40 });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				Label cell;
				cell = new Label() { };
				cell.SetBinding(Label.TextProperty, "Text");
				cell.FontSize = 20;
				cell.FontAttributes = FontAttributes.Bold;
				cell.BackgroundColor = Color.LightBlue;
				grid.Children.Add(cell, 0, 0);

				for (int i = 0; i < columns; i++)
				{
					grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
					cell = new Label() { };
					cell.Text = "Col:" + i;
					cell.FontAttributes = FontAttributes.Bold;
					cell.BackgroundColor = Color.Beige;
					grid.Children.Add(cell, i + 1, 0);
				}
				return grid;
			});
			return template;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue7395Model
	{
		public string Text { get; set; }
	}
}
