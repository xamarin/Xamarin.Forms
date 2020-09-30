﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	public class CustomFrame10348 : Frame
	{

	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10348, "[Bug] Cannot change shadow appearance on Frame from version 4.5.x", PlatformAffected.iOS)]
	public partial class Issue10348 : ContentPage
	{
		public Issue10348()
		{
#if APP
			InitializeComponent();
#endif
		}
	}

}