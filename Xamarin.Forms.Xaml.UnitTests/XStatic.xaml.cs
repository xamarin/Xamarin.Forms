﻿using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public class MockxStatic
	{
		public static string MockStaticProperty { get { return "Property"; } }
		public const string MockConstant = "Constant";
		public static string MockField = "Field";
		public string InstanceProperty { get { return "InstanceProperty"; } }
		public static readonly Color BackgroundColor = Color.Fuchsia;
	}

	public enum MockEnum : long
	{
		First,
		Second,
		Third,
	}

	public partial class XStatic : ContentPage
	{
		public XStatic ()
		{
			InitializeComponent ();
		}
		public XStatic (bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			//{x:Static Member=prefix:typeName.staticMemberName}
			//{x:Static prefix:typeName.staticMemberName}

			//The code entity that is referenced must be one of the following:
			// - A constant
			// - A static property
			// - A field
			// - An enumeration value
			// All other cases should throw

			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
			}

			[TestCase (false)]
			[TestCase (true)]
			public void StaticProperty (bool useCompiledXaml)
			{
				var layout = new XStatic (useCompiledXaml);
				Assert.AreEqual ("Property", layout.staticproperty.Text);
			}

			[TestCase (false)]
			[TestCase (true)]
			public void MemberOptional (bool useCompiledXaml)
			{
				var layout = new XStatic (useCompiledXaml);
				Assert.AreEqual ("Property", layout.memberisoptional.Text);
			}

			[TestCase (false)]
			[TestCase (true)]
			public void FieldColor (bool useCompiledXaml)
			{
				var layout = new XStatic (useCompiledXaml);
				Assert.AreEqual (Color.Fuchsia, layout.color.TextColor);
			}

			[TestCase(false)]
			[TestCase(true)]
			public void Constant(bool useCompiledXaml)
			{
				var layout = new XStatic(useCompiledXaml);
				Assert.AreEqual("Constant", layout.constant.Text);
			}

			[TestCase(false)]
			[TestCase(true)]
			public void Field(bool useCompiledXaml)
			{
				var layout = new XStatic(useCompiledXaml);
				Assert.AreEqual("Field", layout.field.Text);
			}

			[TestCase(false)]
			[TestCase(true)]
			public void Enum(bool useCompiledXaml)
			{
				var layout = new XStatic(useCompiledXaml);
				Assert.AreEqual(ScrollOrientation.Both, layout.enuM.Orientation);
			}
		}
	}
}