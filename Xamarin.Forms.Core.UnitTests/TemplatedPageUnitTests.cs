using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
    [TestFixture]
	public class TemplatedPageUnitTests : BaseTestFixture 
	{
        [Test]
        public void TemplatedPage_should_have_the_InternalChildren_correctly_when_ControlTemplate_changed()
        {
            var sut = new TemplatedPage();
            IList<Element> internalChildren = ((IControlTemplated)sut).InternalChildren;
            internalChildren.Add(new VisualElement());
            internalChildren.Add(new VisualElement());
            internalChildren.Add(new VisualElement());

            sut.ControlTemplate = new ControlTemplate();

            Assert.AreEqual(1, internalChildren.Count);
        }
	}
}
