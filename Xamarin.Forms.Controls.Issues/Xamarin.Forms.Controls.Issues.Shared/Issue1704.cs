using System;
using System.Collections.Generic;
using System.Threading;
using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1704, "[Enhancement] Basic GIF animation features", PlatformAffected.UWP)]
	public class Issue1704 : TestTabbedPage
	{
		ContentPage _page1;
		ContentPage _page2;
		ContentPage _page3;
		ContentPage _page4;

		protected override void Init()
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

#if UITEST
		[Test]
		[Category(UITestCategories.ManualReview)]
		public void Issue1704Test()
		{
			RunningApp.WaitForElement("On Load");
			RunningApp.WaitForElement("On Start");
			RunningApp.WaitForElement("Source");
			RunningApp.WaitForElement("Misc");

			RunningApp.Tap(q => q.Marked("On Load"));
			RunningApp.Tap(q => q.Marked("On Start"));
			RunningApp.WaitForElement(q => q.Marked("Start Animation"));
			RunningApp.Tap(q => q.Marked("Start Animation"));
			RunningApp.Tap(q => q.Marked("Stop Animation"));

			RunningApp.Tap(q => q.Marked("Misc"));
			RunningApp.WaitForElement(q => q.Marked("Start Animation"));
			RunningApp.Tap(q => q.Marked("Start Animation"));
			RunningApp.Tap(q => q.Marked("Stop Animation"));
		}
#endif
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
		ImageButton _animatedImageButton;
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
			_animatedImageButton = new ImageButton
			{
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
					_animatedImage.StartAnimation();
					_animatedImageButton.StartAnimation();
					_startStopButton.Text = "Stop Animation";
				}
				else
				{
					_animatedImage.StopAnimation();
					_animatedImageButton.StopAnimation();
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
					_animatedImageButton,
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
	//
	// Small animated GIF (24 KB compressed, 14 frames)
	// https://media.giphy.com/media/qyjQsUt0p0TT2/giphy.gif
	//
	// Medium animated GIF (184 KB compressed, 30 frames)
	// https://media.giphy.com/media/xThta5b9vezPO75kL6/giphy.gif
	//
	// Semi large GIF (447 KB, 48 frames).
	// https://media.giphy.com/media/AWNxDbtHGIJDW/giphy.gif
	//
	// Large animated GIF (3 MB compressed, 192 frames).
	// https://media.giphy.com/media/YVYRtHiAv1t8Q/giphy.gif
	//
	// Large animated GIF that could trigger OOM scenarios and slow load times (12 MB compressed, 240 frames).
	// http://media.giphy.com/media/mf8UbIDew7e8g/giphy.gif
	//
	class LoadImageSourceAnimationPage : ContentPage
	{
		Label _animatedImageLabel;
		Image _animatedImage;
		Entry _imageSource;
		Button _loadImageButton;
		ActivityIndicator _loadingIndicator;

		class TimerContextData
		{
			public Image AnimationImage { get; set; }
			public Entry ImageSource { get; set; }
			public Button LoadButton { get; set; }
			public ActivityIndicator LoadIndicator { get; set; }
			public Timer Timer { get; set; }
		}

		public LoadImageSourceAnimationPage()
		{
			_animatedImageLabel = new Label {
				Text = "Animated image.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_animatedImage = new Image {
				HorizontalOptions = LayoutOptions.Start,
				IsAnimationAutoPlay = true,
			};

			_imageSource = new Entry { Placeholder = "Image Source" };

			_imageSource.Focused += (object sender, FocusEventArgs e) => {
				_imageSource.TextColor = Color.Default;
			};

			_loadImageButton = new Button { Text = "Load Image" };
			_loadImageButton.Clicked += (object sender, EventArgs e) => {
				if (!string.IsNullOrEmpty(_imageSource.Text) && !_animatedImage.IsLoading)
				{
					try
					{
						_loadImageButton.IsEnabled = false;
						_imageSource.IsEnabled = false;
						_loadingIndicator.IsVisible = true;
						_loadingIndicator.IsRunning = true;

						_animatedImage.Source = ImageSource.FromUri(new Uri(_imageSource.Text));

						var timerContext = new TimerContextData {
							AnimationImage = _animatedImage,
							ImageSource = _imageSource,
							LoadButton = _loadImageButton,
							LoadIndicator = _loadingIndicator
						};

						var onLoadCompleteTimer = new Timer(OnLoadImageComplete, timerContext, 100, 100);
						timerContext.Timer = onLoadCompleteTimer;
					}
					catch (Exception)
					{
						_imageSource.TextColor = Color.Red;
						_loadImageButton.IsEnabled = true;
						_imageSource.IsEnabled = true;
						_loadingIndicator.IsVisible = false;
						_loadingIndicator.IsRunning = false;
					}
				}
			};

			_loadingIndicator = new ActivityIndicator {
				IsVisible = false,
				IsRunning = false
			};

			Content = new StackLayout {
				Padding = new Thickness(0, 16),
				Children = {
					_animatedImageLabel,
					_animatedImage,
					_imageSource,
					_loadingIndicator,
					_loadImageButton
				}
			};
		}

		static void OnLoadImageComplete(Object state)
		{
			if (state is TimerContextData context)
			{
				lock (context)
				{
					if (context.AnimationImage != null && !context.AnimationImage.IsLoading)
					{
						var animationImage = context.AnimationImage;
						var imageSource = context.ImageSource;
						var loadButton = context.LoadButton;
						var loadingIndicator = context.LoadIndicator;

						context.Timer?.Dispose();
						context.Timer = null;

						context.AnimationImage = null;
						context.ImageSource = null;
						context.LoadButton = null;
						context.LoadIndicator = null;

						Device.BeginInvokeOnMainThread(() => {
							if (loadButton != null)
								loadButton.IsEnabled = true;

							if (loadingIndicator != null)
							{
								loadingIndicator.IsVisible = false;
								loadingIndicator.IsRunning = false;
							}

							if (imageSource != null)
								imageSource.IsEnabled = true;

							if (animationImage != null)
							{
								animationImage.StartAnimation();
							}
						});
					}
				}
			}
		}
	}

	class MiscPage : ContentPage
	{
		Label _noAnimationFallbackLabel;
		Image _noAnimationFallbackImage;
		Label _initNoAnimationLabel;
		Image _initNoAnimationImage;
		Button _initNoAnimationButton;
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

			_initNoAnimationLabel = new Label {
				Text = "Initial loaded without animation.",
				FontSize = 12,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(0, 12, 0, 12)
			};

			_initNoAnimationImage = new Image {
				Source = "ie_retro.gif",
				HorizontalOptions = LayoutOptions.Start
			};

			_initNoAnimationButton = new Button {
				Text = "Start Animation",
				Margin = new Thickness(0, 12, 0, 12)
			};

			_initNoAnimationButton.Clicked += (object sender, EventArgs e) => {

				if (!_initNoAnimationImage.IsAnimationPlaying)
				{
					_noAnimationFallbackImage.StartAnimation();
					_initNoAnimationImage.StartAnimation();
					_initNoAnimationButton.Text = "Stop Animation";
				}
				else
				{
					_noAnimationFallbackImage.StopAnimation();
					_initNoAnimationImage.StopAnimation();
					_initNoAnimationButton.Text = "Start Animation";
				}
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
					_initNoAnimationLabel,
					_initNoAnimationImage,
					_initNoAnimationButton,
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
				if (!_abortStressTest)
					_stressTestProgressBar.Progress = 1;
			});
		}
	}
}
