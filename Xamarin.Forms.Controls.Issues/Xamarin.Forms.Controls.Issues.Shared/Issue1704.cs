using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1704, "[Enhancement] Basic gif animation features", PlatformAffected.UWP)]
	public class Issue1704 : TabbedPage
	{
		ContentPage _page1;
		ContentPage _page2;
		ContentPage _page3;

		public Issue1704()
		{
			_page1 = new OnLoadAnimationPage { Title = "OnLoad" };
			_page2 = new OnStartAnimationPage { Title = "OnStart" };
			_page3 = new LoadImageSourceAnimationPage { Title = "LoadSource" };

			Children.Add(_page1);
			Children.Add(_page2);
			Children.Add(_page3);
		}
	}

	class OnLoadAnimationPage : ContentPage
	{
		Label _referenceImageLabel;
		Image _referenceImage;
		Label _animatedImageLabel;
		Image _animatedImage;

		public OnLoadAnimationPage()
		{
			var _referenceImageLabel = new Label {
				Text = "Reference image (no animation).",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			var _referenceImage = new Image {
				Source = "ie_retro.gif",
				HorizontalOptions = LayoutOptions.Start
			};

			var _animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			var _animatedImage = new Image {
				Source = "ie_retro.gif",
				AnimationPlayBehavior = Image.AnimationPlayBehaviorValue.OnLoad,
				HorizontalOptions = LayoutOptions.Start
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_referenceImageLabel, _referenceImage, _animatedImageLabel, _animatedImage
				}
			};
		}
	}

	class OnStartAnimationPage : ContentPage
	{
		Label _referenceImageLabel;
		Image _referenceImage;
		Label _animatedImageLabel;
		Image _animatedImage;
		Button _startStopButton;

		public OnStartAnimationPage()
		{
			var _referenceImageLabel = new Label {
				Text = "Reference image (no animation).",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			var _referenceImage = new Image {
				Source = "sweden.gif",
				HorizontalOptions = LayoutOptions.Start
			};

			var _animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			var _animatedImage = new Image {
				Source = "sweden.gif",
				AnimationPlayBehavior = Image.AnimationPlayBehaviorValue.OnStart,
				HorizontalOptions = LayoutOptions.Start
			};

			_animatedImage.AnimationFinishedPlaying += OnAnimationFinishedPlaying;

			_startStopButton = new Button { Text = "Start Animation" };
			_startStopButton.Clicked += (object sender, EventArgs e) => {
				if (!_animatedImage.IsAnimationPlaying)
				{
					_referenceImage.StartAnimation(); // Shouldn't have any effect.
					_animatedImage.StartAnimation();
					_startStopButton.Text = "Stop Animation";
				}
				else
				{
					_referenceImage.StopAnimation(); // Shouldn't have any effect.
					_animatedImage.StopAnimation();
					_startStopButton.Text = "Start Animation";
				}
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_referenceImageLabel, _referenceImage, _animatedImageLabel, _animatedImage, _startStopButton
				}
			};
		}

		void OnAnimationFinishedPlaying(object sender, EventArgs e)
		{
			_startStopButton.Text = "Start Animation";
		}
	}

	// Example URI's:
	// http://media.giphy.com/media/mf8UbIDew7e8g/giphy.gif
	// https://media.giphy.com/media/AWNxDbtHGIJDW/giphy.gif
	// https://media.giphy.com/media/YVYRtHiAv1t8Q/giphy.gif
	class LoadImageSourceAnimationPage : ContentPage
	{
		Label _animatedImageLabel;
		Image _animatedImage;
		Entry _imageSource;
		Button _loadImageButton;

		public LoadImageSourceAnimationPage()
		{
			_animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_animatedImage = new Image {
				AnimationPlayBehavior = Image.AnimationPlayBehaviorValue.OnLoad,
				HorizontalOptions = LayoutOptions.Start
			};

			_imageSource = new Entry { Placeholder = "Image Source" };

			_imageSource.Focused += (object sender, FocusEventArgs e) => {
				_imageSource.TextColor = Color.Default;
			};

			_loadImageButton = new Button { Text = "Load Image" };
			_loadImageButton.Clicked += (object sender, EventArgs e) => {
				if (!string.IsNullOrEmpty(_imageSource.Text))
				{
					try
					{
						_animatedImage.Source = ImageSource.FromUri(new Uri(_imageSource.Text));
					}
					catch (Exception)
					{
						_imageSource.TextColor = Color.Red;
					}
				}
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_animatedImageLabel, _animatedImage, _imageSource, _loadImageButton
				}
			};
		}
	}
}
