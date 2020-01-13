using System;

using NUnit.Framework;

using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Markup.UnitTests
{
	public class MarkupBaseTestFixture<TBindable> : MarkupBaseTestFixture where TBindable : BindableObject, new()
	{
		protected TBindable Bindable { get; private set; }

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Bindable = new TBindable();
		}

		[TearDown]
		public override void TearDown()
		{
			Bindable = null;
			base.TearDown();
		}

		protected void TestPropertiesSet<TPropertyValue>(
			Action<TBindable> modify,
			params (BindableProperty property, TPropertyValue beforeValue, TPropertyValue expectedValue)[] propertyChanges
		) => TestPropertiesSet(Bindable, modify, propertyChanges);

		protected void TestPropertiesSet(
			Action<TBindable> modify,
			params (BindableProperty property, object beforeValue, object expectedValue)[] propertyChanges
		) => TestPropertiesSet(Bindable, modify, propertyChanges);
	}

	public class MarkupBaseTestFixture : BaseTestFixture
	{
		protected void TestPropertiesSet<TBindable, TPropertyValue>(
			TBindable bindable,
			Action<TBindable> modify,
			params (BindableProperty property, TPropertyValue beforeValue, TPropertyValue expectedValue)[] propertyChanges
		) where TBindable : BindableObject
		{
			foreach (var change in propertyChanges)
			{
				bindable.SetValue(change.property, change.beforeValue);
				Assume.That(bindable.GetPropertyIfSet(change.property, change.expectedValue), Is.Not.EqualTo(change.expectedValue));
			}

			modify(bindable);

			foreach (var change in propertyChanges)
				Assert.That(bindable.GetPropertyIfSet(change.property, change.beforeValue), Is.EqualTo(change.expectedValue));
		}
	}
}
