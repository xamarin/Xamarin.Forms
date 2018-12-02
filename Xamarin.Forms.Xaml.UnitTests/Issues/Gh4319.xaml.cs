﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class Gh4319 : ContentPage
	{
		public Gh4319() => InitializeComponent();
		public Gh4319(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[TestCase(true), TestCase(false)]
			public void OnPlatformMarkupAndNamedSizes(bool useCompiledXaml)
			{
				((MockPlatformServices)Device.PlatformServices).RuntimePlatform = Device.iOS;
				var layout = new Gh4319(useCompiledXaml);
				Assert.That(layout.label.FontSize, Is.EqualTo(4d));

				((MockPlatformServices)Device.PlatformServices).RuntimePlatform = Device.Android;
				layout = new Gh4319(useCompiledXaml);
				Assert.That(layout.label.FontSize, Is.EqualTo(8d));
			}
		}
	}
}