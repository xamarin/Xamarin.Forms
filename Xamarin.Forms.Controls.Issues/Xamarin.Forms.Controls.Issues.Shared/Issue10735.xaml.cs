using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10735, "[Bug] [Fatal] [Android] CollectionView Causes Application Crash When Keyboard Opens", PlatformAffected.Android)]
	public partial class Issue10735 : TestContentPage
	{
#if APP
		readonly int _addItemDelay = 300;
		int _item = 0;
		readonly int _changeFocusDelay = 1000;
		View _lastFocus;
#endif

		const string Success = "Success";

		public Issue10735()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
			StartAddingMessages();
#endif
		}

		public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();

		protected override void Init()
		{

		}

#if APP
		void StartAddingMessages()
		{
			Task.Run(async () =>
			{
				while (_item < 30)
				{
					await Task.Delay(_addItemDelay);
					Items.Add(_item.ToString());
					_item++;
				}

				Device.BeginInvokeOnMainThread(() =>
				{
					Result.Text = Success;
				});
			});

			Task.Run(async () =>
			{
				while (_item < 30)
				{
					await Task.Delay(_changeFocusDelay);
					Device.BeginInvokeOnMainThread(() =>
					{
						_lastFocus?.Unfocus();

						if (_lastFocus == _editor)
							_lastFocus = _button;
						else
							_lastFocus = _editor;

						_lastFocus.Focus();
					});
				}
			});
		}
#endif

#if UITEST
		[Test]
		public void KeyboardOpeningDuringCollectionViewAnimationShouldNotCrash()
		{
			RunningApp.WaitForElement(Success, timeout: TimeSpan.FromSeconds(15));
		}
#endif
	}
}