using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class AttachedProperties : ContentPage
	{

		public AttachedProperties()
		{
			InitializeComponent();
		}

		public AttachedProperties(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
			}

			[TestCase(true)]
			[TestCase(false)]
			public void BindProperties(bool useCompiledXaml)
			{
				var layout = new AttachedProperties(useCompiledXaml);
				var collection1 = StackLayoutProperties.GetStackLayoutCollection(layout.StackLayout1);
				var collection2 = StackLayoutProperties.GetStackLayoutCollection(layout.StackLayout2);
				Assert.That(collection1[0].ExampleProperty1, Is.EqualTo("a"));
				Assert.That(collection1[1].ExampleProperty1, Is.EqualTo("b"));
				Assert.That(collection2[0].ExampleProperty1, Is.EqualTo("c"));
				Assert.That(collection2[1].ExampleProperty1, Is.EqualTo("d"));
			}
		}
	}


	public class MyCustomClass
	{
		public string ExampleProperty1 { get; set; }
	}

	public class StackLayoutProperties
	{
		public static readonly BindableProperty StackLayoutCollectionProperty =
			BindableProperty.CreateAttached("StackLayoutCollection",
				typeof(IList<MyCustomClass>),
				typeof(StackLayoutProperties),
				null,
				defaultValueCreator: _ => new List<MyCustomClass>());

		public static IList<MyCustomClass> GetStackLayoutCollection(BindableObject view) => (IList<MyCustomClass>)view.GetValue(StackLayoutCollectionProperty);
		public static void SetStackLayoutCollection(BindableObject view, IList<MyCustomClass> value) => view.SetValue(StackLayoutCollectionProperty, value);
	}
}