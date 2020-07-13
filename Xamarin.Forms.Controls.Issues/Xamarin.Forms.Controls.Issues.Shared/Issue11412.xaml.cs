using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.CustomAttributes;
using System.Collections.Generic;
using System;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ManualReview)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11412, "Bug] MediaElement will crash when page popup", PlatformAffected.iOS)]
	public partial class Issue11412 : ContentPage
	{
		public Issue11412()
		{
#if APP
			Device.SetFlags(new List<string> { ExperimentalFlags.MediaElementExperimental });
			InitializeComponent();
#endif
        }

#if APP
        void OnPlayPauseButtonClicked(object sender, EventArgs args)
        {
            if (mediaElement.CurrentState == MediaElementState.Closed ||
                mediaElement.CurrentState == MediaElementState.Stopped ||
                mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
            }
            else if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
            }
        }

        void OnStopButtonClicked(object sender, EventArgs args)
        {
            mediaElement.Stop();
        }
#endif
    }
}