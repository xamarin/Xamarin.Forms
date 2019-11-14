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

			var FormsBuilder = new FormsBuilder(() => list.Add("init"));
			FormsBuilder.PostInit(() => list.Add("post1"));
			FormsBuilder.PreInit(() => list.Add("pre1"));
			FormsBuilder.PreInit(() => list.Add("pre2"));
			FormsBuilder.PostInit(() => list.Add("post2"));
			FormsBuilder.Init();

			Assert.IsTrue(list[0] == "pre1");
			Assert.IsTrue(list[1] == "pre2");
			Assert.IsTrue(list[2] == "init");
			Assert.IsTrue(list[3] == "post1");
			Assert.IsTrue(list[4] == "post2");
		}
	}
}