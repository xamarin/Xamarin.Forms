﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Gh4215 : ContentPage
	{
		public class Gh4215VM
		{
			public static implicit operator DateTime(Gh4215VM value) => DateTime.UtcNow;
			public static implicit operator string(Gh4215VM value) => "foo";
			public static implicit operator long(Gh4215VM value) => long.MaxValue;
			public static implicit operator Rectangle(Gh4215VM value) => new Rectangle();
		}

		public Gh4215()
		{
			InitializeComponent();
		}

		public Gh4215(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
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

			[TestCase(true), TestCase(false)]
			public void AvoidAmbiguousMatch(bool useCompiledXaml)
			{
				var layout = new Gh4215();
				Assert.DoesNotThrow(() => layout.BindingContext = new Gh4215VM());
				Assert.That(layout.l0.Text, Is.EqualTo("foo"));
			}
		}
	}
}
