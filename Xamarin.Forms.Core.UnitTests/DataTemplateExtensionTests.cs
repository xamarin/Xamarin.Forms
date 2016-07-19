using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class DataTemplateExtensionTests : BaseTestFixture
	{
		[Test]
		public void SetBindingNull()
		{
			Assert.That(() => TemplateExtensions.SetBinding(null, MockBindable.TextProperty, "Name"),
				Throws.InstanceOf<ArgumentNullException>());
			Assert.That(() => TemplateExtensions.SetBinding(new DataTemplate(() => new MockBindable()), null, "Name"),
				Throws.InstanceOf<ArgumentNullException>());
			Assert.That(() => TemplateExtensions.SetBinding(new DataTemplate(() => new MockBindable()), MockBindable.TextProperty, null),
				Throws.InstanceOf<ArgumentNullException>());

			Assert.That(() => TemplateExtensions.SetBinding<MockViewModel>(null, MockBindable.TextProperty, vm => vm.Text),
				Throws.InstanceOf<ArgumentNullException>());
			Assert.That(() => TemplateExtensions.SetBinding<MockViewModel>(new DataTemplate(() => new MockBindable()), null, vm => vm.Text),
				Throws.InstanceOf<ArgumentNullException>());
			Assert.That(() => TemplateExtensions.SetBinding<MockViewModel>(new DataTemplate(() => new MockBindable()), MockBindable.TextProperty, null),
				Throws.InstanceOf<ArgumentNullException>());
		}
	}
}
