using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.Android;
using NUnit.Framework;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class PlatformSpecific : MasterDetailPage
	{
		public PlatformSpecific()
		{
			InitializeComponent();
		}

		public PlatformSpecific(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[TestCase(true)]
			[TestCase(false)]
			public void PlatformSpecificPropertyIsSet(bool useCompiledXaml)
			{
				var layout = new PlatformSpecific(useCompiledXaml);
				Assert.AreEqual(layout.On<IConfigAndroid>().GetSomeAndroidThing(), 9999);
			}
		}
	}
}