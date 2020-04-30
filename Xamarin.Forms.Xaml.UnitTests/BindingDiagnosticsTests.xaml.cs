// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Xaml.Diagnostics;

namespace Xamarin.Forms.Xaml.UnitTests
{
	public partial class BindingDiagnosticsTests : ContentPage
	{
		public BindingDiagnosticsTests() => InitializeComponent();

		public BindingDiagnosticsTests(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();

			[TearDown] public void TearDown() => Device.PlatformServices = null;

			[TestCase(false)]
			//[TestCase(true)]
			public void Test(bool useCompiledXaml)
			{
				List<(XamlSourceInfo source, string message)> failures = new List<(XamlSourceInfo failure, string message)>();
				BindingDiagnostics.BindingFailed += (o, e) => failures.Add((e.XamlSourceInfo, string.Format(e.Message, e.MessageArgs)));
				var layout = new BindingDiagnosticsTests(useCompiledXaml) { BindingContext = new { foo = "bar" } };
				Assert.That(failures.Count, Is.GreaterThan(0));
			}
		}
	}
}