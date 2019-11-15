using System;
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

		[Test]
		public void BuildTest()
		{
			bool init = false;
			bool createApp = false;

			IFormsBuilder formsBuilder = new FormsBuilder(() => init = true);
			var app = formsBuilder.Build(() =>
			{
				createApp = true;
				return new MockApplication();
			});

			Assert.IsTrue(init);
			Assert.IsTrue(createApp);
			Assert.NotNull(app);
			Assert.NotNull(Application.ServiceProvider);
		}

		[Test]
		public void InitBuildTest()
		{
			int initCount = 0;

			IFormsBuilder formsBuilder = new FormsBuilder(() => initCount++);

			formsBuilder.Init();
			formsBuilder.Init();
			formsBuilder.Init();

			var app = formsBuilder.Build<MockApplication>();

			Assert.NotNull(app);
			Assert.AreEqual(1, initCount);
		}

		[Test]
		public void NullReferenceExceptionTest()
		{
			Assert.Throws<NullReferenceException>(() =>
			{
				IFormsBuilder formsBuilder = new FormsBuilder(null);
				formsBuilder.PreInit(null);
				formsBuilder.PostInit(null);
				formsBuilder.Init();
				formsBuilder.Build(null);
			});
		}
	}
}