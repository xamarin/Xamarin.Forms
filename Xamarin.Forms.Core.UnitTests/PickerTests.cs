using System;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Core.UnitTests
{
	internal class ContextFixture
	{
		public string DisplayName { get; set; }

		public string ComplexName { get; set; }

		public ContextFixture(string displayName, string complexName)
		{
			DisplayName = displayName;
			ComplexName = complexName;
		}

		public ContextFixture()
		{
		}
	}

	internal class BindingContext
	{
		public ObservableCollection<object> Items { get; set; }

		public object SelectedItem { get; set; }
	}

	[TestFixture]
	public class PickerTests : BaseTestFixture
	{
		[Test]
		public void TestSetSelectedIndexOnNullRows()
		{
			var picker = new Picker();

			Assert.IsEmpty(picker.Items);
			Assert.AreEqual(-1, picker.SelectedIndex);

			picker.SelectedIndex = 2;

			Assert.AreEqual(-1, picker.SelectedIndex);
		}

		[Test]
		public void TestSelectedIndexInRange()
		{
			var picker = new Picker
			{
				Items = { "John", "Paul", "George", "Ringo" },
				SelectedIndex = 2
			};

			Assert.AreEqual(2, picker.SelectedIndex);

			picker.SelectedIndex = 42;
			Assert.AreEqual(3, picker.SelectedIndex);

			picker.SelectedIndex = -1;
			Assert.AreEqual(-1, picker.SelectedIndex);

			picker.SelectedIndex = -42;
			Assert.AreEqual(-1, picker.SelectedIndex);
		}

		[Test]
		public void TestSelectedIndexInRangeDefaultSelectedIndex()
		{
			var picker = new Picker
			{
				Items = { "John", "Paul", "George", "Ringo" }
			};

			Assert.AreEqual(-1, picker.SelectedIndex);

			picker.SelectedIndex = -5;
			Assert.AreEqual(-1, picker.SelectedIndex);

			picker.SelectedIndex = 2;
			Assert.AreEqual(2, picker.SelectedIndex);

			picker.SelectedIndex = 42;
			Assert.AreEqual(3, picker.SelectedIndex);

			picker.SelectedIndex = -1;
			Assert.AreEqual(-1, picker.SelectedIndex);

			picker.SelectedIndex = -42;
			Assert.AreEqual(-1, picker.SelectedIndex);
		}

		[Test]
		public void TestSelectedIndexChangedOnCollectionShrink()
		{
			var picker = new Picker { Items = { "John", "Paul", "George", "Ringo" }, SelectedIndex = 3 };

			Assert.AreEqual(3, picker.SelectedIndex);

			picker.Items.RemoveAt(3);
			picker.Items.RemoveAt(2);

			Assert.AreEqual(1, picker.SelectedIndex);

			picker.Items.Clear();
			Assert.AreEqual(-1, picker.SelectedIndex);
		}

		[Test]
		public void TestSetItemsSourceProperty()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo",
				0,
				new DateTime(1970, 1, 1),
			};
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				ItemsSource = items
			};
			Assert.AreEqual(5, picker.Items.Count);
			Assert.AreEqual("John", picker.Items[0]);
			Assert.AreEqual("0", picker.Items[3]);
		}

		[Test]
		public void TestItemsSourceCollectionChangedAppend()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo"
			};
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				ItemsSource = items,
				SelectedIndex = 0
			};
			Assert.AreEqual(3, picker.Items.Count);
			Assert.AreEqual("John", picker.Items[0]);
			items.Add(new { Name = "George" });
			Assert.AreEqual(4, picker.Items.Count);
			Assert.AreEqual("George", picker.Items[picker.Items.Count - 1]);
		}

		[Test]
		public void TestItemsSourceCollectionChangedClear()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo"
			};
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				ItemsSource = items,
				SelectedIndex = 0
			};
			Assert.AreEqual(3, picker.Items.Count);
			items.Clear();
			Assert.AreEqual(0, picker.Items.Count);
		}

		[Test]
		public void TestItemsSourceCollectionChangedInsert()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo"
			};
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				ItemsSource = items,
				SelectedIndex = 0
			};
			Assert.AreEqual(3, picker.Items.Count);
			Assert.AreEqual("John", picker.Items[0]);
			items.Insert(1, new { Name = "George" });
			Assert.AreEqual(4, picker.Items.Count);
			Assert.AreEqual("George", picker.Items[1]);
		}

		[Test]
		public void TestItemsSourceCollectionChangedReAssign()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo"
			};
			var bindingContext = new { Items = items };
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				BindingContext = bindingContext
			};
			picker.SetBinding(Picker.ItemsSourceProperty, "Items");
			Assert.AreEqual(3, picker.Items.Count);
			items = new ObservableCollection<object>
			{
				"Peach",
				"Orange"
			};
			picker.BindingContext = new { Items = items };
			Assert.AreEqual(2, picker.Items.Count);
			Assert.AreEqual("Peach", picker.Items[0]);
		}

		[Test]
		public void TestItemsSourceCollectionChangedRemove()
		{
			var items = new ObservableCollection<object>
			{
				new { Name = "John" },
				"Paul",
				"Ringo"
			};
			var picker = new Picker
			{
				DisplayMemberPath = "Name",
				ItemsSource = items,
				SelectedIndex = 0
			};
			Assert.AreEqual(3, picker.Items.Count);
			Assert.AreEqual("John", picker.Items[0]);
			items.RemoveAt(1);
			Assert.AreEqual(2, picker.Items.Count);
			Assert.AreEqual("Ringo", picker.Items[1]);
		}

		[Test]
		public void TestItemsSourceCollectionOfStrings()
		{
			var items = new ObservableCollection<string>
			{
				"John",
				"Paul",
				"Ringo"
			};
			var picker = new Picker
			{
				ItemsSource = items,
				SelectedIndex = 0
			};
			Assert.AreEqual(3, picker.Items.Count);
			Assert.AreEqual("John", picker.Items[0]);
		}

		[Test]
		public void TestSelectedItemDefault()
		{
			var bindingContext = new BindingContext
			{
				Items = new ObservableCollection<object>
				{
					new ContextFixture("John", "John")
				}
			};
			var picker = new Picker
			{
				BindingContext = bindingContext
			};
			picker.SetBinding(Picker.ItemsSourceProperty, "Items");
			picker.SetBinding(Picker.SelectedItemProperty, "SelectedItem");
			Assert.AreEqual(1, picker.Items.Count);
			Assert.AreEqual(-1, picker.SelectedIndex);
			Assert.AreEqual(bindingContext.SelectedItem, picker.SelectedItem);
		}

		[Test]
		public void TestSelectedItemSet()
		{
			var obj = new ContextFixture("John", "John");
			var bindingContext = new BindingContext
			{
				Items = new ObservableCollection<object>
				{
					obj
				},
				SelectedItem = obj
			};
			var picker = new Picker
			{
				BindingContext = bindingContext,
				DisplayMemberPath = "DisplayName"
			};
			picker.SetBinding(Picker.ItemsSourceProperty, "Items");
			picker.SetBinding(Picker.SelectedItemProperty, "SelectedItem");
			Assert.AreEqual(1, picker.Items.Count);
			Assert.AreEqual(0, picker.SelectedIndex);
			Assert.AreEqual(obj, picker.SelectedItem);
		}

		[Test]
		public void TestSelectedItemChangeSelectedIndex()
		{
			var obj = new ContextFixture("John", "John");
			var bindingContext = new BindingContext
			{
				Items = new ObservableCollection<object>
				{
					obj
				},
			};
			var picker = new Picker
			{
				BindingContext = bindingContext,
				DisplayMemberPath = "DisplayName"
			};
			picker.SetBinding(Picker.ItemsSourceProperty, "Items");
			picker.SetBinding(Picker.SelectedItemProperty, "SelectedItem");
			Assert.AreEqual(1, picker.Items.Count);
			Assert.AreEqual(-1, picker.SelectedIndex);
			Assert.AreEqual(null, picker.SelectedItem);
			picker.SelectedItem = obj;
			Assert.AreEqual(0, picker.SelectedIndex);
			Assert.AreEqual(obj, picker.SelectedItem);
			picker.SelectedIndex = -1;
			Assert.AreEqual(-1, picker.SelectedIndex);
			Assert.AreEqual(null, picker.SelectedItem);
		}
	}
}