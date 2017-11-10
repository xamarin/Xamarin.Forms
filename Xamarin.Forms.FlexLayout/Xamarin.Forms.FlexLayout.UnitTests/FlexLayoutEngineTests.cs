using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.FlexLayoutTests;

namespace Xamarin.Forms.FlexLayoutTests
{
    [TestFixture]
    public class FlexLayoutengineTests : BaseTestFixture
    {
        [SetUp]
        public override void Setup()
        {
            global::Xamarin.Forms.FlexLayout.RegisterEngine(null);
            base.Setup();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            global::Xamarin.Forms.FlexLayout.RegisterEngine(null);
        }

        [Test]
        public void ThrowsOnInitWithNoEngine()
        {
            Assert.Throws<InvalidOperationException>(() => new FlexLayout());
        }

        [Test]
        public void OnInitWithEngine()
        {
			global::Xamarin.Forms.FlexLayout.RegisterEngine(typeof(Xamarin.FlexLayoutEngine.Flex.FlexEngine));
            Assert.DoesNotThrow(() => new FlexLayout());
        }


    }
}
