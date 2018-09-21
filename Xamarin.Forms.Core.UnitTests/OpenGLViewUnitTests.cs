using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class OpenGLViewUnitTests : BaseTestFixture
	{
		
		public void Display ()
		{
			var view = new OpenGLView ();
			bool displayed = false;

			view.DisplayRequested += (s, o) => displayed = true;

			view.Display ();
			Assert.True (displayed);
		}
	}
}
