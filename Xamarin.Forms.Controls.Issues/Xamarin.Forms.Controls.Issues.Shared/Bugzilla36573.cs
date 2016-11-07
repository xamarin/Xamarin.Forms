using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 36573, "[iOS] Selecting Disabled Context Action Fire ListView.ItemSelected", PlatformAffected.iOS)]
	public class Bugzilla36573 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var items = new List<object> { "", "", "", "" };

			var myListView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(MyCell)),
				ItemsSource = items,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			myListView.ItemSelected += async (sender, e) => {

				if (e.SelectedItem != null)
					await DisplayAlert("Item Selected", "List item was selected", "Okay");

				(sender as ListView).SelectedItem = null;
			};

			Content = myListView;
		}
	}

	internal class MyCell : ViewCell
	{
		public MyCell()
		{
			var downloadItem = new MenuItem
			{
				Text = "Download",
				Command = new Command(async () =>
				{
					await Application.Current.MainPage.DisplayAlert("ContextAction", "Download Selected", "Okay");
				})
			};

			var deleteItem = new MenuItem
			{
				Text = "Disabled",
				Command = new Command(async () =>
				{
					await Application.Current.MainPage.DisplayAlert("ContextAction", "Disabled MenuItem Selected", "Okay");
				}, () => false),

			};

			ContextActions.Add(downloadItem);
			ContextActions.Add(deleteItem);

			View = new Label { Text = "List Item" };
		}
	}
}