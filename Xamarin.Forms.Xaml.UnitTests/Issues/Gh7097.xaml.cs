using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Gh7097 : Gh7097Base
	{
		public Gh7097() => InitializeComponent();
		public Gh7097(bool useCompiledXaml) : base()
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			IReadOnlyList<string> _flags;
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
				_flags = Device.Flags;
				Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "CollectionView_Experimental" }.AsReadOnly());
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
				Device.SetFlags(_flags);
			}

			[Test]
			public void CanXReferenceRoot([Values(false, true)]bool useCompiledXaml)
			{
				var layout = new Gh7097(useCompiledXaml);
				var cv = layout.Content as CollectionView;
				var content = cv.ItemTemplate.CreateContent();
				//Assert.That((layout.Content as Label).Text, Is.EqualTo("Foo"));
			}

			[Test]
			//this was later reported as https://github.com/xamarin/Xamarin.Forms/issues/7286
			public void RegisteringXNameOnSubPages([Values(false, true)]bool useCompiledXaml)
			{
				var layout = new Gh7097(useCompiledXaml);
				Assert.That(layout.self, Is.Not.Null);
				Assert.That(layout.collectionview, Is.Not.Null);
			}
		}
	}
}
