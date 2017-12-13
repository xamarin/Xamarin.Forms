﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Diagnostics;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Gestures)]
	[Category(UITestCategories.ListView)]
	[Category(UITestCategories.Cells)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 58833, "ListView SelectedItem Binding does not fire", PlatformAffected.Android)]
	public class Bugzilla58833 : TestContentPage
	{
		const string ItemSelectedSuccess = "ItemSelected Success";
		const string TapGestureSucess = "TapGesture Fired";
		Label _resultLabel;
		static Label s_tapGestureFired;

		[Preserve(AllMembers = true)]
		class TestCell : ViewCell
		{
			readonly Label _content;

			internal static int s_index;

			public TestCell()
			{
				_content = new Label();

				if (s_index % 2 == 0)
				{
					_content.GestureRecognizers.Add(new TapGestureRecognizer
					{
						Command = new Command(() =>
						{
							s_tapGestureFired.Text = TapGestureSucess;
						})
					});
				}

				View = _content;
				ContextActions.Add(new MenuItem { Text = s_index++ + " Action" });

				_content.SetBinding(Label.TextProperty, new Binding("."));
			}

			protected override void OnBindingContextChanged()
			{
				base.OnBindingContextChanged();
				if(BindingContext != null)
					_content.AutomationId = (string)BindingContext;
			}
		}

		protected override void Init()
		{
			TestCell.s_index = 0;

			_resultLabel = new Label { Text = "Testing..." };
			s_tapGestureFired = new Label { Text = "Testing..." };

			var items = new List<string>();
			for (int i = 0; i < 5; i++)
				items.Add($"Item #{i}");

			var list = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(TestCell)),
				ItemsSource = items
			};
			list.ItemSelected += List_ItemSelected;

			Content = new StackLayout
			{
				Children = {
					_resultLabel,
					s_tapGestureFired,
					list
				}
			};
		}

		void List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			_resultLabel.Text = ItemSelectedSuccess;
		}

#if UITEST
		protected override bool Isolate => true;

		[Test]
		[Ignore("Failing without explanation on XTC, please run manually")]
		public void Bugzilla58833Test()
		{
			// Item #1 should not have a tap gesture, so it should be selectable
			RunningApp.WaitForElement(q => q.Marked("Item #1"));
			RunningApp.Tap(q => q.Text("Item #1"));
			RunningApp.Screenshot("After selected tap");
			RunningApp.WaitForElement(q => q.Text(ItemSelectedSuccess));

			// Item #2 should have a tap gesture
			RunningApp.WaitForElement(q => q.Marked("Item #2"));
			RunningApp.Tap(q => q.Text("Item #2"));
			RunningApp.WaitForElement(q => q.Text(TapGestureSucess));

			// Both items should allow access to the context menu
			RunningApp.ActivateContextMenu("Item #2");
			RunningApp.WaitForElement("2 Action");
			RunningApp.Tap(q => q.Marked("Item #3"));

			RunningApp.ActivateContextMenu("Item #1");
			RunningApp.WaitForElement("1 Action");
			RunningApp.Tap(q => q.Marked("Item #3"));

		}
#endif
	}
}