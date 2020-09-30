﻿using System;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UITests.Shared
{
	internal class PlatformAutomatedTest : BaseTestFixture
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.PlatformAutomatedTestsGallery);
		}

		[Test]
		[Category(UITestCategories.ViewBaseTests)]
		public void AutomatedTests()
		{
			App.WaitForElement("SUCCESS", timeout: TimeSpan.FromMinutes(2));
		}
	}
}
