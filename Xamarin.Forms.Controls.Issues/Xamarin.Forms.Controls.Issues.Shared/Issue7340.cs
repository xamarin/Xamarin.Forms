using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Threading.Tasks;
#endif


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7340, "Picker sets null to bound property on BindingContext change", PlatformAffected.All)]
	public class Issue7340 : TestContentPage, System.ComponentModel.INotifyPropertyChanged
	{
		//TODO: Someone needs to fix this test. It doesn't get executed and I have no idea how this test setup should be used or works internally.

		const string Success = "Success";
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		private List<string> _items;
		public List<string> Items {
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Items)));
			}
		}
		private string _selectedItem;
		public string SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				_selectedItem = value;
				PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedItem)));
			}
		}

		protected override void Init()
		{
			var picker = new Picker();
			Items = new List<string>() { "test1", "test2" };
			SelectedItem = Items.First();
			picker.SetBinding(Picker.SelectedItemProperty, nameof(SelectedItem));
			picker.SetBinding(Picker.ItemsSourceProperty, nameof(Items));
			picker.BindingContext = this;

			if (Items.First() == (string)picker.SelectedItem)
			{
				//found this in another test ¯\_(ツ)_/¯
				var success = new Label { Text = Success };
				Content= success;
			}
		}

#if UITEST
		[Test]
		[Category(UITestCategories.Picker)]
		public void TestSelectedItemNullOnItemSourceChanged()
		{
			Assert.AreEqual(Items.First(), SelectedItem);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
