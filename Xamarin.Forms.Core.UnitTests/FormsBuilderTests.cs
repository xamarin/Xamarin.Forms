using System.Collections.Generic;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class FormsBuilderTests : BaseTestFixture
	{
		[Test]
		public void InitTest()
		{
			var list = new List<string>();

			var formsBuilder = new FormsBuilder(() => list.Add("init"));
			formsBuilder.PostInit(() => list.Add("post1"));
			formsBuilder.PreInit(() => list.Add("pre1"));
			formsBuilder.PreInit(() => list.Add("pre2"));
			formsBuilder.PostInit(() => list.Add("post2"));
			formsBuilder.Init();

			Assert.IsTrue(list[0] == "pre1");
			Assert.IsTrue(list[1] == "pre2");
			Assert.IsTrue(list[2] == "init");
			Assert.IsTrue(list[3] == "post1");
			Assert.IsTrue(list[4] == "post2");
		}
	}
}