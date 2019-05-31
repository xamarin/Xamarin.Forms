﻿using System;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class HRTests
	{
		[SetUp]
		public void Setup()
		{
			Device.PlatformServices = new MockPlatformServices();
			Xamarin.Forms.Internals.Registrar.RegisterAll(new Type[0]);
			Application.Current = null;
		}

		[TearDown]
		public void TearDown()
		{
			Device.PlatformServices = null;
			XamlLoader.FallbackTypeResolver = null;
			XamlLoader.ValueCreatedCallback = null;
			XamlLoader.InstantiationFailedCallback = null;
			Forms.Internals.ResourceLoader.ExceptionHandler2 = null;
#pragma warning disable 0618
			Internals.XamlLoader.DoNotThrowOnExceptions = false;
#pragma warning restore 0618
			Application.ClearCurrent();
		}

		[Test]
		public void LoadResources()
		{
			var app = @"
				<Application xmlns=""http://xamarin.com/schemas/2014/forms""
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"">
					<Application.Resources>
						<ResourceDictionary>
							<Color x:Key=""almostPink"">HotPink</Color>
						</ResourceDictionary>
					</Application.Resources>
				</Application>
			";
			Assert.That(Application.Current, Is.Null);
			var mockApplication = new MockApplication();
			var rd = XamlLoader.LoadResources(app, mockApplication);
			Assert.That(rd, Is.TypeOf<ResourceDictionary>());
			Assert.That(((ResourceDictionary)rd).Count, Is.EqualTo(1));

			//check that the live app hasn't ben modified
			Assert.That(Application.Current, Is.EqualTo(mockApplication));
			Assert.That(Application.Current.Resources.Count, Is.EqualTo(0));
		}
	}
}
