using System;

using Xamarin.Forms;

using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Exceptions;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class TypeMismatch : ContentPage
	{
		public TypeMismatch() => InitializeComponent();
		public TypeMismatch (bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[Test]
			public void ThrowsOnMismatchingType ([Values(true, false)]bool useCompiledXaml)
			{
				if (useCompiledXaml)
					Assert.Throws(new XamlParseExceptionConstraint(7, 16, XFException.Ecode.TypeMismatch), () => MockCompiler.Compile(typeof(TypeMismatch)));
				else
					Assert.Throws(new XamlParseExceptionConstraint(7, 16, XFException.Ecode.AssignProperty), () => new TypeMismatch(useCompiledXaml));
			}
		}
	}
}