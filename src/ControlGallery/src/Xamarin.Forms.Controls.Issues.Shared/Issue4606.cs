﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
	[Issue(IssueTracker.Github, 4606, "[Android] ImageButton on android is not clipping view to corner radius",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Button)]
	[NUnit.Framework.Category(UITestCategories.ImageButton)]
	[NUnit.Framework.Category(UITestCategories.ManualReview)]
#endif
	public class Issue4606 : TestContentPage
	{
		protected override void Init()
		{
			Content = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "Make sure all the images are clipped to the corner radius. Button will only clip when using a fast renderer.",
					},
					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Button()
							{
								HeightRequest = 50,
								WidthRequest = 50,
								CornerRadius = 25,
								BackgroundColor = Color.Purple,
								BorderColor = Color.Green,
								ImageSource = "coffee.png"
							},
							new ImageButton()
							{
								HeightRequest = 50,
								WidthRequest = 50,
								CornerRadius = 25,
								BackgroundColor = Color.Purple,
								BorderColor = Color.Green,
								Source = "coffee.png"
							}
						}
					},
					new StackLayout()
					{
						Visual = VisualMarker.Material,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Button()
							{
								HeightRequest = 50,
								WidthRequest = 50,
								CornerRadius = 25,
								BackgroundColor = Color.Purple,
								BorderColor = Color.Green,
								ImageSource = "coffee.png"
							},
							new ImageButton()
							{
								HeightRequest = 50,
								WidthRequest = 50,
								CornerRadius = 25,
								BackgroundColor = Color.Purple,
								BorderColor = Color.Green,
								Source = "coffee.png"
							}
						}
					},
					new Label()
					{
						Text = "The box view height should match the button border width"
					},
					new BoxView()
					{
						BackgroundColor = Color.Red,
						HeightRequest = 2
					},
					new Button()
					{
						BorderColor = Color.Red,
						BackgroundColor = Color.White,
						CornerRadius = 25,
						BorderWidth = 2
					},
					new Label()
					{
						Text = "The box view height should match the button border width"
					},
					new BoxView()
					{
						BackgroundColor = Color.Red,
						HeightRequest = 5
					},
					new Button()
					{
						BorderColor = Color.Red,
						BackgroundColor = Color.White,
						CornerRadius = 25,
						BorderWidth = 5
					},
					new Label()
					{
						Text = "The box view height should match the button border width"
					},
					new BoxView()
					{
						BackgroundColor = Color.Red,
						HeightRequest = 1
					},
					new Button()
					{
						BorderColor = Color.Red,
						BackgroundColor = Color.White,
						CornerRadius = 25,
						BorderWidth = 1
					}
				}
			};
		}
	}
}
