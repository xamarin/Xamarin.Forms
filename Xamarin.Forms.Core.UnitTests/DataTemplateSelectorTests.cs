using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Xamarin.Forms;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class DataTemplateSelectorTests : BaseTestFixture
	{
		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Device.PlatformServices = null;
		}

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Device.PlatformServices = new MockPlatformServices();
		}

		class TemplateOne : DataTemplate
		{
			public TemplateOne() : base(typeof(ViewCell))
			{

			}
		}

		class TemplateTwo : DataTemplate
		{
			public TemplateTwo() : base(typeof(EntryCell))
			{

			}
		}

		class TemplateThree : DataTemplate
		{
			public TemplateThree() : base(typeof(ContentPage))
			{

			}
		}

		class NullSelector : DataTemplateSelector
		{
			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				return null;
			}
		}

		class TestDTS : DataTemplateSelector
		{
			public TestDTS()
			{
				templateOne = new TemplateOne();
				templateTwo = new TemplateTwo();
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				if (item is double)
					return templateOne;
				if (item is byte)
					return new TestDTS();
				if (item is string)
					return templateTwo;

				return null;
			}

			readonly DataTemplate templateOne;
			readonly DataTemplate templateTwo;
		}

		class TestPageDTS : DataTemplateSelector
		{
			public TestPageDTS()
			{
				templateThree = new TemplateThree();
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				if (item is View)
					return templateThree;

				return null;
			}

			readonly DataTemplate templateThree;
		}

		[Test]
		public void Constructor()
		{
			var dts = new TestDTS();
		}

		[Test]
		public void CanReturnNull()
		{
			var dts = new NullSelector();
			Assert.IsNull(dts.SelectTemplate((short)0, null));
		}

		[Test]
		public void ReturnsCorrectType()
		{
			var dts = new TestDTS();
			Assert.IsInstanceOf<TemplateOne>(dts.SelectTemplate(1d, null));
			Assert.IsInstanceOf<TemplateTwo>(dts.SelectTemplate("test", null));
			Assert.IsNull(dts.SelectTemplate((short)0, null));
		}

		[Test]
		public void ListViewSupport()
		{
			var listView = new ListView(ListViewCachingStrategy.RecycleElement);
			listView.ItemsSource = new object[] { 0d, "test", (short)0 };

			listView.ItemTemplate = new TestDTS();
			Assert.IsInstanceOf<ViewCell>(listView.TemplatedItems[0]);
			Assert.IsInstanceOf<EntryCell>(listView.TemplatedItems[1]);
			Assert.IsInstanceOf<TextCell>(listView.TemplatedItems[2]);
		}

		[Test]
		public void ListViewRecycleDTSupport()
		{
			var listView = new ListView(ListViewCachingStrategy.RecycleElementAndDataTemplate);
			listView.ItemsSource = new object[] { 0d, "test", (short)0 };

			listView.ItemTemplate = new TestDTS();
			Assert.IsInstanceOf<ViewCell>(listView.TemplatedItems[0]);
			Assert.IsInstanceOf<EntryCell>(listView.TemplatedItems[1]);
			Assert.IsInstanceOf<TextCell>(listView.TemplatedItems[2]);
		}

		[Test]
		public void NestingThrowsException()
		{
			var dts = new TestDTS();
			Assert.Throws<NotSupportedException>(() => dts.SelectTemplate((byte)0, null));
		}

		[Test]
		public void TabbedPageSupport()
		{
			var tabbedPage = new TabbedPage();
			tabbedPage.ItemsSource = new object[] { new View(), "test" };

			tabbedPage.ItemTemplate = new TestPageDTS();
			Assert.IsInstanceOf<ContentPage>(tabbedPage.Children[0]);
			Assert.IsInstanceOf<Page>(tabbedPage.Children[1]);
			Assert.AreEqual(tabbedPage.Children[1].BindingContext, "test");
		}
	}

	[TestFixture]
	public class DataTemplateRecycleTests : BaseTestFixture
	{
		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Device.PlatformServices = null;
		}

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Device.PlatformServices = new MockPlatformServices();
		}

		class TestDataTemplateSelector : DataTemplateSelector
		{
			readonly DataTemplate declarativeTemplate;
			readonly DataTemplate proceduralTemplate;

			public TestDataTemplateSelector()
			{
				declarativeTemplate = new DataTemplate(typeof(ViewCell));
				proceduralTemplate = new DataTemplate(() => new EntryCell());
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				Counter++;

				if (item is string)
					return declarativeTemplate;

				return proceduralTemplate;
			}

			public int Counter = 0;
		}

		[Test]
		public void ListViewSupport()
		{
			var listView = new ListView(ListViewCachingStrategy.RecycleElementAndDataTemplate);
			listView.ItemsSource = new object[] { "foo", "bar", 0 };

			Assert.That(listView.CachingStrategy ==
				ListViewCachingStrategy.RecycleElementAndDataTemplate);

			var selector = new TestDataTemplateSelector();
			listView.ItemTemplate = selector;
			Assert.That(selector.Counter == 0);

			Assert.IsInstanceOf<ViewCell>(listView.TemplatedItems[0]);
			Assert.That(selector.Counter == 1);

			Assert.IsInstanceOf<ViewCell>(listView.TemplatedItems[1]);
			Assert.That(selector.Counter == 1);

			Assert.Throws<NotSupportedException>(
				() => { var o = listView.TemplatedItems[2]; });
		}
	}
}