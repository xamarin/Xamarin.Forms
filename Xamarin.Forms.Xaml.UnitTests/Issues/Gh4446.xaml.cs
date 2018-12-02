﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using NUnit.Framework;

using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
    public class Gh4446Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

	public partial class Gh4446 : ContentPage
	{
		public Gh4446()
		{
			InitializeComponent();
		}

		public Gh4446(bool useCompiledXaml)
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
			public void BindingThrowsOnWrongConverterParameter(bool useCompiledXaml)
			{
				Assert.DoesNotThrow(() => new Gh4446(useCompiledXaml) { BindingContext = new Gh4446Item { Text = null } });
			}
		}
	}
}
