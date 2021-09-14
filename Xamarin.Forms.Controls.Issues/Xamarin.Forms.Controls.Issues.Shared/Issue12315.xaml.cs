﻿using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 12315, "[Bug] Button disappears when setting CornerRadius",
		PlatformAffected.iOS)]
	public partial class Issue12315 : TestContentPage
	{
		public Issue12315()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}

#if APP
		void OnMarginSliderValueChanged(object sender, ValueChangedEventArgs e) => IssueButton.CornerRadius = (int)e.NewValue;
#endif
	}
}