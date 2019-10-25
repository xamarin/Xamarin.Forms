using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class FormsInitTests : BaseTestFixture
	{
		[Test]
		public void InitTest()
		{
			var list = new List<string>();

			var formsInit = new FormsInit(() => list.Add("init"));
			formsInit.PostInit(() => list.Add("post1"));
			formsInit.PreInit(() => list.Add("pre1"));
			formsInit.PreInit(() => list.Add("pre2"));
			formsInit.PostInit(() => list.Add("post2"));
			formsInit.Init();

			Assert.IsTrue(list[0] == "pre1");
			Assert.IsTrue(list[1] == "pre2");
			Assert.IsTrue(list[2] == "init");
			Assert.IsTrue(list[3] == "post1");
			Assert.IsTrue(list[4] == "post2");
		}
	}
}