using System;
using System.Threading;
using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1704, "[Enhancement] Basic GIF animation features", PlatformAffected.UWP)]
	public class Issue1704 : TabbedPage
	{
		ContentPage _page1;
		ContentPage _page2;
		ContentPage _page3;
		ContentPage _page4;

		public Issue1704()
		{
			_page1 = new OnLoadAnimationPage { Title = "On Load" };
			_page2 = new OnStartAnimationPage { Title = "On Start" };
			_page3 = new LoadImageSourceAnimationPage { Title = "Source" };
			_page4 = new MiscPage { Title = "Misc" };

			Children.Add(_page1);
			Children.Add(_page2);
			Children.Add(_page3);
			Children.Add(_page4);
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
			_referenceImageLabel = new Label {
				Text = "Reference image (no animation).",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_referenceImage = new Image {
				Source = "ie_retro.gif",
				HorizontalOptions = LayoutOptions.Start
			};

			_animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_animatedImage = new Image {
				Source = "ie_retro.gif",
				IsAnimationAutoPlay = true,
				HorizontalOptions = LayoutOptions.Start
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_referenceImageLabel,
					_referenceImage,
					_animatedImageLabel,
					_animatedImage
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
			_referenceImageLabel = new Label {
				Text = "Reference image (no animation).",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_referenceImage = new Image {
				Source = "sweden.gif",
				HorizontalOptions = LayoutOptions.Start
			};

			_animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_animatedImage = new Image {
				Source = "sweden.gif",
				IsAnimationAutoPlay = false,
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
					_referenceImageLabel,
					_referenceImage,
					_animatedImageLabel,
					_animatedImage,
					_startStopButton
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
				IsAnimationAutoPlay = true,
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
					_animatedImageLabel,
					_animatedImage,
					_imageSource,
					_loadImageButton
				}
			};
		}
	}

	class MiscPage : ContentPage
	{
		Label _noAnimationFallbackLabel;
		Image _noAnimationFallbackImage;
		Label _stressTestLabel;
		Label _stressTestIterationLabel;
		Entry _stressTestItertionEntry;
		Image _stressTestImage;
		Button _startStressTestButton;
		ProgressBar _stressTestProgressBar;
		Button _stopStressTestButton;

		int _stressTestIterationCount = 1000;
		AutoResetEvent _nextStressTest = new AutoResetEvent(false);
		bool _abortStressTest = false;

		public MiscPage()
		{
			_noAnimationFallbackLabel = new Label {
				Text = "No animation error fallback.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_noAnimationFallbackImage = new Image {
				Source = "coffee.png",
				IsAnimationAutoPlay = true,
				HorizontalOptions = LayoutOptions.Start
			};

			_stressTestLabel = new Label {
				Text = "Image loading stress test.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 24, 0, 0)
			};

			_stressTestIterationLabel = new Label {
				Text = "Test iterations:",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold
			};

			_stressTestItertionEntry = new Entry { Text = _stressTestIterationCount.ToString() };

			_stressTestImage = new Image {
				Source = "ie_retro.gif",
				IsAnimationAutoPlay = true,
				HorizontalOptions = LayoutOptions.Start,
				IsVisible = false
			};

			_startStressTestButton = new Button {
				Text = "Run Stress Test",
				Margin = new Thickness(0, 12, 0, 12)
			};

			_startStressTestButton.Clicked += (object sender, EventArgs e) => {

				_startStressTestButton.Text = "Running...";
				_startStressTestButton.IsEnabled = false;
				_stopStressTestButton.IsEnabled = true;
				_abortStressTest = false;

				int.TryParse(_stressTestItertionEntry.Text, out _stressTestIterationCount);
				ThreadPool.QueueUserWorkItem(delegate { runStressTest(); });
			};

			_stressTestProgressBar = new ProgressBar();

			_stopStressTestButton = new Button {
				Text = "Stop Stress Test",
				IsEnabled = false,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_stopStressTestButton.Clicked += (object sender, EventArgs e) => {
				_stopStressTestButton.IsEnabled = false;
				_abortStressTest = true;
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_noAnimationFallbackLabel,
					_noAnimationFallbackImage,
					_stressTestLabel,
					_stressTestIterationLabel,
					_stressTestItertionEntry,
					_stressTestImage,
					_startStressTestButton,
					_stressTestProgressBar,
					_stopStressTestButton
				}
			};
		}

		void runStressTest()
		{
			for (int i = 0; i < _stressTestIterationCount && !_abortStressTest; i++)
			{
				Device.BeginInvokeOnMainThread(() => {
					if (i % 2 == 0)
					{
						_stressTestImage.Source = "ie_retro.gif";
					}
					else
					{
						_stressTestImage.Source = "sweden.gif";
					}

					_stressTestProgressBar.Progress = (double)i / (double)_stressTestIterationCount;

					_nextStressTest.Set();
				});

				_nextStressTest.WaitOne();

				while (_stressTestImage.IsLoading)
					Thread.Sleep(10);

				Thread.Sleep(10);
			}

			Device.BeginInvokeOnMainThread(() => {
				_startStressTestButton.Text = "Run Stress Test";
				_startStressTestButton.IsEnabled = true;
				_stopStressTestButton.IsEnabled = false;
				_stressTestProgressBar.Progress = 1;
			});
		}
	}
}
