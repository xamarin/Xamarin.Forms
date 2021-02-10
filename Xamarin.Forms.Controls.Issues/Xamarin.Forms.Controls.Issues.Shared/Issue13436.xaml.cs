﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13436,
		"[Bug] Java.Lang.IllegalArgumentException in CarouselView adjusting PeekAreaInsets in OnSizeAllocated using XF 5.0",
		PlatformAffected.Android)]
	public partial class Issue13436 : TestContentPage
	{
		public Issue13436()
		{
#if APP
			InitializeComponent();
#endif
		}

#if APP
		double _prevWidth;

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Carousel.ItemsSource = new List<Issue13436Model>
			{
				new Issue13436Model
				{
					Name = "N1",
					Desc = "D1",
					Color = Color.Yellow
				},
				new Issue13436Model
				{
					Name = "N2",
					Desc = "D2",
					Color = Color.Orange
				},
				new Issue13436Model
				{
					Name = "N3",
					Desc = "D3",
					Color = Color.AliceBlue
				}
			};
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			if (Math.Abs(width - _prevWidth) < .1)
			{
				return;
			}

			_prevWidth = width;
			Carousel.PeekAreaInsets = width * .15;
		}
#endif
		protected override void Init()
		{
		}

#if UITEST && __ANDROID__
		[Test]
		public void ChangePeekAreaInsetsInOnSizeAllocatedTest()
        {
            RunningApp.WaitForElement("CarouselId");
        }
#endif
	}

	[Preserve(AllMembers = true)]
	public class Issue13436Model
	{
		public string Name { get; set; }
		public string Desc { get; set; }
		public Color Color { get; set; }
		public double Scale { get; set; }
	}
}