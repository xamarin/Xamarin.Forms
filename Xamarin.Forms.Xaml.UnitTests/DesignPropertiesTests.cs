﻿using System;
using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class DesignPropertiesTests
	{
		[SetUp] public void Setup() => Device.PlatformServices = new MockPlatformServices();
		[TearDown] public void TearDown() => Device.PlatformServices = null;

		[Test]
		public void DesignProperties()
		{
			var xaml = @"
				<ContentPage
						xmlns=""http://xamarin.com/schemas/2014/forms""
						xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
						xmlns:d=""http://xamarin.com/schemas/2014/forms/design""
						xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
						mc:Ignorable=""d"">
					<Label  d:Text=""Bar"" Text=""Foo"" x:Name=""label"" />
				</ContentPage>";

			var view = new ContentPage();
			XamlLoader.Load(view, xaml, useDesignProperties: true); //this is equiv as LoadFromXaml, but with the bool set

			var label = ((Forms.Internals.INameScope)view).FindByName("label") as Label;

			Assert.That(label.Text, Is.EqualTo("Bar"));
		}
	}
}
