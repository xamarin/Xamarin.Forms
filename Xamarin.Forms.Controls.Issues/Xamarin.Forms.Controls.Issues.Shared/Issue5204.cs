﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5204, "[MacOS] Image size issue (not rendered if skip setting WidthRequest and HeightRequest", PlatformAffected.macOS)]
	public class Issue5204 : TestContentPage
	{
		protected override void Init()
		{
			Title = "You should see image";
			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = {
					new Image
					{
						BackgroundColor = Color.Black,
						Source = "https://user-images.githubusercontent.com/10124814/53306353-27302b80-389d-11e9-98ce-690db32f1ee3.jpg"
					}
				}
			};
		}
	}
}