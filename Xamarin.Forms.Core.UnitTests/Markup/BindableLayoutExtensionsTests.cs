using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class BindableLayoutExtensionsTests : MarkupBaseTestFixture<StackLayout>
	{
		public BindableLayoutExtensionsTests(bool withExperimentalFlag) : base(withExperimentalFlag) { }

		[Test]
		public void EmptyView()
		{
			var view = new BoxView();
			TestPropertiesSet(l => l.EmptyView(view), (BindableLayout.EmptyViewProperty, view));
		}

		[Test]
		public void EmptyViewTemplate()
		{
			var template = new DataTemplate(() => new BoxView());
			TestPropertiesSet(l => l.EmptyViewTemplate(template), (BindableLayout.EmptyViewTemplateProperty, template));
		}

		[Test]
		public void ItemsSource()
		{
			var source = new string[] { };
			TestPropertiesSet(l => l.ItemsSource(source), (BindableLayout.ItemsSourceProperty, source));
		}

		[Test]
		public void ItemTemplate()
		{
			var template = new DataTemplate(() => new BoxView());
			TestPropertiesSet(l => l.ItemTemplate(template), (BindableLayout.ItemTemplateProperty, template));
		}

		[Test]
		public void ItemTemplateSelector()
		{
			var selector = new Selector();
			TestPropertiesSet(l => l.ItemTemplateSelector(selector), (BindableLayout.ItemTemplateSelectorProperty, selector));
		}

		class Selector : DataTemplateSelector
		{
			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			=> new DataTemplate(() => new BoxView());
		}

		[Test]
		public void SupportDerivedFromView() => AssertExperimental(() =>
		{
			_ = new DerivedFromView()
				.EmptyView(new BoxView())
				.EmptyViewTemplate(new DataTemplate(() => new BoxView()))
				.ItemsSource(new string[] { })
				.ItemTemplate(new DataTemplate(() => new BoxView()))
				.ItemTemplateSelector(new Selector());
		});

		class DerivedFromView : StackLayout { }
	}
}