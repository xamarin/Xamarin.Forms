using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.View.Menu;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android.UnitTests;
using AToolBar = Android.Support.V7.Widget.Toolbar;

[assembly: ExportRenderer(typeof(TestShell), typeof(TestShellRenderer))]
namespace Xamarin.Forms.Platform.Android.UnitTests
{
	public class ToolbarExtensionsTests : PlatformTestFixture
	{
		[Test, Category("ToolbarExtensions")]
		[Description("ToolbarItem Text Set Correctly")]
		public void TextSetsCorrectlyWithNoTintColor()
		{
			List<ToolbarItem> sortedItems = new List<ToolbarItem>()
			{
				new ToolbarItem() { IsEnabled = true, Text = "a" },
				new ToolbarItem() { IsEnabled = true, Text = "b" },
				new ToolbarItem() { IsEnabled = true, Text = "c" },
			};

			var settings = new ToolbarSettings(sortedItems);
			SetupToolBar(settings);


			int i = 0;
			foreach(var textView in settings.TextViews)
			{
				Assert.AreEqual(sortedItems[i].Text, textView.Text);
				i++;
			}
		}


		[Test, Category("ToolbarExtensions")]
		[Description("ToolbarItem with Empty String doesn't crash")]
		public void DoesntCrashWithEmptyStringOnText()
		{
			List<ToolbarItem> sortedItems = new List<ToolbarItem>()
			{
				new ToolbarItem(),
			};

			// If this doesn't crash test has passed
			SetupToolBar(new ToolbarSettings(sortedItems));
		}

		[Test, Category("ToolbarExtensions")]
		[Description("Validate ToolBarItem TextColor Changes")]
		public void ToolBarItemsColoredCorrectlyBasedOnEnabledDisabled()
		{
			List<ToolbarItem> sortedItems = new List<ToolbarItem>()
			{
				new ToolbarItem() { IsEnabled = true, Text = "a" },
				new ToolbarItem() { IsEnabled = true, Text = "b" },
				new ToolbarItem() { IsEnabled = true, Text = "c" },
			};

			var settings = new ToolbarSettings(sortedItems) { TintColor = Color.Red };
			SetupToolBar(settings);
			AToolBar aToolBar = settings.ToolBar;
			List<IMenuItem> menuItemsCreated = settings.MenuItemsCreated;

			Assert.IsTrue(menuItemsCreated[2].IsEnabled);
			sortedItems[2].IsEnabled = false;

			Assert.IsTrue(menuItemsCreated[0].IsEnabled);
			Assert.IsTrue(menuItemsCreated[1].IsEnabled);
			Assert.IsFalse(menuItemsCreated[2].IsEnabled);

			var textViews = settings.TextViews.ToList();

			for(int i = 0; i < 3; i++)
			{
				ISpanned span = (ISpanned)textViews[i].TextFormatted;
				var color = (ForegroundColorSpan)span.GetSpans(0, span.Length(), Java.Lang.Class.FromType(typeof(ForegroundColorSpan)))[0];
				int androidColor;
				if (i != 2)
				{
					androidColor = (int)Color.Red.ToAndroid();
				}
				else
				{
					androidColor = (int)Color.Red.MultiplyAlpha(0.302).ToAndroid();
				}

				Assert.AreEqual(sortedItems[i].Text, textViews[i].Text);
				Assert.AreEqual(androidColor, color.ForegroundColor);
			}
		}

		void SetupToolBar(ToolbarSettings settings)
		{
			foreach(var item in settings.ToolbarItems)
			{
				if (String.IsNullOrWhiteSpace(item.AutomationId) && !String.IsNullOrWhiteSpace(item.Text))
					item.AutomationId = item.Text;
			}

			settings.ToolBar = new AToolBar(this.Context);

			Context context = this.Context;

			ToolbarExtensions.UpdateMenuItems(
				settings.ToolBar,
				settings.ToolbarItems,
				Context,
				settings.TintColor,
				OnToolbarItemPropertyChanged,
				settings.MenuItemsCreated,
				settings.ToolbarItemsCreated
			);

			void OnToolbarItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				settings.ToolBar.OnToolbarItemPropertyChanged(e,
					(ToolbarItem)sender, settings.ToolbarItems, Context, settings.TintColor, OnToolbarItemPropertyChanged,
					settings.MenuItemsCreated,
					settings.ToolbarItemsCreated);
			}
		}

		public class ToolbarSettings
		{
			public ToolbarSettings(List<ToolbarItem> toolbarItems)
			{
				ToolbarItems = toolbarItems;
				MenuItemsCreated = new List<IMenuItem>();
				ToolbarItemsCreated = new List<ToolbarItem>();
			}

			public List<ToolbarItem> ToolbarItems;
			public List<ToolbarItem> ToolbarItemsCreated;
			public AToolBar ToolBar;
			public Color? TintColor;
			public List<IMenuItem> MenuItemsCreated;

			public IEnumerable<ActionMenuItemView> TextViews =>
				ToolBar.GetChildrenOfType<ActionMenuItemView>()
					.OrderBy(x => x.Text);
		}
	}
}
