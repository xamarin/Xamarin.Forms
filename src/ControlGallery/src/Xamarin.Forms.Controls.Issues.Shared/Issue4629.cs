﻿using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4629, "Picker/DatePicker/TimePicker on iOS iPad should NOT have word suggestions", PlatformAffected.iOS)]
	public class Issue4629 : ContentPage
	{
		public Issue4629()
		{
			var picker = new Picker()
			{
				ItemsSource = new List<string>()
				{
					"Apple",
					"Banana",
					"Peach"
				},
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			var datePicker = new DatePicker();
			var timePicker = new TimePicker();
			Content = new StackLayout()
			{
				Children = { picker, datePicker, timePicker },
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
		}
	}
}
